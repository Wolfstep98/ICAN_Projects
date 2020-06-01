using System;
using UnityEngine;

public class CustomBallControllerV2 : MonoBehaviour
{
    #region Fields & Properties
    [Header("Parameters")]
    [SerializeField]
    private float rotationForceMultiplicator = 1.0f;
    [SerializeField]
    private float rotationForce = 0.0f;
    [SerializeField]
    private Vector3 force = Vector3.zero;
    [SerializeField]
    private RotationDirection rotationDirection = RotationDirection.Clockwise;

    [Header("References")]
    [SerializeField]
    private Transform springRoot = null;
    [SerializeField]
    private Rigidbody ballRigidbody = null;
    [SerializeField]
    private CustomControllerV2 customController = null;

    [Header("Debug")]
    [SerializeField]
    private bool showForce = true;
	#endregion
	
	#region Methods
	#region Intialization
	private void Awake()
	{
		this.Initialize();
	}
	
	private void Initialize()
	{
		
	}
    #endregion

    public void UpdateRotationDirection(RotationDirection rotationDirection)
    {
        if(this.rotationDirection != rotationDirection)
        {
            this.rotationDirection = rotationDirection;
        }
    }

    public void UpdateRotationForce(float value)
    {
        this.rotationForce = value * this.rotationForceMultiplicator;
    }

    public void StopWatch()
    {
        this.ballRigidbody.velocity = Vector3.zero;
    }

    public void CustomFixedUpdate()
    {
        //Appply force to the ball in the right direction
        Vector3 springRootToBall = this.ballRigidbody.position - this.springRoot.position;
        Debug.DrawRay(this.springRoot.position, springRootToBall, Color.blue);
        Vector3 normal = (this.rotationDirection == RotationDirection.Clockwise)? -this.springRoot.up: this.springRoot.up;
        //switch(this.customController.NormalRotationAxis)
        //{
        //    case Axis.X:
        //        normal = (this.rotationDirection == RotationDirection.Clockwise) ? Vector3.left : Vector3.right;
        //        break;
        //    case Axis.Y:
        //        normal = (this.rotationDirection == RotationDirection.Clockwise) ? Vector3.back : Vector3.forward;
        //        break;
        //    case Axis.Z:
        //        normal = (this.rotationDirection == RotationDirection.Clockwise) ? Vector3.down : Vector3.up;
        //        break;
        //    default:
        //        break;
        //}
        Debug.DrawRay(this.springRoot.position, normal);
        this.force = Vector3.Cross(springRootToBall, normal).normalized * this.rotationForce;

        this.ballRigidbody.AddForce(this.force, ForceMode.Force);
    }

    private void OnDrawGizmosSelected()
    {
        if (this.showForce)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawRay(this.ballRigidbody.position, this.force);
        }
    }
    #endregion
}
