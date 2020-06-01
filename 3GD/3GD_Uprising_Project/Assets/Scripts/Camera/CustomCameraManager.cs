using System;
using UnityEngine;

public class CustomCameraManager : MonoBehaviour
{
    #region Fields & Properties
    [Header("References")]
    [SerializeField]
    private CustomCameraInput input = null;
    [SerializeField]
    private CustomCameraController controller = null;
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
        if (this.controller == null)
            Debug.LogError("[Missing Reference] - controller is missing !");
#endif
    }
    #endregion

    private void Update()
    {
        //Input 
        this.input.CustomUpdate();
        //Controller
        this.controller.CustomLateUpdate();
    }

    public void CustomUpdate()
    {
        //Input 
        this.input.CustomUpdate();
        //Controller
        this.controller.CustomLateUpdate();
    }
	#endregion
}
