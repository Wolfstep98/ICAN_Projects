using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerManager : MonoBehaviour
{
    #region Fields & Properties
    [Header("References")]
    [SerializeField]
    private RimeInputDetector rimeInputDetector = null;
    [SerializeField]
    private CustomCharacterController customCharacterController = null;
    #endregion

    #region Methods
    #region Initialization
    private void Awake()
    {
        this.Initialize();
    }

    private void Initialize()
    {
#if UNITY_EDITOR
        if (this.rimeInputDetector == null)
            Debug.LogError("[Missing Reference] - rimeInputDetector is not set !");
        if (this.customCharacterController == null)
            Debug.LogError("[Missing Reference] - customCharacterController is not set !");
#endif
    }
    #endregion

    private void Update()
    {
        //Parse inputs
        this.rimeInputDetector.CustomUpdate();

        //Apply physics to the player
        this.customCharacterController.CustomUpdate();
    }
    #endregion
}
