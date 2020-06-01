using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CustomCharacterController : MonoBehaviour 
{
    #region Fields & Properties
    [Header("Parameters")]

    [Header("Gravity")]
    [SerializeField]
    private float gravityForce = 9.81f;
    [SerializeField]
    private float gravityModifier = 1.0f;
    [SerializeField]
    private float maxGravityForce = 30.0f;
    [SerializeField]
    private Vector3 gravity = Vector3.zero;

    //Ground Checker
    [SerializeField]
    private Vector3 groundCheckerPosition = Vector3.zero;
    [SerializeField]
    private float groundCheckerRadius = 0.5f;
    [SerializeField]
    private int layerMaskCollision = (1 << 34) - 1;

    [Header("Movement")]
    [SerializeField]
    private bool showMovementDebug = false;
    [SerializeField]
    private float moveSpeed = 10.0f;
    [SerializeField]
    private Vector3 direction = Vector3.zero;
    [SerializeField]
    private Vector3 finalDirection = Vector3.zero;

    [Header("Rotation")]
    [SerializeField]
    private bool showRotationDebug = false;
    [SerializeField]
    private Vector3 rotation = Vector3.zero;

    [Header("References")]
    [SerializeField]
    private CharacterController characterController = null;
    [SerializeField]
    private Rigidbody ballRigidbody = null;

    [Header("Debug")]
    [SerializeField]
    private bool showGravity = true;
    [SerializeField]
    private bool showGroundChecker = true;
	#endregion

	#region Methods
	#region Initializers
	// Use this for initialization
	void Awake () 
	{
#if UNITY_EDITOR
        if (this.characterController == null)
            this.characterController = this.gameObject.GetComponent<CharacterController>();
#endif
    }
	#endregion
	
    public void CustomUpdate()
    {
        this.UpdateGravity();
        this.finalDirection = this.direction + this.gravity;
        this.characterController.Move(this.finalDirection * Time.deltaTime);
    }

    private void UpdateGravity()
    {
        if(!this.characterController.isGrounded)
        {
            //RaycastHit hit;
            //if (Physics.SphereCast(this.transform.position + this.groundCheckerPosition, this.groundCheckerRadius, Vector3.down, out hit, 0.0f, this.layerMaskCollision)) 
            //{

            //}
            this.gravity = new Vector3(0.0f, this.gravity.y - this.gravityForce * this.gravityModifier * Time.deltaTime, 0.0f);
        }
        else
        {
            this.gravity = Vector3.zero;
        }
    }

    public void UpdateDirection(Vector3 direction)
    {
        direction.x = Mathf.RoundToInt(direction.x);
        direction.z = Mathf.RoundToInt(direction.z);

        if(direction.x == direction.z)
        {
            if (direction.x == 1.0f || direction.x == -1.0f)
                direction.z = 0.0f;
        }

        this.direction = direction * this.moveSpeed;
    }

    public void UpdateRotation(Vector3 direction)
    {

    }

    private void OnDrawGizmosSelected()
    {
        if(this.showGroundChecker)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(this.transform.position + this.groundCheckerPosition, this.groundCheckerRadius);
        }
        if(this.showGravity)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(this.transform.position, this.gravity);
        }
    }

    #endregion
}
