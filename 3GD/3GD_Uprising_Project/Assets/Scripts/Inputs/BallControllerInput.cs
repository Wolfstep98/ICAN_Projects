using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallControllerInput : MonoBehaviour 
{
    #region Fields & Properties
    [Header("References")]
    [SerializeField]
    private BallController controller = null;
	#endregion

	#region Methods
	#region Initializers
	void Awake () 
	{
#if UNITY_EDITOR
        if (this.controller == null)
            Debug.LogError("[Missing Reference] - controller is not set !");
#endif
    }
	#endregion

	void Update () 
	{
		if(Input.GetButtonDown(InputNames.DisableStickBehaviour))
        {
            Debug.Log("Reset Stick Behaviours");
            this.controller.DisableStickBehaviours();
        }
	}

    private void OnCollisionEnter(Collision collision)
    {
        this.controller.OnGameObjectCollision(collision.gameObject);
    }
    #endregion
}
