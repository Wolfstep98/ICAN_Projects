using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerBehaviours : MonoBehaviour {

    [Header("Ball properties")]
    public bool isIA = false;
    public bool isWaiting = true;
    public bool isMulti = false;
    public bool isLien = false;

    [Header("Game Design values")]
    public bool colorOnCollision = false;
    public bool interactible = true;

    //public float playerSpeed = 10f;
    [Range(0, 2000)]
    public float maxSpeed = 100f;

    [Header("Multi")]
    public float timeBeforeDie;
    
    public GameObject circleCollider;

    TrailRenderer parentTrail;

    [Header("Trajectory")]
    public Vector2 playerTrajectory = new Vector2(-0.5f, -0.5f);
    public Vector2 velocity;

    public GameObject trailHandler;

    [Header("Aura")]
    public bool auraStoled;
    public int points;
    public GameObject aura;

    [Header("Lien")]
    public Color playerColor;

    public GameObject lienInCollision;

    [Header("Power")]
    public GameObject powerObj;
    private GameObject instancePowerObj;

    public float distanceMin = 0.3f;
    public float powerTime = 2f;

    [Header("Debug values")]
    public bool isMoving = false;
    public bool onCollision;
    public bool copyCreated;
    public float timeOnCircle = 0f;
    public float topValue = 5.5f;
    public float leftValue = -11f;
    public float rightValue = 6.7f;
    public float bottomValue = -4.9f;

    [Header("Other")]

    public GameObject circleInCollision;

    public Collider2D spawnedWall;

    private Rigidbody2D rigid;

    public Camera mainCam;

    public GameManager2 gameManagerScript;

    public GameObject lightChild;

    public Text scoreText;
    [SerializeField]
    public Coroutine pointsPerSec;

    public enum State
    {
        spawned,
        alive,
        outOfBound
    }
    [Header("Player States")]
    public State playerState = State.alive;

    public enum ShootType
    {
        perpendicular,
        right
    }
    public ShootType shootType = ShootType.perpendicular;

    public enum Player
    {
        J1,
        J2,
        J3,
        J4
    }
    public Player player = Player.J1;

    public void Shoot(ShootType type)
    {    
        if (shootType == ShootType.perpendicular)
        {
            Vector2 vecPlayerCircle = transform.position - transform.parent.position;
            if (transform.parent.GetComponent<CircleBehaviours>().rotateRight)
            {
                playerTrajectory = new Vector2(-vecPlayerCircle.y, vecPlayerCircle.x);
            }
            else if (!transform.parent.GetComponent<CircleBehaviours>().rotateRight)
            {
                playerTrajectory = new Vector2(vecPlayerCircle.y, -vecPlayerCircle.x);
            }
            playerTrajectory.Normalize();
            rigid.velocity = new Vector2(playerTrajectory.x * maxSpeed * Time.deltaTime, playerTrajectory.y * maxSpeed * Time.deltaTime);
            transform.parent = null;
            onCollision = false;
            isMoving = true;
        }
        else if (shootType == ShootType.right)
        {
            Vector2 vecPlayerCircle = transform.position - transform.parent.position;
            playerTrajectory = vecPlayerCircle;
            playerTrajectory.Normalize();
            rigid.velocity = new Vector2(playerTrajectory.x * maxSpeed * Time.deltaTime, playerTrajectory.y * maxSpeed * Time.deltaTime);
            transform.parent = null;
            onCollision = false;
            isMoving = true;
        }
        circleInCollision = null;
    }

    IEnumerator SpawnCollider()
    {
        while (true)
        {
            GameObject circle = Instantiate<GameObject>(circleCollider, transform.position, transform.rotation);
            circle.name = player.ToString();
            circle.tag = "CircleCollider";
            Destroy(circle, timeBeforeDie);
            yield return new WaitForSeconds(0.02f);
        }
    }

    // Use this for initialization
    void Start()
    {
        isWaiting = true;
        copyCreated = false;
        auraStoled = false;
        points = 0;
        rigid = GetComponent<Rigidbody2D>();
        playerTrajectory = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        playerTrajectory.Normalize();
        onCollision = false;
        velocity = Vector2.zero;
        gameManagerScript = GameObject.Find("GameManager").GetComponent<GameManager2>();
        mainCam = Camera.main;
        if(gameManagerScript.gameType == GameManager2.GameType.lighting)
        {
            lightChild = transform.GetChild(0).gameObject;
            lightChild.SetActive(true);
        }
        if(isMulti)
            StartCoroutine(SpawnCollider());
        parentTrail = transform.GetComponentInParent<TrailRenderer>();
        timeBeforeDie = parentTrail.time;
        if (gameManagerScript.gameType == GameManager2.GameType.recoltables)
        {
            scoreText = GameObject.Find("Score" + player.ToString()).GetComponent<Text>();
            scoreText.text = player.ToString() + " : " + points;
        }
        playerColor = GetComponent<Renderer>().material.color;
        powerObj = Resources.Load<GameObject>("Prefabs/Final/SphereSave");

    }

    // Update is called once per frame
    void Update()
    {
        if (gameManagerScript.gameType == GameManager2.GameType.recoltables)
        {
            scoreText.text = player.ToString() + " : " + points;
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            GetComponent<TrailRenderer>().enabled = (GetComponent<TrailRenderer>().enabled) ? false : true;
            GetComponent<TrailRenderer>().Clear();
        }
        velocity = rigid.velocity;
        if (isMoving)
        {
            rigid.velocity = new Vector2(playerTrajectory.x * maxSpeed * Time.deltaTime, playerTrajectory.y * maxSpeed * Time.deltaTime);
            //rigid.AddForce(playerTrajectory * playerSpeed * Time.deltaTime);
            rigid.velocity = Vector2.ClampMagnitude(rigid.velocity, maxSpeed);
        }
        if (!isIA)
        {
            if (Input.GetKeyDown(KeyCode.Space) && interactible && player == Player.J1)
            {
                if (onCollision)
                {
                    if (circleInCollision.GetComponent<CircleBehaviours>() != null)
                    {
                        if (circleInCollision.GetComponent<CircleBehaviours>().isTiming)
                        {
                            if (circleInCollision.GetComponent<CircleBehaviours>().isOnColor)
                            {
                                Shoot(shootType);
                            }
                        }
                        else
                        {
                            Shoot(shootType);
                        }
                    }
                    else
                    {
                        Shoot(shootType);
                    }
                }
                else
                {
                    if(isWaiting)
                    {
                        playerTrajectory = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
                        isMoving = true;
                        isWaiting = false;
                    }
                    else
                    {
                        bool canCreateCircle = true;
                        foreach(GameObject circle in gameManagerScript.circles)
                        {
                            if (Vector2.Distance(transform.position, circle.transform.position) < distanceMin)
                            {
                                canCreateCircle = false;
                                break;
                            }
                        }
                        if (Vector2.Distance(transform.position, new Vector2(transform.position.x, topValue)) < distanceMin
                            || Vector2.Distance(transform.position, new Vector2(transform.position.x, bottomValue)) < distanceMin
                            || Vector2.Distance(transform.position, new Vector2(leftValue, transform.position.y)) < distanceMin
                            || Vector2.Distance(transform.position, new Vector2(rightValue, transform.position.y)) < distanceMin)
                            canCreateCircle = false;
                        if(canCreateCircle && instancePowerObj == null)
                        {
                            Debug.Log("Create power");
                            instancePowerObj = Instantiate<GameObject>(powerObj, (Vector2)transform.position + playerTrajectory * 0.35f, Quaternion.identity);
                            instancePowerObj.GetComponent<Renderer>().material.color = playerColor;
                            Destroy(instancePowerObj, powerTime);
                        }
                    }

                }
                
            }
            if (Input.GetKeyDown(KeyCode.DownArrow) && interactible && player == Player.J2)
            {
                if (onCollision)
                {
                    if (circleInCollision.GetComponent<CircleBehaviours>() != null)
                    {
                        if (circleInCollision.GetComponent<CircleBehaviours>().isTiming)
                        {
                            if (circleInCollision.GetComponent<CircleBehaviours>().isOnColor)
                            {
                                Shoot(shootType);
                            }
                        }
                        else
                        {
                            Shoot(shootType);
                        }
                    }
                    else
                    {
                        Shoot(shootType);
                    }
                }
                else
                {
                    if (isWaiting)
                    {
                        playerTrajectory = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
                        isMoving = true;
                        isWaiting = false;
                    }
                    else
                    {
                        bool canCreateCircle = true;
                        foreach (GameObject circle in gameManagerScript.circles)
                        {
                            if (Vector2.Distance(transform.position, circle.transform.position) < distanceMin)
                            {
                                canCreateCircle = false;
                                break;
                            }
                        }
                        if (Vector2.Distance(transform.position, new Vector2(transform.position.x, topValue)) < distanceMin
                            || Vector2.Distance(transform.position, new Vector2(transform.position.x, bottomValue)) < distanceMin
                            || Vector2.Distance(transform.position, new Vector2(leftValue, transform.position.y)) < distanceMin
                            || Vector2.Distance(transform.position, new Vector2(rightValue, transform.position.y)) < distanceMin)
                            canCreateCircle = false;
                        if (canCreateCircle && instancePowerObj == null)
                        {
                            Debug.Log("Create power");
                            instancePowerObj = Instantiate<GameObject>(powerObj, (Vector2)transform.position + playerTrajectory * 0.35f, Quaternion.identity);
                            instancePowerObj.GetComponent<Renderer>().material.color = playerColor;
                            Destroy(instancePowerObj, powerTime);
                        }
                    }
                }
            }
        }
        else
        {
            if (onCollision)
            {
                timeOnCircle += Time.deltaTime;
            }

            if (timeOnCircle % 2 <= 0.01 && timeOnCircle != 0)
            {
                float randNumber = Random.value;
                Debug.Log("Check");
                if (randNumber <= 0.5f)
                {
                    if (onCollision)
                    {
                        if (shootType == ShootType.perpendicular)
                        {
                            Vector2 vecPlayerCircle = transform.position - transform.parent.position;
                            if (transform.parent.GetComponent<CircleBehaviour>().rotateRight)
                            {
                                playerTrajectory = new Vector2(-vecPlayerCircle.y, vecPlayerCircle.x);
                            }
                            else if (!transform.parent.GetComponent<CircleBehaviour>().rotateRight)
                            {
                                playerTrajectory = new Vector2(vecPlayerCircle.y, -vecPlayerCircle.x);
                            }
                            playerTrajectory.Normalize();
                            rigid.velocity = new Vector2(playerTrajectory.x * maxSpeed * Time.deltaTime, playerTrajectory.y * maxSpeed * Time.deltaTime);
                            transform.parent = null;
                            onCollision = false;
                        }
                        else if (shootType == ShootType.right)
                        {
                            Vector2 vecPlayerCircle = transform.position - transform.parent.position;
                            playerTrajectory = vecPlayerCircle;
                            playerTrajectory.Normalize();
                            rigid.velocity = new Vector2(playerTrajectory.x * maxSpeed * Time.deltaTime, playerTrajectory.y * maxSpeed * Time.deltaTime);
                            transform.parent = null;
                            onCollision = false;
                        }
                    }
                    isMoving = true;
                    timeOnCircle = 0f;
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Mur")
        {
            if (gameManagerScript.wallType == GameManager2.WallType.bounce)
            {
                if (collision.gameObject.name == "MurHaut" || collision.gameObject.name == "MurBas")
                {
                    playerTrajectory.y *= -1;
                    playerTrajectory.y += Random.Range(-0.05f, 0.05f);
                }
                else if (collision.gameObject.name == "MurGauche" || collision.gameObject.name == "MurDroit")
                {
                    playerTrajectory.x *= -1;
                    playerTrajectory.x += Random.Range(-0.05f, 0.05f);
                }
            }
        }
        else if (collision.gameObject.tag == "Circle")
        {
            circleInCollision = collision.gameObject;
            if (colorOnCollision)
                collision.gameObject.GetComponent<Renderer>().material.color = GetComponent<Renderer>().material.color;
            onCollision = true;
            if(gameManagerScript.gameType != GameManager2.GameType.rotateCustom)
                rigid.velocity = Vector2.zero;
            transform.SetParent(collision.transform);
            isMoving = false;
            rigid.velocity = Vector2.zero;
            if(isIA)
                timeOnCircle += Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Mur")
        {
            if (gameManagerScript.wallType == GameManager2.WallType.wrap && (playerState == State.alive || playerState == State.spawned) && !copyCreated && (collision != spawnedWall || spawnedWall == default(Collider2D)))
            {
                playerState = State.outOfBound;
                if (collision.gameObject.name == "BordHaut")
                {
                    GameObject playerCopy = Instantiate<GameObject>(gameObject, new Vector2(transform.position.x, transform.position.y - gameManagerScript.distanceUpDown), Quaternion.identity);
                    playerCopy.name = gameObject.name;
                    playerCopy.GetComponent<Rigidbody2D>().velocity = rigid.velocity;
                    playerCopy.GetComponent<PlayerBehaviours>().playerTrajectory = playerTrajectory;
                    playerCopy.GetComponent<PlayerBehaviours>().playerState = State.spawned;
                    playerCopy.GetComponent<PlayerBehaviours>().spawnedWall = GameObject.Find("BordBas").GetComponent<Collider2D>();
                    copyCreated = true;
                }
                else if (collision.gameObject.name == "BordBas")
                {
                    GameObject playerCopy = Instantiate<GameObject>(gameObject, new Vector2(transform.position.x, transform.position.y + gameManagerScript.distanceUpDown), Quaternion.identity);
                    playerCopy.name = gameObject.name;
                    playerCopy.GetComponent<Rigidbody2D>().velocity = rigid.velocity;
                    playerCopy.GetComponent<PlayerBehaviours>().playerTrajectory = playerTrajectory;
                    playerCopy.GetComponent<PlayerBehaviours>().playerState = State.spawned;
                    playerCopy.GetComponent<PlayerBehaviours>().spawnedWall = GameObject.Find("BordHaut").GetComponent<Collider2D>();
                    copyCreated = true;
                }
                else if (collision.gameObject.name == "BordGauche")
                {
                    GameObject playerCopy = Instantiate<GameObject>(gameObject, new Vector2(transform.position.x + gameManagerScript.distanceRightLeft, transform.position.y), Quaternion.identity);
                    playerCopy.name = gameObject.name;
                    playerCopy.GetComponent<Rigidbody2D>().velocity = rigid.velocity;
                    playerCopy.GetComponent<PlayerBehaviours>().playerTrajectory = playerTrajectory;
                    playerCopy.GetComponent<PlayerBehaviours>().playerState = State.spawned;
                    playerCopy.GetComponent<PlayerBehaviours>().spawnedWall = GameObject.Find("BordDroit").GetComponent<Collider2D>();
                    copyCreated = true;
                }
                else if (collision.gameObject.name == "BordDroit")
                {
                    GameObject playerCopy = Instantiate<GameObject>(gameObject, new Vector2(transform.position.x - gameManagerScript.distanceRightLeft, transform.position.y), Quaternion.identity);
                    playerCopy.name = gameObject.name;
                    playerCopy.GetComponent<Rigidbody2D>().velocity = rigid.velocity;
                    playerCopy.GetComponent<PlayerBehaviours>().playerTrajectory = playerTrajectory;
                    playerCopy.GetComponent<PlayerBehaviours>().playerState = State.spawned;
                    playerCopy.GetComponent<PlayerBehaviours>().spawnedWall = GameObject.Find("BordGauche").GetComponent<Collider2D>();
                    copyCreated = true;
                }
            }
        }
        else if (collision.gameObject.tag == "Recoltable")
        {
            if (collision.transform.parent.name == "Aura_White")
            {
                GameObject recoltable = collision.transform.root.gameObject;
                Destroy(recoltable);
                float rdmNbr = Random.value;
                Debug.Log(rdmNbr);
                aura = (GameObject)Instantiate(Resources.Load("Prefabs/Test/Auras/Aura_" + ((rdmNbr >= 0.5f) ? "Green" : "Red")), transform.position, Quaternion.identity, transform);
                aura.name = "Aura_" + ((rdmNbr >= 0.5f) ? "Green" : "Red");
                aura.transform.localScale = new Vector3(1, 1, 1);
                aura.transform.GetChild(0).transform.localScale = new Vector3(4f, 4f, 4f);
                gameManagerScript.AuraTimer();
                pointsPerSec = StartCoroutine(onePointPerSec());
            }
            else if ((collision.transform.parent.name == "Aura_Green" || collision.transform.parent.name == "Aura_Red") && !auraStoled)
            {
                if (aura != null)
                {
                    PlayerBehaviours playerScript = collision.transform.GetComponentInParent<PlayerBehaviours>();
                    playerScript.aura = aura;
                    playerScript.aura.transform.parent = playerScript.gameObject.transform;
                    playerScript.aura.transform.position = playerScript.gameObject.transform.position;
                    playerScript.aura.transform.localScale = new Vector3(1, 1, 1);
                    playerScript.aura.transform.GetChild(0).transform.localScale = new Vector3(4f, 4f, 4f);
                    playerScript.aura.transform.GetChild(0).transform.position = playerScript.gameObject.transform.position;
                    playerScript._StopCoroutine();
                    playerScript.pointsPerSec = null;
                    pointsPerSec = StartCoroutine(onePointPerSec());
                    aura = null;
                    playerScript.auraStoled = true;
                }
                else
                {
                    PlayerBehaviours playerScript = collision.transform.GetComponentInParent<PlayerBehaviours>();
                    aura = playerScript.aura;
                    aura.transform.parent = transform;
                    aura.transform.position = transform.position;
                    aura.transform.localScale = new Vector3(1, 1, 1);
                    aura.transform.GetChild(0).transform.localScale = new Vector3(4f, 4f, 4f);
                    aura.transform.GetChild(0).transform.position = transform.position;
                    _StopCoroutine();
                    pointsPerSec = null;
                    playerScript.pointsPerSec = StartCoroutine(playerScript.onePointPerSec());
                    playerScript.aura = null;
                    playerScript.auraStoled = true;
                }
                auraStoled = true;
            }
            else if ((collision.transform.parent.name == "Aura_Green" || collision.transform.parent.name == "Aura_Red") && auraStoled)
            {
                PlayerBehaviours playerScript = collision.transform.GetComponentInParent<PlayerBehaviours>();
                playerScript.auraStoled = false;
                auraStoled = false;
            }
        }
        else if (collision.tag == "Lien" && collision.name != "Lien" + player.ToString())
        {
            lienInCollision = collision.gameObject;
            gameManagerScript.OnPlayerDestroyed(this, lienInCollision.GetComponent<LienBehaviours>().playerLinked);
            Destroy(gameObject);
        }
        else if (collision.name != player.ToString() && isMulti) 
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (gameManagerScript.wallType == GameManager2.WallType.wrap)
        {
            if (playerState == State.spawned)
            {
                playerState = State.alive;
            }
            else if (playerState == State.outOfBound || playerState == State.alive)
            {
                Destroy(gameObject);
            }
            if (collision == spawnedWall)
            {
                spawnedWall = default(Collider2D);
            }
        }
        if (gameManagerScript.gameType == GameManager2.GameType.recoltables && collision.transform.GetComponentInParent<PlayerBehaviours>() != null)
        {
            PlayerBehaviours playerScript = collision.transform.GetComponentInParent<PlayerBehaviours>();
            playerScript.auraStoled = false;
            auraStoled = false;
        }
    }

    public void DestroyColliders(string nom)
    {
        GameObject[] colliders = GameObject.FindGameObjectsWithTag("CircleCollider");
        if (colliders.Length > 0)
        {
            int i = colliders.Length - 1;
            while (i != 0)
            {
                if (colliders[i].name == nom)
                {
                    Collider2D.Destroy(colliders[i].GetComponent<CircleCollider2D>());
                    Destroy(colliders[i]);
                }
                i--;
            }
        }
    }

    private void OnDestroy()
    {
        DestroyColliders(player.ToString());
        if (isLien)
        {
            GameObject[] liens = GameObject.FindGameObjectsWithTag("Lien");
            foreach (GameObject lien in liens)
            {
                if (lien.name == "Lien" + player.ToString())
                {
                    Destroy(lien);
                }
            }
        }
    }

    IEnumerator onePointPerSec()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            points++;
        }
    }

    public void _StopCoroutine(string name = "")
    {
        StopAllCoroutines();
    }

}
