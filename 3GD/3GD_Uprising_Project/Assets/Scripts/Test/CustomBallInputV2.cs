using System;
using UnityEngine;

public class CustomBallInputV2 : MonoBehaviour
{
    #region Fields & Properties
    [Header("Data")]
    [Header("Left Trigger")]
    [SerializeField]
    private bool leftTriggerPressed = false;
    [SerializeField]
    private float leftTriggerValue = 0.0f;
    [Header("Right Trigger")]
    [SerializeField]
    private bool rightTriggerPressed = false;
    [SerializeField]
    private float rightTriggerValue = 0.0f;

    [Header("References")]
    [SerializeField]
    private CustomBallControllerV2 controller = null;
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
	
    public void CustomUpdate()
    {
        //Left Trigger
        this.leftTriggerPressed = Input.GetButton(InputNames.LeftTrigger);
        this.leftTriggerValue = Input.GetAxis(InputNames.LeftTrigger);

        //Right Trigger
        this.rightTriggerPressed = Input.GetButton(InputNames.RightTrigger);
        this.rightTriggerValue = Input.GetAxis(InputNames.RightTrigger);

        if(this.leftTriggerPressed || this.rightTriggerPressed)
        {
            if(this.leftTriggerPressed && !this.rightTriggerPressed)
            {
                this.controller.UpdateRotationDirection(RotationDirection.CounterClockwise);
                this.controller.UpdateRotationForce((this.leftTriggerValue + 1.0f) * 0.5f);
            }
            else if(!this.leftTriggerPressed && this.rightTriggerPressed)
            {
                this.controller.UpdateRotationDirection(RotationDirection.Clockwise);
                this.controller.UpdateRotationForce((this.rightTriggerValue + 1.0f) * 0.5f);
            }
            else
            {
                this.controller.UpdateRotationDirection(RotationDirection.Clockwise);
                this.controller.UpdateRotationForce((this.rightTriggerValue + 1.0f) * 0.5f);
            }
        }
        else
        {
            this.controller.UpdateRotationForce(0.0f);
        }

        if(Input.GetButton("Left Button"))
        {
            this.controller.StopWatch();
        }
    }

	#endregion
}
