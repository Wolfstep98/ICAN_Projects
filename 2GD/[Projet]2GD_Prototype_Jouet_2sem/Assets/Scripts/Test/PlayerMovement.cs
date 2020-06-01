using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    [Header("Debug values")]
    public bool isMoving = false;
    public bool onCollision;
    public bool copyCreated;

    [Header("Game Design values")]
    public bool colorOnCollision = false;
    public bool interactible = true;


    //public float playerSpeed = 10f;
    [Range(0,2000)]
    public float maxSpeed = 100f;

    [Header("Trajectory")]
    public Vector2 playerTrajectory = new Vector2(-0.5f, -0.5f);
    public Vector2 velocity;

    public GameObject trailHandler;

    [Header("Other")]

    public GameObject circleInCollision;

    public Collider2D spawnedWall;

    private Rigidbody2D rigid;

    public Camera mainCam;

    public GameManager2 gameManagerScript;

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

    // Use this for initialization
    void Start () {
        copyCreated = false;
        rigid = GetComponent<Rigidbody2D>();
        playerTrajectory = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        playerTrajectory.Normalize();
        onCollision = false;
        velocity = Vector2.zero;
        gameManagerScript = GameObject.Find("GameManager").GetComponent<GameManager2>();
        mainCam = Camera.main;
    }
	
	// Update is called once per frame
	void Update () {
        velocity = rigid.velocity;
        if (isMoving)
        {
            rigid.velocity = new Vector2(playerTrajectory.x * maxSpeed * Time.deltaTime, playerTrajectory.y * maxSpeed * Time.deltaTime);
            //rigid.AddForce(playerTrajectory * playerSpeed * Time.deltaTime);
            rigid.velocity = Vector2.ClampMagnitude(rigid.velocity, maxSpeed);
        }

        if(Input.GetKeyDown(KeyCode.Space) && interactible && player == Player.J1)
        {
            if (onCollision)
            {
                circleInCollision = null;
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
                else if(shootType == ShootType.right)
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
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) && interactible && player == Player.J2)
        {
            if (onCollision)
            {
                circleInCollision = null;
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
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Mur")
        {
            if (gameManagerScript.wallType == GameManager2.WallType.bounce)
            {
                if (collision.gameObject.name == "BordHaut" || collision.gameObject.name == "BordBas")
                {
                    playerTrajectory.y *= -1;
                }
                else if (collision.gameObject.name == "BordGauche" || collision.gameObject.name == "BordDroit")
                {
                    playerTrajectory.x *= -1;
                }
            }
        }
        else if(collision.gameObject.tag == "Circle")
        {
            circleInCollision = collision.gameObject;
            if (colorOnCollision)
                collision.gameObject.GetComponent<Renderer>().material.color = new Color(Random.value, Random.value, Random.value, 1f);
            onCollision = true;
            rigid.velocity = Vector2.zero;
            transform.SetParent(collision.transform);
            isMoving = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Mur")
        {
            if (gameManagerScript.wallType == GameManager2.WallType.wrap && (playerState == State.alive || playerState == State.spawned) && !copyCreated && (collision != spawnedWall ||spawnedWall == default(Collider2D)))
            {
                 playerState = State.outOfBound;
                if (collision.gameObject.name == "BordHaut")
                {
                    GameObject playerCopy = Instantiate<GameObject>(gameObject, new Vector2(transform.position.x, transform.position.y - gameManagerScript.distanceUpDown), Quaternion.identity);
                    playerCopy.name = gameObject.name;
                    playerCopy.GetComponent<Rigidbody2D>().velocity = rigid.velocity;
                    playerCopy.GetComponent<PlayerMovement>().playerTrajectory = playerTrajectory;
                    playerCopy.GetComponent<PlayerMovement>().playerState = State.spawned;
                    playerCopy.GetComponent<PlayerMovement>().spawnedWall = GameObject.Find("BordBas").GetComponent<Collider2D>();
                    copyCreated = true;
                }
                else if (collision.gameObject.name == "BordBas")
                {
                    GameObject playerCopy = Instantiate<GameObject>(gameObject, new Vector2(transform.position.x, transform.position.y + gameManagerScript.distanceUpDown), Quaternion.identity);
                    playerCopy.name = gameObject.name;
                    playerCopy.GetComponent<Rigidbody2D>().velocity = rigid.velocity;
                    playerCopy.GetComponent<PlayerMovement>().playerTrajectory = playerTrajectory;
                    playerCopy.GetComponent<PlayerMovement>().playerState = State.spawned;
                    playerCopy.GetComponent<PlayerMovement>().spawnedWall = GameObject.Find("BordHaut").GetComponent<Collider2D>();
                    copyCreated = true;
                }
                else if (collision.gameObject.name == "BordGauche")
                {
                    GameObject playerCopy = Instantiate<GameObject>(gameObject, new Vector2(transform.position.x + gameManagerScript.distanceRightLeft, transform.position.y), Quaternion.identity);
                    playerCopy.name = gameObject.name;
                    playerCopy.GetComponent<Rigidbody2D>().velocity = rigid.velocity;
                    playerCopy.GetComponent<PlayerMovement>().playerTrajectory = playerTrajectory;
                    playerCopy.GetComponent<PlayerMovement>().playerState = State.spawned;
                    playerCopy.GetComponent<PlayerMovement>().spawnedWall = GameObject.Find("BordDroit").GetComponent<Collider2D>();
                    copyCreated = true;
                }
                else if (collision.gameObject.name == "BordDroit")
                {
                    GameObject playerCopy = Instantiate<GameObject>(gameObject, new Vector2(transform.position.x - gameManagerScript.distanceRightLeft, transform.position.y), Quaternion.identity);
                    playerCopy.name = gameObject.name;
                    playerCopy.GetComponent<Rigidbody2D>().velocity = rigid.velocity;
                    playerCopy.GetComponent<PlayerMovement>().playerTrajectory = playerTrajectory;
                    playerCopy.GetComponent<PlayerMovement>().playerState = State.spawned;
                    playerCopy.GetComponent<PlayerMovement>().spawnedWall = GameObject.Find("BordGauche").GetComponent<Collider2D>();
                    copyCreated = true;
                }
            }
        }
        if(collision.name != player.ToString())
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(gameManagerScript.wallType == GameManager2.WallType.wrap)
        {
            if(playerState == State.spawned)
            {
                playerState = State.alive;
            }
            else if(playerState == State.outOfBound || playerState == State.alive)
            {
                Destroy(gameObject);
            }
            if(collision == spawnedWall)
            {
                spawnedWall = default(Collider2D);
            }
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
    }

}
