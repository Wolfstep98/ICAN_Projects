using System;
using UnityEngine;

public class RimeInputDetector : MonoBehaviour
{
    #region Fields & Properties
    [Header("References")]
    [SerializeField]
    private CustomCharacterController customCharacterController = null;
    [SerializeField]
    private CameraSwitcher cameraSwitcher = null;
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
        if (this.customCharacterController == null)
            Debug.LogError("[Missing Reference] - customCharacterController is not set !");
        if (this.cameraSwitcher == null)
            Debug.LogError("[Missing Reference] - cameraSwitcher is not set !");
#endif
    }
    #endregion

    public void CustomUpdate()
    {
        this.ParseInputs();
    }

    private void ParseInputs()
    {
        if(Input.GetButton(InputNames.Horizontal))
        {
            float right = Input.GetAxis(InputNames.Horizontal);
            Vector3 direction = (right >= 0) ? Vector3.right : Vector3.left;
            this.customCharacterController.Move(direction);
            this.customCharacterController.Rotation(direction);
        }
        if (Input.GetButton(InputNames.Vertical))
        {
            float forward = Input.GetAxis(InputNames.Vertical);
            Vector3 direction = (forward >= 0) ? Vector3.forward : Vector3.back;
            this.customCharacterController.Move(direction);
            this.customCharacterController.Rotation(direction);
        }

        for(int i = 1; i <= this.cameraSwitcher.NumberOfCamera;i++)
        {
            if(Input.GetButtonDown(InputNames.CameraSwitcher + i))
            {
                this.cameraSwitcher.SetMainCamera(i - 1);
            }
        }
    }
    #endregion
}
