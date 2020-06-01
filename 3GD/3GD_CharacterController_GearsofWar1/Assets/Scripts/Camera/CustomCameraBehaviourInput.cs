using System;
using UnityEngine;

public class CustomCameraBehaviourInput : MonoBehaviour
{
    #region Fields & Properties
    [Header("References")]
    [SerializeField]
    private CustomCameraBehaviourController controller = null;
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
            Debug.LogError("[Missing Reference] - controller is missing ! "); 
#endif
    }
	#endregion
	
    public void CustomUpdate()
    {
        //Check Input for the rotation the camera
        Vector3 rotationDelta = new Vector3(0.0f, Input.GetAxis(InputNames.MouseY), 0.0f);
        this.controller.UpdateRotation(rotationDelta);

        //Sprint
        if(Input.GetButtonDown(InputNames.Sprint))
        {
            if (this.controller.CurrentCameraBehaviour != CameraBehaviours.WalkCam)
                this.controller.UpdateCameraBehaviour(CameraBehaviours.SprintCam);
        }
        else if(Input.GetButtonUp(InputNames.Sprint))
        {
            if (this.controller.CurrentCameraBehaviour == CameraBehaviours.SprintCam)
                this.controller.UpdateCameraBehaviour(CameraBehaviours.FreeCam);
        }

        //Aim
        if (Input.GetButtonDown(InputNames.Aim))
        {
            if (this.controller.CurrentCameraBehaviour != CameraBehaviours.WalkCam)
                this.controller.UpdateCameraBehaviour(CameraBehaviours.ShoulderCam);
        }
        else if (Input.GetButtonUp(InputNames.Aim))
        {
            if (this.controller.CurrentCameraBehaviour == CameraBehaviours.ShoulderCam)
                this.controller.UpdateCameraBehaviour(CameraBehaviours.FreeCam);
        }
    }

    public void OnWalkZoneEnter()
    {
        if(this.controller.CurrentCameraBehaviour != CameraBehaviours.WalkCam)
            this.controller.UpdateCameraBehaviour(CameraBehaviours.WalkCam);
    }

    public void OnWalkZoneLeave()
    {
        if (this.controller.CurrentCameraBehaviour == CameraBehaviours.WalkCam)
            this.controller.UpdateCameraBehaviour(CameraBehaviours.FreeCam);
    }
	#endregion
}
