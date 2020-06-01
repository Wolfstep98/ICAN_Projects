using System;
using UnityEngine;

public class CustomInputV2 : MonoBehaviour
{
    #region Fields & Properties
    [Header("References")]
    [SerializeField]
    private CustomControllerV2 controller = null;
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
        this.controller.UpdateRotation(new Vector2(Input.GetAxis(InputNames.RightStickX), Input.GetAxis(InputNames.RightStickY)));
    }

	#endregion
}
