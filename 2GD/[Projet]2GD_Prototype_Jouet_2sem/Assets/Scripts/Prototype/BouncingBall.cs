using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class BouncingBall : MonoBehaviour {

    [ReadOnly]
    public BouncingBall instance;

    #region Variables
    [Header("Ball properties")]
    public float bounceThreshold = 1f;
    public float speed = 400f;
    [ReadOnly]
    public Vector2 trajectory;
    [ReadOnly]
    public Vector2 velocity;

    [ReadOnly]
    public Player player;
    [ReadOnly]
    public GameManager.GameColor color;

    //References
    [ReadOnly]
    public Rigidbody2D rigid;
    [ReadOnly]
    public CircleCollider2D circleCollider2D;
    [ReadOnly]
    public CircleCollider2D hitBox;
    [ReadOnly]
    public Animator animator;
    [ReadOnly]
    public Animator feedbackSpawnAnimator;
    public int respawnHash;


    #endregion

    private void Awake()
    {
        instance = this;
        rigid = GetComponent<Rigidbody2D>();
        circleCollider2D = GetComponent<CircleCollider2D>();
        hitBox = GetComponentInChildren<CircleCollider2D>();
        animator = GetComponent<Animator>();
        feedbackSpawnAnimator = transform.Find("Feedback_Spawn").GetComponent<Animator>();
        respawnHash = Animator.StringToHash("Respawn");
    }

    // Use this for initialization
    void Start ()
    {
        color = GameManager.GameColor.White;
        animator.SetInteger("Color", (int)color);
        trajectory = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        trajectory.Normalize();
        player = null;
        PlayRespawnFeedbacks();
    }
	
	// Update is called once per frame
	void Update ()
    {
        velocity = new Vector2(trajectory.x * speed * Time.deltaTime, trajectory.y * speed * Time.deltaTime);
        velocity = Vector2.ClampMagnitude(velocity, speed);
        rigid.velocity = velocity;
        Debug.DrawRay(transform.position, trajectory, Color.white);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Mur" || collision.transform.tag == "Circle")
        {
            trajectory = collision.contacts[0].normal;
            trajectory.x += Random.Range(-bounceThreshold, bounceThreshold);
            trajectory.y += Random.Range(-bounceThreshold, bounceThreshold);
            trajectory.Normalize();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            player = collision.GetComponent<Player>();
            color = player.playerColor;
            animator.SetInteger("Color", (int)color);
        }
    }

    //---------- Feedbacks ----------
    public void PlayRespawnFeedbacks()
    {
        feedbackSpawnAnimator.SetTrigger(respawnHash);
    }
}
