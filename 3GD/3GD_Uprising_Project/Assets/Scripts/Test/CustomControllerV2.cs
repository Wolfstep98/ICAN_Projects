using System;
using UnityEngine;

public class CustomControllerV2 : MonoBehaviour
{
    #region Fields & Properties
    [Header("Parameters")]
    [SerializeField]
    private int currentSide = 0;
    [SerializeField]
    private int side = 0;
    [SerializeField]
    private float rotationSpeed = 5.0f;
    [SerializeField]
    private float currentRotation = 0.0f;
    [SerializeField]
    private float targetRotationZ = 0.0f;
    [SerializeField]
    private float angleThreshold = 1.0f;
    [SerializeField]
    private Axis normalRotationAxis = Axis.X;
    public Axis NormalRotationAxis { get { return this.normalRotationAxis; } }
    [SerializeField]
    private Vector3 rotateAngle = Vector3.zero;
    [SerializeField]
    private Vector3 rotation = Vector3.down;
    [SerializeField]
    private Vector3 targetRotation = Vector3.down;
    [SerializeField]
    private Vector3 intialRotation = Vector3.down;

    [Header("Reference")]
    [SerializeField]
    private Transform root = null;
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
        if (this.root == null)
            Debug.LogError("[Missing Reference] - root is missing !");
#endif
    }
	#endregion
	

    public void CustomUpdate()
    {
        if (this.currentRotation != this.targetRotationZ)
        {
            //this.rotation = Vector3Calculus.RotateVector3ByAngle(this.rotation, this.rotationSpeed * side, this.normalRotationAxis);
            //Debug.Log("Rotation : " + this.rotation);

            //this.currentRotation = Vector3.Angle(this.intialRotation, this.rotation);

            this.currentSide = (Vector3.Cross(this.intialRotation, -this.root.up).z > 0) ? 1 : -1;

            if (this.targetRotationZ > this.currentRotation) 
            {
                if(this.targetRotationZ - this.currentRotation < 360 - this.targetRotationZ + this.currentRotation)
                {
                    this.currentRotation += this.rotationSpeed * Time.deltaTime;
                }
                else
                {
                    this.currentRotation -= this.rotationSpeed * Time.deltaTime;
                }
            }
            else
            {
                if (this.targetRotationZ - this.currentRotation < 360 - this.targetRotationZ + this.currentRotation)
                {
                    if (this.currentSide == -1 && this.side == 1)
                    {
                        if (this.currentRotation - this.targetRotationZ < 360 - this.currentRotation + this.targetRotationZ)
                        {
                            this.currentRotation -= this.rotationSpeed * Time.deltaTime;
                        }
                        else
                        {
                            this.currentRotation += this.rotationSpeed * Time.deltaTime;
                        }
                    }
                    else
                    {
                        if (this.targetRotationZ - this.currentRotation < 360 - this.targetRotationZ + this.currentRotation)
                        {
                            this.currentRotation -= this.rotationSpeed * Time.deltaTime;
                        }
                        else
                        {
                            this.currentRotation += this.rotationSpeed * Time.deltaTime;
                        }
                    }
                }
                else
                {
                    this.currentRotation -= this.rotationSpeed * Time.deltaTime;
                }
            }

            this.currentRotation %= 360;
            if (this.currentRotation < 0)
                this.currentRotation += 360;

            if (Math.Abs(this.currentRotation - this.targetRotationZ) < this.angleThreshold)
            {
                this.currentRotation = this.targetRotationZ;
            }

            Vector3 rot = this.root.rotation.eulerAngles;
            rot.z = this.currentRotation;
            this.root.rotation = Quaternion.Euler(rot);
        }
    }

    public void CustomFixedUpdate()
    {

    }

    public void UpdateRotation(Vector2 rotation)
    {
        if (rotation == Vector2.zero)
            return;
        rotation.Normalize();
        switch (this.normalRotationAxis)
        {
            case Axis.X:
                this.targetRotation = new Vector3(0.0f, rotation.x, rotation.y);
                this.rotateAngle = new Vector3(0.0f, 0.0f, 1.0f);
                break;
            case Axis.Y:
                this.targetRotation = new Vector3(rotation.x, 0.0f, rotation.y);
                this.rotateAngle = new Vector3(1.0f, 0.0f, 0.0f);
                break;
            case Axis.Z:
                this.targetRotation = new Vector3(rotation.x, rotation.y, 0.0f);
                this.rotateAngle = new Vector3(0.0f, 1.0f, 0.0f);
                break;
            default:
                break;
        }
        this.side = (Vector3.Cross(this.intialRotation, this.targetRotation).z > 0) ? 1 : -1;
        if (this.side == -1)
            this.targetRotationZ = 360 - Vector3.Angle(this.intialRotation, this.targetRotation);
        else
            this.targetRotationZ = Vector3.Angle(this.intialRotation, this.targetRotation);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawRay(this.transform.position, this.rotation * 1.1f);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(this.transform.position, this.targetRotation);
    }
    #endregion
}
