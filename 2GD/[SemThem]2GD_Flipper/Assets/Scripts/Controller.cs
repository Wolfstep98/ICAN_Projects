using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {

    public bool isOnLeftWall;
    public bool isOnRightWall;
    public bool isOnGround;
    public bool doubleJump;
    public bool jumpFinished;
    public bool isMoving;

    public float gravity = -9.81f;
    public const float G = -9.81f;

    //-- Player speed movement --\\
    public float minSpeed = 0f;
    public float maxSpeed = 5f;
    public float zeroToMaxSpeedTime = 0.5f;
    public float maxToZeroSpeedTime = 0.5f;
    private float speedDecreaseT0;

    //-- Player jump movement --\\
    public float maxJumpHeight = 4f;
    private float modifiedGravity = 0f;
    public float maximumJumpTime = 1f;

    //-- Player air control --\\
    public float maxAirControlSpeed = 3f;
    public float zeroToMaxSpeedAirTime = 0.5f; 

    Rigidbody rigidbodyPlayer;

    public Vector3 velocity;
    public Vector3 jumpPosition;

    //-- Debug values --\\

    private void FixedUpdate()
    {
        isOnGround = false;
    }

    // Use this for initialization
    void Start ()
    {
        isMoving = false;
        isOnLeftWall = false;
        isOnRightWall = false;
        isOnGround = false;
        doubleJump = false;
        jumpFinished = false;
        velocity = Vector3.zero;
        rigidbodyPlayer = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update ()
    {
        //Movement key checking
        if (((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow)) || (Input.GetAxis("Horizontal") > 0.01 || Input.GetAxis("Horizontal") < -0.01)))
            isMoving = true;
        if(!isOnLeftWall && (Input.GetKey(KeyCode.LeftArrow) || Input.GetAxis("Horizontal") < -0.01))
        {             
            if (isOnGround)
                velocity.x = Mathf.MoveTowards(velocity.x, -maxSpeed, (maxSpeed / zeroToMaxSpeedTime) * Time.deltaTime);
            else if (!isOnGround)
            {
                if(velocity.x > -maxAirControlSpeed)
                    velocity.x = Mathf.MoveTowards(velocity.x, -maxAirControlSpeed, (maxAirControlSpeed / zeroToMaxSpeedAirTime) * Time.deltaTime);
            }
        }
        else if (!isOnRightWall && (Input.GetKey(KeyCode.RightArrow) || Input.GetAxis("Horizontal") > 0.01))
        {
            if (isOnGround)
                velocity.x = Mathf.MoveTowards(velocity.x, maxSpeed, (maxSpeed / zeroToMaxSpeedTime) * Time.deltaTime);
            else if (!isOnGround)
            {
                if(velocity.x < maxAirControlSpeed)
                    velocity.x = Mathf.MoveTowards(velocity.x, maxAirControlSpeed, (maxAirControlSpeed / zeroToMaxSpeedAirTime) * Time.deltaTime);
            }
        }
        if ((Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
            || (isMoving && (Input.GetAxis("Horizontal") < 0.01) && Input.GetAxis("Horizontal") > -0.01))
        {
            speedDecreaseT0 = Mathf.Abs(velocity.x);
            isMoving = false;
        }
        // Movement deceleration if the user is not pressing a movement key
        if (!isMoving && velocity.x != 0 && isOnGround)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, minSpeed, (speedDecreaseT0/maxToZeroSpeedTime) * Time.deltaTime);
        }
        else if(!isMoving && velocity.x == 0 && isOnGround)
        {
            speedDecreaseT0 = 0;
        }

        //Jump checking
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button1)) && isOnGround && !jumpFinished)
        {
            isOnGround = false;
            float longueur = (maximumJumpTime / 2);
            gravity = modifiedGravity = ((-2f * maxJumpHeight) / (Mathf.Pow(longueur, 2f)));
            velocity.y += ((2f * maxJumpHeight) / (longueur));          
        }
        if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.Joystick1Button1) && !isOnGround)
        {
            jumpFinished = true;
            if(velocity.y > 0)
                gravity *= 3f;
        }
        if (!isOnGround && jumpFinished && velocity.y <= 0)
            gravity = modifiedGravity;

        // Gravity applied
        if (!isOnGround && velocity.y > gravity)
        {
            velocity.y += gravity* Time.deltaTime;
        }	
        else if(isOnGround && velocity.y <= 0)
        {
            gravity = G;
            velocity.y = 0;
        }
        velocity.x = Mathf.Clamp(-maxSpeed, velocity.x, maxSpeed);
        rigidbodyPlayer.velocity = velocity;
    }

    void OnCollisionEnter(Collision collision)
    {
        int normal_y = Mathf.RoundToInt(collision.contacts[0].normal.y);
        int normal_x = Mathf.RoundToInt(collision.contacts[0].normal.x);
        if ((Mathf.Abs(normal_y) == 1 && collision.gameObject.tag == "Sol")) // plafond, sol
        {
            jumpFinished = true;
            isOnGround = true;
            speedDecreaseT0 = Mathf.Abs(velocity.x);
        }
        else if ((Mathf.Abs(normal_y) == 1 && (collision.gameObject.tag == "Plafond" || collision.gameObject.tag == "Platform"))) // plafond, sol
        {
            jumpFinished = true;
            velocity.y = 0;
        } 
        if (Mathf.Abs(normal_x) == 1 && collision.gameObject.tag == "Mur")
        {
            jumpFinished = true;
            velocity.x = 0;
            if (velocity.y > 0)
                velocity.y = 0;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        int normal_y = Mathf.RoundToInt(collision.contacts[0].normal.y);
        int normal_x = Mathf.RoundToInt(collision.contacts[0].normal.x);
        if (normal_y == 1) // sol
        {
            isOnGround = true;
            jumpFinished = false;
        }
        if (collision.gameObject.tag == "Mur")
        {
            if (normal_x == -1) // mur à ma droite
            {
                isOnRightWall = true;
            }
            if (normal_x == 1) // mur à ma gauche
            {
                isOnLeftWall = true;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    { 
        if (collision.gameObject.name == "Mur Droit" )
        {
            isOnRightWall = false;
        }
        if (collision.gameObject.name == "Mur Gauche")
        {
            isOnLeftWall = false;
        }
        if (collision.gameObject.tag == "Sol")
        {
            isOnGround = false;
        }
        if(collision.gameObject.tag == "Platform")
        {
            if(isOnGround)
                isOnGround = false;
        }
    }

    private bool GroundDetection()
    {
        Ray rayDown = new Ray(transform.position, Vector3.down);
        RaycastHit hitInfo = new RaycastHit();
        if(Physics.Raycast(rayDown, out hitInfo, 1.2f))
        {
            if (hitInfo.normal.y > 85 && hitInfo.normal.y < 95)
                return true;
        }
        return false;
    }

}
