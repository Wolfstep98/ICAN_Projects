using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCharacterControllerManager : MonoBehaviour 
{
    #region Fields & Properties
    [Header("References")]
    [SerializeField]
    private CustomCharacterControllerInput customCharacterControllerInput = null;
    [SerializeField]
    private CustomCharacterController customCharacterController = null;
	#endregion

	#region Methods
	#region Initializers
	// Use this for initialization
	void Awake () 
	{
#if UNITY_EDITOR
        if (this.customCharacterControllerInput == null)
            Debug.LogError("[Misisng Reference] - customCharacterControllerInput not set !");
        if (this.customCharacterController == null)
            Debug.LogError("[Misisng Reference] - customCharacterController not set !");
#endif
    }
#endregion
	
	// Update is called once per frame
	void Update () 
	{
        this.customCharacterControllerInput.CustomUpdate();

        this.customCharacterController.CustomUpdate();
	}
#endregion
}
