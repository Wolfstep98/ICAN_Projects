using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class CircleBehaviours : MonoBehaviour {


    [Header("Circle type")]
    public bool isRotating;
    public bool isRotatingTyToPlayer;
    public bool isExploding;
    public bool isLighting;
    public bool isInteractible;
    public bool isTiming;
    public bool isLinked;

    [Header("Rotation properties")]
    public bool rotateRight = true;
    public float rotateSpeed = 5f;

    [Header("Explosion gestion properties")]
    public bool randomTimeBetweenMinAndMax = false;

    public float timeBeforeExplosionMin = 1f;
    public float timeBeforeExplosionMax = 2f;

    [Range(0f, 10f)]
    public float timeBeforeExplosion = 2f;

    [Header("Lighting properties")]
    public float fadingTime = 5f;
    public float fadeTimeRemaining = 0f;
    public float currentAngularVel;

    public enum LightSphereBehaviour
    {
        lightFadesAfterXSec,
        lightOffWhenLeaving,
        lightOnAfterATour
    }
    public LightSphereBehaviour lightSphereBehaviour = LightSphereBehaviour.lightFadesAfterXSec;

    [Header("Moving properties")]
    public bool neBougePas = false;

    [Header("Timing properties")]
    public float timeBetweenSwitchs = 2f;
    public Color offColor;
    public Color onColor;
    public bool startOff;

    [Header("Lien")]
    public GameObject[] circleLinked;
    [SerializeField]
    public Dictionary<GameObject, float> circleLinkedDist;
    public int nbrDeLienMax = 2;
    public int nbrDeLien = 0;

    public GameObject lien;
    public GameObject[] liens;

    public Color lastColor;
    public Color currentColor;

    public GameObject player;

    public ColoredCircleHandler coloredCircleHandler;

    [Header("Debug")]

    //bool
    public bool canExplode;
    public bool isFading;
    public bool isCharging;
    private bool isMoving;
    public bool isClicking;
    public bool isOnColor;
    public bool isChangingColor;

    //float
    public float coutdown = 0;
    public float coutdownInit;
    public float maxSpeed = 0f;
    public float switchsTime = 0f;

    //Vector3
    public Vector3 initRot;
    public Vector3 currentRot;
    public Vector3 lastMousePos;
    public Vector3 mousePosition;
    public Vector3 objPos;

    public Vector3 circleTrajectory;

    //Color
    public Color initialColor;
    public Color meshColor;

    //MeshRenderer
    public MeshRenderer mesh;
    public SpriteRenderer spriteRenderer;

    //Camera
    public Camera mainCam;

    //Scripts
    GameManager2 script;
    PlayerBehaviours playerScript;

    //Rigidbody
    private Rigidbody2D rigid;

    //GameObject
    GameObject lightChild;

    // Use this for initialization
    void Start()
    {
        isChangingColor = false;
        canExplode = false;
        isFading = false;
        isMoving = false;

        currentAngularVel = 0f;
        maxSpeed = 0f;

        //lightChild = transform.GetChild(0).gameObject;

        script = GameObject.Find("GameManager").GetComponent<GameManager2>();
        playerScript = GameObject.Find("Ball").GetComponent<PlayerBehaviours>();
        mainCam = GameObject.Find("Main Camera").GetComponent<Camera>();

        //mesh = GetComponent<MeshRenderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();

        
        circleTrajectory = Vector3.zero;

        if(isTiming && !playerScript.colorOnCollision)
        {
            float randNum = Random.value;
            startOff = (randNum <= 0.5f) ? true : false;
            if(startOff)
            {
                spriteRenderer.material.color = offColor;
                isOnColor = false;
            }
            else
            {
                spriteRenderer.material.color = onColor;
                isOnColor = true;
            }
            switchsTime += Time.deltaTime;
        }
        else
        {
            initialColor = spriteRenderer.material.color;
        }

        //Initialize circle wich can be linked
        circleLinkedDist = new Dictionary<GameObject, float>();
        for(int i = 0; i < circleLinked.Length;i++)
        {
            circleLinkedDist.Add(circleLinked[i], Vector3.Distance(transform.position, circleLinked[i].transform.position));
        }
        lien = Resources.Load<GameObject>("Prefabs/Test/Lien");
        lastColor = currentColor = spriteRenderer.material.color;

        liens = new GameObject[nbrDeLienMax];

        coloredCircleHandler = GameObject.Find("GameManager").GetComponent<ColoredCircleHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        currentColor = spriteRenderer.material.color;
        if (isRotating)
            transform.Rotate(Vector3.forward, ((rotateRight) ? rotateSpeed : -rotateSpeed) * Time.deltaTime);

        if(isRotatingTyToPlayer)
        {
            transform.Rotate(Vector3.forward, ((rotateRight) ? rotateSpeed : -rotateSpeed) * Time.deltaTime);
        }
        if (isExploding)
        {
            if (canExplode)
            {
                coutdown -= Time.deltaTime;
                if (coutdown > 0)
                {
                    meshColor = Color.Lerp(initialColor, Color.black, 1f - (coutdown / coutdownInit));
                    spriteRenderer.material.color = meshColor;
                }
                if (coutdown <= 0)
                    Destroy(gameObject);
            }
        }

        if (isLighting)
        {
            if (lightSphereBehaviour == LightSphereBehaviour.lightFadesAfterXSec && isFading)
            {
                fadeTimeRemaining -= Time.deltaTime;
                if (fadeTimeRemaining <= 0f)
                {
                    isFading = false;
                    fadeTimeRemaining = 0f;
                    lightChild.SetActive(false);
                }
            }

            if (lightSphereBehaviour == LightSphereBehaviour.lightOnAfterATour && isCharging)
            {
                //currentRot = transform.rotation.eulerAngles;
                currentAngularVel += rotateSpeed * Time.deltaTime;
            }
        }
        else
        {
            //if (lightChild.activeSelf)
                //lightChild.SetActive(false);
        }

        if (isInteractible)
        {
            if (rigid.bodyType != RigidbodyType2D.Dynamic)
                rigid.bodyType = RigidbodyType2D.Dynamic;
            if (neBougePas)
                rigid.isKinematic = !isMoving;
            else
                rigid.isKinematic = false;
            if(!isClicking)
            {
                rigid.velocity = new Vector2(circleTrajectory.x * maxSpeed * Time.deltaTime, circleTrajectory.y * maxSpeed * Time.deltaTime);
                rigid.velocity = Vector2.ClampMagnitude(rigid.velocity, maxSpeed);
            }
        }
        else
        {
            if (rigid.bodyType == RigidbodyType2D.Dynamic)
                rigid.bodyType = RigidbodyType2D.Static;
        }
        if(isTiming && !playerScript.colorOnCollision)
        {
            if(Input.GetKeyDown(KeyCode.S))
            {
                   switchsTime = timeBetweenSwitchs;
            }
            if(switchsTime % timeBetweenSwitchs <= 0.1)
            {
                if (!isChangingColor)
                {
                    spriteRenderer.material.color = (isOnColor) ? offColor : onColor;
                    isOnColor = !isOnColor;
                }
                isChangingColor = true;
            }
            else
            {
                isChangingColor = false;
            }
            switchsTime += Time.deltaTime;
        }
        if(isLinked)
        {
            for(int i = 0; i < circleLinked.Length;i++)
            {
                //CircleBehaviours script = circleLinked[i].GetComponent<CircleBehaviours>();
                Renderer render = circleLinked[i].GetComponent<Renderer>();
                if (render.GetComponent<CircleBehaviours>().currentColor == currentColor) 
                {
                    //Debug.DrawLine(transform.position, circleLinked[i].transform.position);
                    //Debug.Log("Colors Checked");
                    if (currentColor != lastColor )//&& nbrDeLien < nbrDeLienMax)
                    {
                        //Debug.Log("New Color");
                        if (coloredCircleHandler.lienBetweenCircles.ContainsKey(new KeyValuePair<GameObject, GameObject>(gameObject, circleLinked[i])) || coloredCircleHandler.lienBetweenCircles.ContainsKey(new KeyValuePair<GameObject, GameObject>(circleLinked[i],gameObject)))
                        {
                            //Debug.Log("Update colors");
                            Debug.Log("frame : " + Time.frameCount + "player : " + player.name);
                            coloredCircleHandler.CreateLink(player, gameObject, circleLinked[i]);
                            //GameObject lienInstance = Instantiate(lien);
                            //GameObject lien = coloredCircleHandler.lienBetweenCircles[new KeyValuePair<GameObject, GameObject>(gameObject, circleLinked[i])];
                            //lien.name = "Lien" + player.GetComponent<PlayerBehaviours>().player.ToString();
                            //lien.GetComponent<Renderer>().material.color = currentColor;
                            //lien.SetActive(true);
                            //lienInstance.GetComponent<LineRenderer>().SetPositions(new Vector3[] { transform.position, circleLinked[i].transform.position });
                            //EdgeCollider2D lienCollider = lienInstance.GetComponent<EdgeCollider2D>();
                            //lienCollider.points = new Vector2[] { transform.position, circleLinked[i].transform.position };
                        }
                    }
                }
            }
        }
        lastColor = spriteRenderer.material.color;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if(isRotatingTyToPlayer)
            {
                Vector2 normal = collision.contacts[0].normal;
                Vector2 playerVelocity = collision.gameObject.GetComponent<PlayerBehaviours>().playerTrajectory;
                Vector2 contact = Vector2.zero;
                normal.Normalize();
                playerVelocity.Normalize();
                float sign = Mathf.Sign((normal.x - contact.x) * (playerVelocity.y - normal.y) - (normal.y - contact.y) * (playerVelocity.x - normal.x));
                rotateRight = (sign >= 0) ? false : true;

            }
            if(isExploding)
            {
                if(!canExplode)
                {
                    coutdownInit = coutdown = (randomTimeBetweenMinAndMax) ? Random.Range(timeBeforeExplosionMin, timeBeforeExplosionMax) : timeBeforeExplosion;
                    canExplode = true;
                }
            }
            if (isLighting)
            {
                if (lightSphereBehaviour == LightSphereBehaviour.lightFadesAfterXSec)
                {
                    fadeTimeRemaining = fadingTime;
                    isFading = true;
                    lightChild.SetActive(true);
                }
                else if (lightSphereBehaviour == LightSphereBehaviour.lightOffWhenLeaving)
                {
                    lightChild.SetActive(true);
                }
                else if (lightSphereBehaviour == LightSphereBehaviour.lightOnAfterATour)
                {
                    initRot = currentRot = transform.rotation.eulerAngles;
                    isCharging = true;
                    currentAngularVel = 0f;
                }
            }
            if(isLinked)
            {
                player = collision.gameObject;
                currentColor = player.GetComponent<PlayerBehaviours>().playerColor;
            }
        }
        else if(collision.gameObject.tag == "Mur")
        {
            if (script.wallType == GameManager2.WallType.bounce)
            {
                if (collision.gameObject.name == "MurHaut" || collision.gameObject.name == "MurBas")
                {
                    circleTrajectory.y *= -1;
                    circleTrajectory.y += Random.Range(-0.05f, 0.05f);
                }
                else if (collision.gameObject.name == "MurGauche" || collision.gameObject.name == "MurDroit")
                {
                    circleTrajectory.x *= -1;
                    circleTrajectory.x += Random.Range(-0.05f, 0.05f);
                }
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isLighting)
        {
            if (lightSphereBehaviour == LightSphereBehaviour.lightOnAfterATour)
            {
                if (currentAngularVel >= 360f && isCharging)
                {
                    lightChild.SetActive(true);
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (isLighting)
        {
            if (collision.gameObject.tag == "Player")
            {
                if (lightSphereBehaviour == LightSphereBehaviour.lightOffWhenLeaving)
                {
                    lightChild.SetActive(false);
                }
            }
            else if (lightSphereBehaviour == LightSphereBehaviour.lightOnAfterATour)
            {
                if (isCharging && !lightChild.activeSelf)
                {
                    currentRot = initRot = Vector3.zero;
                    currentAngularVel = 0f;
                }
            }
        }
    }

    private void OnMouseDown()
    {
        if (isInteractible)
        {
            isClicking = true;
            lastMousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            if (neBougePas)
                isMoving = true;
        }
    }

    private void OnMouseDrag()
    {
        if (isInteractible)
        {
            mousePosition = mainCam.ScreenToWorldPoint(Input.mousePosition);
            if (lastMousePos != mousePosition)
            {
                Vector3 translationVec = mousePosition - lastMousePos;
                translationVec.z = 0f;
                objPos = translationVec;
                transform.position += objPos;
                maxSpeed = translationVec.magnitude / Time.deltaTime;
                circleTrajectory = translationVec.normalized;
            }
            lastMousePos = mousePosition;
        }
    }

    private void OnMouseUp()
    {
        if (isInteractible)
        {
            isClicking = false;
            if (neBougePas)
                isMoving = false;
        }
    }

    private void OnDestroy()
    {
        if (isExploding)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            for (int player = 0; player < players.Length; player++)
            {
                if (players[player].GetComponent<PlayerBehaviours>().circleInCollision == gameObject)
                {
                    Destroy(players[player]);
                }
            }
        }
    }
}
