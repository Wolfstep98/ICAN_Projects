using System;
using UnityEngine;

public class CustomCameraInput : MonoBehaviour
{
    #region Fields & Properties
    [Header("References")]
    [SerializeField]
    private CustomCameraController controller = null;
    [SerializeField]
    private Arm leftArm = null;
    [SerializeField]
    private Arm rightArm = null;
    [SerializeField][Range(1, 100)]
    private float sensitivity = 30;
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
        if (this.leftArm == null)
            Debug.LogError("[Missing Reference] - leftArm is missing !");
        if (this.rightArm == null)
            Debug.LogError("[Missing Reference] - rightArm is missing !");
#endif
    }
    #endregion

    public void CustomUpdate()
    {
        if (Input.GetAxis(InputNames.DpadX) != 0 || Input.GetAxis(InputNames.DpadY) != 0)
            this.controller.SetRotation(Input.GetAxis(InputNames.DpadX) * this.sensitivity, Input.GetAxis(InputNames.DpadY) * this.sensitivity);

        //if (Input.GetAxis(InputNames.LeftTrigger) == 0 && this.leftArm.IsNeutral)
        //{
        //    this.controller.SetRotation(-Input.GetAxis(InputNames.LeftStickX) * this.sensitivity, -Input.GetAxis(InputNames.LeftStickY) * this.sensitivity);
        //}
        if (Input.GetAxis(InputNames.RightTrigger) == 0 && this.rightArm.IsNeutral)
        {
            this.controller.SetRotation(-Input.GetAxis(InputNames.RightStickX) * this.sensitivity, -Input.GetAxis(InputNames.RightStickY) * this.sensitivity);
        }

        if (Input.GetButtonDown(InputNames.Button3))
            this.controller.RecenterCamera();
    }
	#endregion
}
