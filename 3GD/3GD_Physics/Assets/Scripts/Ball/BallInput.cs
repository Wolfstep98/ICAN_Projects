using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallInput : MonoBehaviour 
{
    #region Fields & Properties
    [Header("References")]
    [SerializeField]
    private BallController controller = null;
	#endregion

	#region Methods
	#region Initializers
	private void Awake () 
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
        this.ProcessInputs();
    }

    private void ProcessInputs()
    {
        if(Input.GetButton(InputNames.Move))
        {
            this.controller.UpdateMovement(Input.mousePosition);
        }
    }

    public void UpdatePhysicMaterial(int value)
    {
        this.controller.UpdateState(value);
    }
	#endregion
}
