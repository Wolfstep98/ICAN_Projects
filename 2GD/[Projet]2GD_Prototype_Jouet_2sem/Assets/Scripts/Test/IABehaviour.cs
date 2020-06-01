using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IABehaviour : MonoBehaviour {

    [Header("Debug values")]
    public bool isMoving = false;
    public bool onCollision;
    public bool copyCreated;

    [Header("Game Design values")]
    public bool colorOnCollision = false;


    //public float playerSpeed = 10f;
    [Range(0, 2000)]
    public float maxSpeed = 100f;
    public float timeOnCircle = 0f;

    [Header("Trajectory")]
    public Vector2 playerTrajectory = new Vector2(-0.5f, -0.5f);
    public Vector2 velocity;

    public GameObject trailHandler;

    [Header("Other")]
    //Gestion walls
    public GameObject[] walls = new GameObject[4];
    //private Vector2 horizontalDist;
    //private Vector2 verticalDist;

    public Collider2D spawnedWall;

    private Rigidbody2D rigid;

    public Camera mainCam;

    public GameManager2 gameManagerScript;

    public enum PlayerState
    {
        start,
        isMoving,
        onCircle
    }

    public enum ShootType
    {
        perpendicular,
        right
    }
    [Header("Player States")]
    public PlayerState playerState = PlayerState.start;
    public ShootType shootType = ShootType.perpendicular;

    // Use this for initialization
    void Start()
    {
        copyCreated = false;
        rigid = GetComponent<Rigidbody2D>();
        playerTrajectory = new Vector2(Random.Range(-1f,1f), Random.Range(-1f, 1f));
        onCollision = false;
        velocity = Vector2.zero;
        gameManagerScript = GameObject.Find("GameManager").GetComponent<GameManager2>();
        mainCam = Camera.main;
        //horizontalDist = new Vector2(walls[0].transform.position.x - walls[1].transform.position.x, 0);
        //verticalDist = new Vector2(0, walls[2].transform.position.y - walls[3].transform.position.y);
        isMoving = true;
    }

    // Update is called once per frame
    void Update()
    {
        velocity = rigid.velocity;
        if (isMoving)
        {
            rigid.velocity = new Vector2(playerTrajectory.x * maxSpeed * Time.deltaTime, playerTrajectory.y * maxSpeed * Time.deltaTime);
            rigid.velocity = Vector2.ClampMagnitude(rigid.velocity, maxSpeed);
        }
        if(onCollision)
        {
            timeOnCircle += Time.deltaTime;
        }

        if (timeOnCircle % 2 <= 0.01  && timeOnCircle != 0)
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Mur")
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
        else if (collision.gameObject.tag == "Circle")
        {
            if (colorOnCollision)
                collision.gameObject.GetComponent<Renderer>().material.color = new Color(Random.value, Random.value, Random.value, 1f);
            onCollision = true;
            rigid.velocity = Vector2.zero;
            transform.SetParent(collision.transform);
            isMoving = false;
            timeOnCircle += Time.deltaTime;
        }
    }
}
