using System;
using UnityEngine;

public class BallManager : MonoBehaviour 
{
    #region Fields & Properties
    [Header("References")]
    [SerializeField]
    private BallInput input = null;
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
        if (this.input == null)
            Debug.LogError("[Missing Reference] - input is missing !");
        if (this.controller == null)
            Debug.LogError("[Missing Reference] - controller is missing !");
#endif
    }
	#endregion
	
	private void Update () 
	{
        //Input
        this.input.CustomUpdate();

        //Controller
        this.controller.CustomUpdate();
	}

    private void FixedUpdate()
    {
        //Controller
        this.controller.CustomFixedUpdate();
    }
    #endregion
}
