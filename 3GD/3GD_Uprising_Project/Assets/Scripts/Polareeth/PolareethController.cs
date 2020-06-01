using System;
using UnityEngine;

public class PolareethController : MonoBehaviour
{
    #region Fields & Properties
    [Header("Parameters")]
    [SerializeField]
    private float headDefaultMass = 1.0f;
    [SerializeField]
    private float headHeavyMass = 10.0f;

    [Header("References")]
    [SerializeField]
    private Rigidbody headRigidbody = null;
    [SerializeField]
    private Arm leftArm = null;
    [SerializeField]
    private Arm rightArm = null;
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
        if (this.leftArm == null)
            Debug.LogError("[Missing Reference] - leftArm is missing !");
        if (this.rightArm == null)
            Debug.LogError("[Missing Reference] - rightArm is missing !");
#endif
    }
    #endregion

    public void CustomUpdate()
    {
        //Left Update
        this.leftArm.CustomUpdate();

        //Right Update
        this.rightArm.CustomUpdate();
    }

    public void CustomFixedUpdate()
    {
        //Left Fixed Update
        this.leftArm.CustomFixedUpdate();

        //Right Fixed Update
        this.rightArm.CustomFixedUpdate();

        if(!this.leftArm.IsNeutral && !this.rightArm.IsNeutral)
        {
            //this.headRigidbody.mass = this.headHeavyMass;
        }
        else
        {
            //this.headRigidbody.mass = this.headDefaultMass;
        }
    }

    public void UpdateDirection(Vector2 direction, bool leftArm)
    {
        Vector3 direction3 = new Vector3(direction.x, 0.0f, direction.y);
        if (leftArm)
        {
            this.leftArm.UpdateDirection(direction3);
        }
        else
        {
            this.rightArm.UpdateDirection(direction3);
        }
    }

    public void UpdateHeight(float value, bool leftArm)
    {
        if (leftArm)
            this.leftArm.UpdateHeightForce(value);
        else
            this.rightArm.UpdateHeightForce(value);
    }

    public void UpdateGravity(float gravity)
    {
        Physics.gravity = new Vector3(0.0f, gravity, 0.0f);
        this.leftArm.UpdateGravity(gravity);
        this.rightArm.UpdateGravity(gravity);
    }

    #region Polarity
    public void InverseLeftArmPolarity()
    {
        this.InverseArmPolarity(this.leftArm);
    }

    public void InverseRightArmPolarity()
    {
        this.InverseArmPolarity(this.rightArm);
    }

    private void InverseArmPolarity(Arm arm)
    {
        arm.InversePolarity();
    }
    #endregion

    #endregion
}
