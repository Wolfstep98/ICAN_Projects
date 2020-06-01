using System;
using UnityEngine;

public class CustomCharacterController : MonoBehaviour
{
    #region Fields & Properties
    [Header("Movement")]
    [SerializeField]
    private bool isMoving = false;
    public bool IsMoving { get { return this.isMoving; } set { this.isMoving = value; } }
    [SerializeField]
    private float moveSpeed = 10.0f;
    [SerializeField]
    private Vector3 moveDirection = Vector3.zero;

    [Header("Rotation")]
    [SerializeField]
    private Vector3 direction = Vector3.zero;

    [Header("Gravity")]
    [SerializeField]
    private bool isGrounded = false;
    [SerializeField]
    private float gravityApplied = 0.0f;
    [SerializeField]
    private float gravityForce = 9.81f;
    [SerializeField]
    private float gravityModifier = 1.0f;
    [SerializeField]
    private float gravityMaxSpeed = 10.0f;
    [SerializeField]
    private float sphereGroundDetector = 0.25f;
    [SerializeField]
    private float groundTolerance = 0.15f;

    [SerializeField]
    private Vector3 gravity = Vector3.zero;

    [Header("Jump")]
    [SerializeField]
    private bool isJumping = false;
    public bool IsJumping { get { return this.isJumping; } }
    [SerializeField]
    private float jumpHeight = 5.0f;
    [SerializeField]
    private float jumpTimeToReachMax = 0.5f;
    [SerializeField]
    private float jumpTimer = 0.0f;
    [SerializeField]
    private AnimationCurve jumpCurve = null;

    [Header("References")]
    [SerializeField]
    private Transform mainCamera = null;
    [SerializeField]
    private CharacterController characterController = null;

    [Header("Debug")]
    [SerializeField]
    private bool showDirection = true;
    [SerializeField]
    private bool showRotation = true;
    [SerializeField]
    private bool showGravity = true;

    [SerializeField]
    [Range(1.0f,10.0f)]
    private float rayLenght = 2.0f;
    #endregion

    #region Methods
    private void Awake()
    {
        this.moveDirection = this.transform.forward;
        this.direction = this.transform.forward;
    }

    public void CustomUpdate()
    {
        this.DetectGround();
        if (!this.isGrounded)
            this.Fall();

        if (this.isMoving)
            this.isMoving = false;
        else if (!this.isMoving)
        {
            this.moveDirection.x = 0.0f;
            this.moveDirection.z = 0.0f;
        }

        this.characterController.Move(this.moveDirection);

        //Debug
        if (this.showDirection)
            Debug.DrawRay(this.transform.position, this.moveDirection.normalized * this.rayLenght, Color.red);
        if (this.showRotation)
            Debug.DrawRay(this.transform.position + Vector3.up, this.direction.normalized * this.rayLenght, Color.black);
        if (this.showGravity)
            Debug.DrawRay(this.transform.position, Vector3.up * this.characterController.velocity.y, Color.blue);
    }

    private void UpdateGravity()
    {
        if(!this.isGrounded && !this.isJumping)
        {
            this.gravity = new Vector3(0, this.gravity.y - this.gravityModifier * this.gravityForce, 0);
        }
        else if(this.isJumping)
        {
            this.jumpTimer += Time.deltaTime / this.jumpTimeToReachMax;
            if(this.jumpTimer < 1.0f)
            {
                //jumpCurve.Evaluate(this.jumpTimer + Time.deltaTime / this.jumpTimeToReachMax) - this.jumpCurve.Evaluate(this.jumpTimer);
            }
            else
            {

            }
        }
        else
        {

        }
    }

    /// <summary>
    /// Move the player in the <paramref name="direction"/>. 
    /// </summary>
    /// <param name="direction">The direction where you want to move.</param>
    public void Move(Vector3 direction)
    {
        Quaternion angle = this.mainCamera.rotation; //Stocke la rotation de la camera

        this.mainCamera.rotation = Quaternion.Euler(0, this.mainCamera.rotation.eulerAngles.y, 0); //On met la camera à plat

        this.moveDirection = this.mainCamera.TransformDirection(direction).normalized * this.moveSpeed * Time.deltaTime; //On change le référentiel

        this.mainCamera.rotation = angle; //On la remet dans l'angle de base

        this.isMoving = true;
    }

    /// <summary>
    /// Rotate the player to face the <paramref name="direction"/>.
    /// </summary>
    /// <param name="direction">The direction to look at.</param>
    public void Rotation(Vector3 direction)
    {
        Quaternion angle = this.mainCamera.rotation;

        this.mainCamera.rotation = Quaternion.Euler(0, this.mainCamera.rotation.eulerAngles.y, 0);

        this.direction = this.mainCamera.TransformDirection(direction);

        this.transform.forward = this.direction;

        this.mainCamera.rotation = angle;
    }

    public void DetectGround()
    {
        //Cast sphere to detect ground
        RaycastHit infos;
        if(Physics.SphereCast(this.transform.position, 0.25f, Vector3.down, out infos, 1.0f, 255))
        {
            if(infos.normal.y == 1)
            {
                this.moveDirection.y = 0.0f;
                this.gravityApplied = 0.0f;
                this.isGrounded = true;
            }
        }
        else
        {
            this.isGrounded = false;
        }
    }

    public void Fall()
    {
        this.gravityApplied += this.gravityForce * Time.deltaTime;
        this.gravityApplied = Mathf.Clamp(this.gravityApplied, this.gravityMaxSpeed, 0.0f);
        this.moveDirection.y = this.gravityApplied;
    }

    public void Jump()
    {
        if(this.isGrounded)
        {
            this.isJumping = true;
            this.jumpTimer = 0.0f;
        }
    }

    /// <summary>
    /// Change the camera transform.
    /// </summary>
    /// <param name="newMainCamPos">The new MainCamera transform position.</param>
    public void ChangeMainCamera(Transform newMainCamPos)
    {
        this.mainCamera = newMainCamPos;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(new Vector3(this.transform.position.x, this.transform.position.y - this.characterController.bounds.extents.y + this.characterController.radius + -this.sphereGroundDetector, this.transform.position.z),0.25f);
    }
    #endregion
}
