using System;
using UnityEngine;

public class CustomCharacterControllerInput : MonoBehaviour 
{
    #region Fields & Properties
    [Header("Parameters")]
    private float inputThreshold = 0.15f;

    [Header("References")]
    [SerializeField]
    private CustomCharacterController customCharacterController = null;
	#endregion

	#region Methods
	#region Initializers
	// Use this for initialization
	void Awake () 
	{
#if UNITY_EDITOR
        if (this.customCharacterController == null)
            Debug.LogError("[Missing Reference] - customCharacterController not set !");
#endif
    }
    #endregion

    public void CustomUpdate()
    {
        Vector3 direction = new Vector3(Input.GetAxisRaw(InputNames.LeftStickX), 0.0f, Input.GetAxisRaw(InputNames.LeftStickY));

        //this.customCharacterController.UpdateRotation(direction);
        this.customCharacterController.UpdateDirection(direction);
    }
	#endregion
}
