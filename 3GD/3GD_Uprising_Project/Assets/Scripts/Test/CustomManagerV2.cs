using System;
using UnityEngine;

public class CustomManagerV2 : MonoBehaviour
{
    #region Fields & Properties
    [Header("References")]
    [SerializeField]
    private CustomInputV2 input = null;
    [SerializeField]
    private CustomBallInputV2 ballInput = null;
    [SerializeField]
    private CustomControllerV2 controller = null;
    [SerializeField]
    private CustomBallControllerV2 ballController = null;
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
        if (this.input == null)
            Debug.LogError("[Missing Reference] - input is missing !");
        if (this.ballInput == null)
            Debug.LogError("[Missing Reference] - ballInput is missing !");
        if (this.controller == null)
            Debug.LogError("[Missing Reference] - controller is missing !");
        if (this.ballController == null)
            Debug.LogError("[Missing Reference] - ballController is missing !");
#endif
    }
    #endregion

    #region Updates
    private void Update()
    {
        //Input
        this.input.CustomUpdate();

        this.ballInput.CustomUpdate();

        //Controller
        this.controller.CustomUpdate();
    }

    private void FixedUpdate()
    {
        //Controller
        this.controller.CustomFixedUpdate();

        this.ballController.CustomFixedUpdate();
    }
    #endregion
    #endregion
}
