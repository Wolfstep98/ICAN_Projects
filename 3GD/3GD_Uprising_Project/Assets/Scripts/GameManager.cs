using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Fields & Properties
    [Header("References")]
    //Camera
    [SerializeField] private CustomCameraInput cameraInput = null;
    [SerializeField] private CustomCameraController cameraController = null;
    //Player
    [SerializeField] private PolareethInput playerInput = null;
    [SerializeField] private PolareethController playerController = null;
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
        if (this.cameraInput == null)
            Debug.LogError("[Missing Reference] - cameraInput is missing !");
        if (this.cameraController == null)
            Debug.LogError("[Missing Reference] - cameraController is missing !");
        if (this.playerInput == null)
            Debug.LogError("[Missing Reference] - playerInput is missing !");
        if (this.playerController == null)
            Debug.LogError("[Missing Reference] - playerController is missing !");
#endif
    }
    #endregion

    private void Update(){

        this.playerInput.CustomUpdate();
        this.cameraInput.CustomUpdate();

        this.playerController.CustomUpdate();
    }

    private void FixedUpdate(){
        this.playerController.CustomFixedUpdate();
    }

    private void LateUpdate() {
        this.cameraController.CustomLateUpdate();
    }

    #endregion
}
