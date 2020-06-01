using System;
using UnityEngine;

public class PolareethManager : MonoBehaviour
{
    #region Fields & Properties
    [Header("References")]
    [SerializeField]
    private PolareethInput input = null;
    [SerializeField]
    private PolareethController controller = null;
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
        this.controller.CustomUpdate();
    }

    private void FixedUpdate()
    {
        //Controller
        this.controller.CustomFixedUpdate();
    }

    #endregion
}
