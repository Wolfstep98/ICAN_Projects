using System;
using UnityEngine;

public class PolareethInput : MonoBehaviour
{
	#region Fields & Properties
	[Header("References")]
    [SerializeField]
    private PolareethController controller = null;
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
        if (this.controller == null)
            Debug.LogError("[Missing Reference] - controller is missing !");
#endif
    }
    #endregion

    public void CustomUpdate()
    {
        // --- Debug Input ---
        //Debug.Log("Left Button : " + Input.GetButtonDown(InputNames.LeftButton));
        //Debug.Log("Left Trigger : " + Input.GetButtonDown(InputNames.LeftTrigger) + Input.GetAxis(InputNames.LeftTrigger));
        //Debug.Log("Left Stick : " + Input.GetAxis(InputNames.LeftStickX) + " , " + Input.GetAxis(InputNames.LeftStickY));
        //Debug.Log("Right Button : " + Input.GetButtonDown(InputNames.RightButton));
        //Debug.Log("Right Trigger : " + Input.GetButtonDown(InputNames.RightTrigger) + Input.GetAxis(InputNames.RightTrigger));
        //Debug.Log("Right Stick : " + Input.GetAxis(InputNames.RightStickX) + " , " + Input.GetAxis(InputNames.RightStickY));

        // --- Left Arm Inputs ---
        //Inverse Left Arm Polarity
        if (Input.GetButtonDown(InputNames.LeftButton))
            this.controller.InverseLeftArmPolarity();

        //Left Arm Height
        this.controller.UpdateHeight(Input.GetAxis(InputNames.LeftTrigger), true);

        //Left Arm Direction
        this.controller.UpdateDirection(new Vector2(Input.GetAxis(InputNames.LeftStickX), Input.GetAxis(InputNames.LeftStickY)), true);

        // --- Right Arm Inputs ---
        //Inverse Right Arm Polarity
        if (Input.GetButtonDown(InputNames.RightButton))
            this.controller.InverseRightArmPolarity();

        //Right Arm Height
        this.controller.UpdateHeight(Input.GetAxis(InputNames.RightTrigger), false);

        //Right Arm Direction
        this.controller.UpdateDirection(new Vector2(Input.GetAxis(InputNames.RightStickX), Input.GetAxis(InputNames.RightStickY)), false);
    }

    #region Collisions
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "GravityChanger")
        {
            GravityChanger gravityChanger = other.GetComponent<GravityChanger>();
            this.controller.UpdateGravity(gravityChanger.Gravity);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "GravityChanger")
        {
            this.controller.UpdateGravity(-9.81f);
        }
    }
    #endregion
    #endregion
}
