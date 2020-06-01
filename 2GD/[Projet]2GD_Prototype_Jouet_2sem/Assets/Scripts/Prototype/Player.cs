using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour {

    #region Variables
    [Header("Player properties")]

	//Test
	public bool black = false;
    //Speed and trajectory
    public bool isWaiting;
    public bool isMoving;
    public float speed;
    public const float Speed = 500f;
    public string playerInput;
    public Vector2 playerTrajectory = new Vector2(-0.5f, -0.5f);
    public Vector2 velocity;

	public float vol;

    //Color
    public int nbrOfCircleColored;
    //public Color playerColor;
    public GameManager.GameColor playerColor;

    //Bonus
    //Current Bonus
    public Circle.BonusType currentBonus;

    //Shield
    public bool shieldOn;
    public bool shieldOnCooldown;
    public float shieldCooldown;
    [ReadOnly]
    public float shieldTimeLeft;

    //Boost
    public bool isBoosting;
    [Range(0f,1f)]
    public float boostValue;

    //Hack
    public bool isHacking;
    [Range(0f, 1f)]
    public float hackingChance;


    //Collisions
    public bool onCollision;
    public GameObject lienInCollision;
    public GameObject circleInCollision;

    //Rigidbody2D
    private Rigidbody2D rigid;

    //Animations Parameters
    public int respawnHash;

    //References
    public Player instance;
    public GameObject feedBackSpawn;
    public GameObject feedBackShield;
    public GameObject feedbackBoost;
    public GameObject feedbackHack;
    public Animator feedbackSpawnAnimator;
    public Animator animator;
    public GameManager gameManager;
    public LienUpdater lienUpdater;
    public Sprite playerSprite;

    public Dictionary<GameObject, bool> circlesWithSameColor;

    //Enums
    public enum PlayerNumber
    {
        None,
        J1,
        J2,
        J3,
        J4
    }
    public PlayerNumber player;

    [Header("FMOD properties")]

    [FMODUnity.EventRef]
    public string musique_state;

	[FMODUnity.EventRef]
	public string ball_state;

    [FMODUnity.EventRef]
    public string collision_state;

    [FMODUnity.EventRef]
    public string destroy_state;

    [FMODUnity.EventRef]
    public string shutdown_state;

    private FMOD.Studio.EventInstance musique_fmod;
	public FMOD.Studio.ParameterInstance melo1_fmod;
    public FMOD.Studio.ParameterInstance melo2_fmod;
    public FMOD.Studio.ParameterInstance melo3_fmod;
    public FMOD.Studio.ParameterInstance melo4_fmod;
	public FMOD.Studio.ParameterInstance black_fmod;
	private FMOD.Studio.EventInstance ball_fmod;
	public FMOD.Studio.ParameterInstance melo5_fmod;
    private FMOD.Studio.EventInstance collision_fmod;
    private FMOD.Studio.EventInstance destroy_fmod;
    //private FMOD.Studio.EventInstance hack_fmod;
    //private FMOD.Studio.EventInstance shutdown_fmod;

    #endregion

    private void Awake()
    {
        instance = this;
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        lienUpdater = GameObject.FindGameObjectWithTag("GameManager").GetComponent<LienUpdater>();
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        feedBackSpawn = transform.Find("Feedback_Spawn").gameObject;
        feedBackShield = transform.Find("Feedback_Shield").gameObject;
        feedbackBoost = transform.Find("Feedback_Boost").gameObject;
        feedbackHack = transform.Find("Feedback_Hack").gameObject;
        feedBackShield.SetActive(false);
        feedbackBoost.SetActive(false);
        feedbackHack.SetActive(false);

        feedbackSpawnAnimator = feedBackSpawn.GetComponent<Animator>();
        respawnHash = Animator.StringToHash("Respawn");
        //playerColor = GetComponent<MeshRenderer>().material.color;
    }

    // Use this for initialization
    void Start () {
		musique_fmod = FMODUnity.RuntimeManager.CreateInstance(musique_state);
		ball_fmod = FMODUnity.RuntimeManager.CreateInstance(ball_state);
		musique_fmod.getParameter ("black", out black_fmod);
		musique_fmod.getParameter ("melo1", out melo1_fmod);
		musique_fmod.getParameter ("melo2", out melo2_fmod);
		musique_fmod.getParameter ("melo3", out melo3_fmod);
		musique_fmod.getParameter ("melo4", out melo4_fmod);
		ball_fmod.getParameter ("melo5", out melo5_fmod);
		collision_fmod = FMODUnity.RuntimeManager.CreateInstance(collision_state);
		destroy_fmod = FMODUnity.RuntimeManager.CreateInstance(destroy_state);
		//hack_fmod = FMODUnity.RuntimeManager.CreateInstance(hack_state);
		//shutdown_fmod = FMODUnity.RuntimeManager.CreateInstance(shutdown_state);

		black_fmod.setValue (0f);
        melo1_fmod.setValue(0.5f);
        melo2_fmod.setValue(0.5f);
        melo3_fmod.setValue(0.5f);
        melo4_fmod.setValue( 0.5f);
        melo5_fmod.setValue(0f);
		musique_fmod.setVolume (1f);
		musique_fmod.start();
		ball_fmod.setVolume (1f);
		ball_fmod.start();
        isWaiting = false;
        isMoving = false;
        onCollision = false;
        shieldOn = false;
        //isContaminate = false;
        nbrOfCircleColored = 0;
        velocity = Vector2.zero;
        playerTrajectory = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        playerTrajectory.Normalize();
        circlesWithSameColor = new Dictionary<GameObject, bool>();
        foreach(GameObject circle in gameManager.circles)
        {
            circlesWithSameColor.Add(circle, false);
        }
        playerInput = "Input" + player.ToString();

        //Set the player color
        animator.SetInteger("Color", (int)playerColor);

        //Set the feedbacks color
        ParticleSystem.MainModule mainFeedbackBoost = feedbackBoost.GetComponent<ParticleSystem>().main;
        mainFeedbackBoost.startColor = gameManager.GameColorToColor(playerColor);
        ParticleSystem.MainModule mainFeedbackHack = feedbackHack.GetComponent<ParticleSystem>().main;
        mainFeedbackHack.startColor = gameManager.GameColorToColor(playerColor);
    }

    public void OnEnable()
    {
        //Set the player color
        animator.SetInteger("Color", (int)playerColor);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.mainGameManager.currentGameState != MainGameManager.GameState.IsPaused)
        {
            if (gameObject.activeSelf)
            {
                //Setting the speed
                //speed = Speed;
                //if (nbrOfCircleColored > 0)
                //    speed += (nbrOfCircleColored * lienUpdater.valueAmount * ((lienUpdater.bonus) ? 1 : -1));
                speed =
                    Speed //Initial speed
                    + (nbrOfCircleColored * lienUpdater.valueAmount * ((lienUpdater.bonus) ? 1 : -1)) //Player speed indexed on the number of circle colored
                    + ((isBoosting) ? Speed * boostValue : 0); //Speed increase with the bonus

                //Move the player
                if (isMoving)
                {
                    velocity = new Vector2(playerTrajectory.x * speed * Time.deltaTime, playerTrajectory.y * speed * Time.deltaTime);
                    //rigid.AddForce(playerTrajectory * playerSpeed * Time.deltaTime);
                    velocity = Vector2.ClampMagnitude(velocity, speed);
                    rigid.velocity = velocity;
                    Debug.DrawRay(transform.position, playerTrajectory, Color.white);
                }

                //Player Input
                if (Input.GetButtonDown(playerInput))
                {
                    if (onCollision)
                    {
                        Shoot();
                    }
                    else if (isWaiting)
                    {
                        playerTrajectory = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
                        isMoving = true;
                        isWaiting = false;
                    }
                }

                //Bonus Update
                if (shieldOnCooldown)
                {
                    shieldTimeLeft -= Time.deltaTime;
                    if (shieldTimeLeft <= 0f)
                    {
                        shieldTimeLeft = 0f;
                        shieldOnCooldown = false;
                        feedBackShield.SetActive(true);
                    }
                }

                if (onCollision)
                {
                    if (!circleInCollision.GetComponent<Circle>().isBonus)
                        circleInCollision.GetComponent<Circle>().animator.SetBool(circleInCollision.GetComponent<Circle>().playerCollisionAnimParam, true);
                }

            }
        }
    }
    public void Shoot()
    {
        Vector2 vecPlayerCircle = transform.position - transform.parent.position;
        playerTrajectory = vecPlayerCircle;
        playerTrajectory.Normalize();
        rigid.velocity = new Vector2(playerTrajectory.x * speed * Time.deltaTime, playerTrajectory.y * speed * Time.deltaTime);
        transform.parent = null;
        onCollision = false;
        if (!circleInCollision.GetComponent<Circle>().isBonus)
            circleInCollision.GetComponent<Circle>().animator.SetBool(circleInCollision.GetComponent<Circle>().playerCollisionAnimParam, false);
        isMoving = true;
        circleInCollision = null;
    }

    public void AddCircle(GameObject circle)
    {
        if (!circlesWithSameColor[circle])
        {
            circlesWithSameColor[circle] = true;
            //nbrOfCircleColored++;
        }
    }

    //Enumerates through the circles to check if some circles that had the same color changed color
    public void UpdateCircleColored()
    {
        Dictionary<GameObject, bool>.KeyCollection keysTemp = circlesWithSameColor.Keys;
        GameObject[] keys = new GameObject[circlesWithSameColor.Count];
        keysTemp.CopyTo(keys, 0);
        nbrOfCircleColored = 0;
        foreach (GameObject key in circlesWithSameColor.Keys)
        {
            if(circlesWithSameColor[key])
            {
                nbrOfCircleColored++;
            }
        }
        foreach (GameObject circle in keys)
        {
            if (!circlesWithSameColor[circle])
                continue;
            Circle script = circle.GetComponent<Circle>();
            if (script.color != playerColor)
            {
                circlesWithSameColor[circle] = false;
                nbrOfCircleColored--;
            }
        }
    }

    //---------- Feedbacks ----------
    public void PlayRespawnFeedbacks()
    {
        feedbackSpawnAnimator.SetTrigger(respawnHash);
    }


    //---------- Collisions ----------
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Circle")
        {
            collision_fmod.setVolume(0f);
            collision_fmod.start();
            isMoving = false;
            onCollision = true;
            rigid.velocity = Vector2.zero;
            circleInCollision = collision.gameObject;
            transform.SetParent(collision.transform);
            if (collision.gameObject.GetComponent<Circle>().isBonus)
            {
                Shoot();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Lien" && (collision.GetComponent<Lien>().playerLinked != player && collision.GetComponent<Lien>().color != playerColor))
        {
            if (shieldOn && !shieldOnCooldown)
            {
                shieldOnCooldown = true;
                shieldTimeLeft = shieldCooldown;
                feedBackShield.SetActive(false);
                Debug.Log("Shield destroyed / " + player.ToString());
            }
            else if(collision.GetComponent<Lien>().isActive)
            {
				if (player == PlayerNumber.J1) {
					melo1_fmod.setValue (0f);
				}
				if (player == PlayerNumber.J2) {
					melo2_fmod.setValue (0f);
				}
				if (player == PlayerNumber.J3) {
					melo3_fmod.setValue (0f);
				}
				if (player == PlayerNumber.J4) {
					melo4_fmod.setValue (0f);
				}
				destroy_fmod.setVolume (0.5f);
				destroy_fmod.start();
                lienInCollision = collision.gameObject;
                gameManager.OnPlayerDestroyed(instance, lienInCollision.GetComponent<Lien>().playerLinked);
            }
        }
    }

    //Bonus
    //Shield
    public void ActivateShield(float bonusTime)
    {
        shieldOn = true;
        shieldCooldown = bonusTime;
        currentBonus = Circle.BonusType.Shield;
        feedBackShield.SetActive(true);
    }

    public void UpdateShieldCooldown(float bonusTime)
    {
        shieldCooldown = bonusTime;
    }

    public void DeactivateShield()
    {
        shieldOn = false;
        shieldOnCooldown = false;
        shieldCooldown = 0f;
        shieldTimeLeft = 0f;
        currentBonus = Circle.BonusType.None;
        feedBackShield.SetActive(false);
    }

    //Hack
    public void ActivateHack(float hackChance)
    {
        isHacking = true;
        hackingChance = hackChance;
        currentBonus = Circle.BonusType.Hack;
        feedbackHack.SetActive(true);
    }

    public void UpdateHackChance(float hackChance)
    {
        hackingChance = hackChance;
    }

    public void DeactivateHack()
    {
        isHacking = false;
        hackingChance = 0f;
        currentBonus = Circle.BonusType.None;
        feedbackHack.SetActive(false);
    }

    //Boost
    public void ActivateBoost(float speedMultiplicator)
    {
        isBoosting = true;
        boostValue = speedMultiplicator;
        currentBonus = Circle.BonusType.Boost;
        feedbackBoost.SetActive(true);
    }

    public void UpdateBoost(float speedMultiplicator)
    {
        boostValue = speedMultiplicator;
    }

    public void DeactivateBoost()
    {
        isBoosting = false;
        boostValue = 0f;
        currentBonus = Circle.BonusType.None;
        feedbackBoost.SetActive(false);
    }



    // ---------- Reset ----------
    public void Reset()
    {
        isWaiting = true;
        isMoving = false;
        onCollision = false;
        //shieldOn = false;
        transform.parent = null;
        velocity = Vector2.zero;
        playerTrajectory = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        playerTrajectory.Normalize();
        //Set the player color
        animator.SetInteger("Color", (int)playerColor);
        UpdateCircleColored();
    }

    private void OnDestroy()
    {
        musique_fmod.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}
