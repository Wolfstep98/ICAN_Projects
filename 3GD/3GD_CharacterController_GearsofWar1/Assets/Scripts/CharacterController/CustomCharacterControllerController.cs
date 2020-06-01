using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CustomCharacterControllerController : MonoBehaviour
{
    #region Fields & Properties
    [Header("Parameters")]
    [Header("Movement")]
    [SerializeField]
    private bool isWalking = false;
    public bool IsWalking { get { return this.isWalking; } }
    [SerializeField]
    private MovementBehaviours currentMovementBehaviour = MovementBehaviours.Free;
    public MovementBehaviours CurrentMovementBehaviour { get { return this.currentMovementBehaviour; } }
    [SerializeField]
    private int currentMovementIndex = 0;

    [SerializeField]
    private MovementProperties[] movements = null;
    [SerializeField]
    private Dictionary<MovementBehaviours, int> movementBehaviourIndex = null;

    [SerializeField]
    private Vector3 direction = Vector3.zero;
    [SerializeField]
    private Vector3 finalDirection = Vector3.zero;

    [Header("Rotation")]
    [SerializeField]
    private bool enableRotation = true;
    [SerializeField]
    private bool inverseMouseXMovement = false;
    [SerializeField]
    private bool inverseMouseYMovement = false;
    public bool InverseMouseYMovement { get { return this.inverseMouseXMovement; } }

    //Sensitivity
    [SerializeField]
    private float mouseXSensitivity = 1.0f;
    [SerializeField]
    private float mouseYSensitivity = 1.0f;
    public float MouseYSensitivity { get { return this.mouseYSensitivity; } }

    [Header("Cover")]
    [SerializeField]
    private float timeToGetToCover = 0.5f;
    [SerializeField]
    private float timerToGetToCover = 0.0f;
    [SerializeField]
    private float maxCoverDistanceCheck = 2.0f;
    [SerializeField]
    private Vector3 coverPositionCheck = Vector3.zero;
    [SerializeField]
    private Vector3 intialPosition = Vector3.zero;
    [SerializeField]
    private Vector3 targetPosition = Vector3.zero;

    [Header("Animation")]
    [SerializeField]
    private Animator animator = null;

    [Header("References")]
    [SerializeField]
    private CharacterController characterController = null;
    [SerializeField]
    private CustomCameraBehaviourController cameraController = null;
    [SerializeField]
    private Camera mainCamera = null;
    [SerializeField]
    private Transform cameraRoot = null;

    [Header("Debug")]
    [SerializeField]
    private bool showDirection = true;
    [SerializeField]
    private bool showCoverCheck = true;
	#endregion
	
	#region Methods
	#region Intialization
	private void Awake()
	{
        this.Initialize();
	}
	
	private void Initialize()
	{
#if UNITY_EDITOR
        if (this.characterController == null)
            Debug.LogError("[Missing Reference] - characterController is missing ! ");
        if (this.cameraController == null)
            Debug.LogError("[Missing Reference] - cameraController is missing ! ");

        if (this.mainCamera == null)
            Debug.LogError("[Missing Reference] - mainCamera is missing ! ");
        if (this.cameraRoot == null)
            Debug.LogError("[Missing Reference] - cameraRoot is missing ! ");

        if (this.animator == null)
            Debug.LogError("[Missing Reference] - animator is missing ! ");

        if (this.movements.Length == 0)
            Debug.LogError("[Missing Reference] - movements are missing !");
#endif

        this.movementBehaviourIndex = new Dictionary<MovementBehaviours, int>(this.movements.Length);
        for (int i = 0; i < this.movements.Length; i++)
        {
            MovementBehaviours behaviour = this.movements[i].Behaviour;
            if (!this.movementBehaviourIndex.ContainsKey(behaviour))
            {
                this.movementBehaviourIndex.Add(behaviour, i);
            }
            else
            {
                Debug.LogError("[Movement behaviour duplicate] - More than 1 movement behaviour : " + behaviour);
            }
        }

        Debug.Log("[Initialization] - CustomCharacterControllerController correctly initialize !");
    }
#endregion

    public void CustomUpdate()
    {
        this.finalDirection = direction;
        if (this.currentMovementBehaviour == MovementBehaviours.Sprint)
            this.characterController.Move(this.transform.forward * this.movements[this.currentMovementIndex].Speed * Time.deltaTime);
        else if (this.currentMovementBehaviour == MovementBehaviours.Cover)
        {
            Vector3 currentPos = this.transform.position;
            currentPos.y = 0.0f;
            if (this.transform.position != this.targetPosition)
            {
                this.timerToGetToCover += Time.deltaTime / this.timeToGetToCover;
                if (this.timerToGetToCover > 1.0f)
                    this.timerToGetToCover = 1.0f;
                Vector3 direction = Vector3.Lerp(this.intialPosition, this.targetPosition, this.timerToGetToCover) - this.transform.position;
                this.characterController.Move(direction);
            }
        }
        else
            this.characterController.Move(this.finalDirection * Time.deltaTime);
    }

    public void UpdateDirection(Vector3 playerDirection)
    {
        Vector3 eulerAngles = new Vector3(0.0f, this.cameraRoot.transform.eulerAngles.y, 0.0f);
        playerDirection = Quaternion.Euler(eulerAngles) * playerDirection;
        playerDirection.Normalize();

        this.direction = playerDirection * this.movements[this.currentMovementIndex].Speed;
    }

    public void UpdateRotation(Vector3 rotationDelta)
    {
        if (this.enableRotation)
        {
            this.transform.Rotate(Vector3.up, ((this.inverseMouseXMovement) ? -1 : 1) * rotationDelta.x * this.mouseXSensitivity * this.movements[this.currentMovementIndex].PlayerRotationMultiplicator * Time.deltaTime, Space.World);
        }
    }

    public void UpdateMovement(MovementBehaviours behaviour)
    {
        if (this.currentMovementBehaviour == MovementBehaviours.Shoulder && behaviour == MovementBehaviours.Sprint)
            return;
        this.currentMovementBehaviour = behaviour;
        this.currentMovementIndex = this.movementBehaviourIndex[behaviour];
        //Update Animation
        switch(behaviour)
        {
            case MovementBehaviours.Cover:
                this.animator.SetBool("Cover", true);
                break;
            case MovementBehaviours.Sprint:
                this.animator.SetBool("Sprint", true);
                break;
            default:
                this.animator.SetBool("Sprint", false);
                this.animator.SetBool("Cover", false);
                break;
        }
    }

    public bool CheckForCover()
    {
        RaycastHit raycastHit;
        Ray ray = new Ray(this.transform.position + this.coverPositionCheck, this.transform.forward);
        if (Physics.Raycast(ray, out raycastHit, this.maxCoverDistanceCheck))
        {
            Vector3 wallToPlayer = (raycastHit.point - this.transform.position ).normalized * this.characterController.radius;
            this.targetPosition = raycastHit.point - (wallToPlayer * 1.1f);
            this.targetPosition.y = 0.0f;
            this.intialPosition = this.transform.position;
            this.timerToGetToCover = 0.0f;
            return true;
        }
        return false;
    }

    #region Debug
    private void OnDrawGizmosSelected()
    {
        if(this.showDirection)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawRay(this.transform.position, this.direction);
            Gizmos.color = Color.black;
            Gizmos.DrawRay(this.transform.position + (Vector3.up * (this.characterController.height / 2.0f)), this.transform.forward);
        }

        if(this.showCoverCheck)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawRay(this.transform.position + this.coverPositionCheck, this.transform.forward * this.maxCoverDistanceCheck);
        }

        if(this.currentMovementBehaviour == MovementBehaviours.Cover)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawLine(this.transform.position, this.targetPosition);
        }
    }
    #endregion

    #endregion
}
