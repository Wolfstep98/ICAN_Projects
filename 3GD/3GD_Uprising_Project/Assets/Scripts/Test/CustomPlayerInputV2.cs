using System;
using UnityEngine;

public class CustomPlayerInputV2 : MonoBehaviour
{
    #region Fields & Properties
    [Header("References")]
    [SerializeField]
    private CustomPlayerControllerV2 controller = null;
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
        this.controller.UpdateDirection(new Vector2(Input.GetAxis(InputNames.LeftStickX), Input.GetAxis(InputNames.LeftStickY)));
    }
    #endregion
}
