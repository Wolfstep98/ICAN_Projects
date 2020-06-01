using System;
using UnityEngine;
using Polareeth;

[Serializable]
[RequireComponent(typeof(MeshRenderer))]
public class Arm : MonoBehaviour
{
    #region Properties
    [Header("Parameters")]
    [Header("Movement")]
    [SerializeField]
    private bool groundLocked = false;
    [SerializeField]
    private bool isNeutral = false;
    [SerializeField]
    private float speed = 5.0f;
    [SerializeField]
    private Vector3 direction = Vector3.zero;

    [SerializeField]
    private float upwardForce = 0.0f;
    [SerializeField]
    private float upwardForceModifier = 7.0f;
    [SerializeField]
    private float minForceAddedByJoint = 1.0f;
    [SerializeField]
    private float upwardForceMult = 0.5f;

    [Header("Polarity")]
    [SerializeField]
    private Polarity polarity = Polarity.None;

    [SerializeField]
    private Material polarityNone = null;
    [SerializeField]
    private Material polarityNeg = null;
    [SerializeField]
    private Material polarityPos = null;

    [Header("References")]
    [SerializeField]
    private Transform head = null;
    [SerializeField]
    private Polareeth.CustomJoint[] joints = null;
    [SerializeField]
    private Camera mainCamera = null;
    [SerializeField]
    new private Rigidbody rigidbody = null;
    [SerializeField]
    private MeshRenderer meshRenderer = null;
    #endregion

    #region Properties
    public bool IsGroundLocked { get { return this.groundLocked; } }
    public bool IsNeutral { get { return this.isNeutral; } }
    public Polarity Polarity { get { return this.polarity; } }
    #endregion

    #region Methods

    #region Initializations
    private void Awake()
    {
        this.Initialize();
    }

    protected virtual void Initialize()
    {
#if UNITY_EDITOR
        if (this.meshRenderer == null)
            Debug.LogError("[Missing Reference] - meshRenderer is missing !");

        if (this.polarityNone == null)
            Debug.LogError("[Missing Reference] - polarityNone is missing !");
        if (this.polarityPos == null)
            Debug.LogError("[Missing Reference] - polarityPos is missing !");
        if (this.polarityNeg == null)
            Debug.LogError("[Missing Reference] - polarityNeg is missing !");
#endif
        this.isNeutral = true;
        this.groundLocked = false;
        this.InitializePolarity();
    }

    protected virtual void InitializePolarity()
    {
       switch(this.polarity)
        {
            case Polarity.Negative:
                this.meshRenderer.material = this.polarityNeg;
                break;
            case Polarity.None:
                this.meshRenderer.material = this.polarityNone;
                break;
            case Polarity.Positive:
                this.meshRenderer.material = this.polarityPos;
                break;
            default:
                break;
        }
    }

    private void OnValidate()
    {
        if(this.rigidbody == null)
            this.rigidbody = GetComponent<Rigidbody>();
        if(this.meshRenderer == null)
            this.meshRenderer = GetComponent<MeshRenderer>();
    }
    #endregion

    #region Movement
    public void UpdateDirection(Vector3 direction)
    {
        Quaternion tmpRot = this.mainCamera.transform.rotation;
        this.mainCamera.transform.rotation = Quaternion.Euler(0.0f, this.mainCamera.transform.rotation.eulerAngles.y, this.mainCamera.transform.rotation.eulerAngles.z);
        direction = this.mainCamera.transform.TransformDirection(direction);
        this.mainCamera.transform.rotation = tmpRot;
        this.direction = direction;
    }

    public void UpdateHeightForce(float value)
    {
        if (value != 0)
        {
            if (this.isNeutral)
            {
                this.UnlockArm();
            }
            this.upwardForce = value * this.upwardForceModifier;
        }
        else
        {
            if(this.groundLocked)
            {
                this.isNeutral = true;
                this.rigidbody.isKinematic = true;
            }
            this.upwardForce = 0.0f;
        }
    }

    private void UnlockArm()
    {
        this.rigidbody.isKinematic = false;
        this.groundLocked = false;
        this.isNeutral = false;
        this.ApplyJointsForce();
    }

    private void ApplyJointsForce()
    {
        Vector3 upwardForce = (Vector3.up * 2 + (this.head.position - this.transform.position).normalized).normalized;
        foreach(CustomJoint joint in joints)
        {
            if(joint.ForceHorizontal > this.minForceAddedByJoint)
                upwardForceMult += joint.ForceHorizontal;
        }
        upwardForce *= this.upwardForceMult;
        this.rigidbody.AddForce(upwardForce, ForceMode.Impulse);
    }

    public void UpdateGravity(float gravity)
    {
        this.upwardForceModifier = gravity * -0.7f;
        foreach(CustomJoint joint in this.joints)
        {
            joint.UpdateGravity(gravity);
        }
    }
    #endregion

    #region Polarity
    public virtual void InversePolarity()
    {
        switch(this.polarity)
        {
            case Polarity.Negative:
                this.SetPositivePolarity();
                break;
            case Polarity.Positive:
                this.SetNegativePolarity();
                break;
            default:
                Debug.LogWarning("[Warning Polarity] - You can't inverse polarity if it's set to none !");
                break;
        }
    }

    public virtual void SetPositivePolarity()
    {
        if (this.polarity == Polarity.Positive)
            return;

        this.polarity = Polarity.Positive;
        this.meshRenderer.material = this.polarityPos;
    }

    public virtual void SetNegativePolarity()
    {
        if (this.polarity == Polarity.Negative)
            return;

        this.polarity = Polarity.Negative;
        this.meshRenderer.material = this.polarityNeg;
    }

    public virtual void SetNonePolarity()
    {
        if (this.polarity == Polarity.None)
            return;

        this.polarity = Polarity.None;
        this.meshRenderer.material = this.polarityNone;
    }
    #endregion

    #region Tick
    public void CustomUpdate()
    {

    }

    public void CustomFixedUpdate()
    {
        this.rigidbody.AddForce(Vector3.up * this.upwardForce, ForceMode.Force);
        this.rigidbody.AddForce(this.direction * this.speed, ForceMode.Force);
    }

    #endregion

    #region Collisions
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            Debug.Log("Collision");
            //this.rigidbody.isKinematic = true;
            this.groundLocked = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            if(this.groundLocked)
                this.groundLocked = false;
        }
    }
    #endregion

    #region Debug
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawRay(this.transform.position, this.direction * this.speed);
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(this.transform.position,Vector3.up * this.upwardForce);
    }
    #endregion

    #endregion
}
