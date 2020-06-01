using System;
using UnityEngine;

public class ArmRotationManager : MonoBehaviour
{
    #region Fields & Properties
    [Header("References")]
    [SerializeField]
    private ArmRotationInput armRotationInput = null;
    [SerializeField]
    private ArmRotation armRotation = null;
    #endregion

    #region Methods
    private void Update()
    {
        this.armRotationInput.CustomUpdate();

        this.armRotation.CustomUpdate();
    }
    #endregion
}
