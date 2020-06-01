using System;
using UnityEngine;
using UnityEngine.UI;

public class ArmRotationInput : MonoBehaviour
{
    #region Fields & Properties
    [SerializeField]
    private float inputThreshold = 0.1f;

    [Header("References")]
    [SerializeField]
    private ArmRotation controller = null;

    [Header("UI")]
    [SerializeField]
    private bool showUI = false;
    [SerializeField]
    private Text text_angleX;
    [SerializeField]
    private Text text_angleY;
    [SerializeField]
    private Text text_trigger;
    #endregion

    #region Methods
    public void CustomUpdate()
    {
        if(Math.Abs(Input.GetAxisRaw(InputNames.RightStickX)) > this.inputThreshold || Math.Abs(Input.GetAxisRaw(InputNames.RightStickY)) > this.inputThreshold)
        {
            this.controller.UpdateRotation(new Vector3(Input.GetAxisRaw(InputNames.RightStickX), Input.GetAxisRaw(InputNames.RightStickY), 0.0f));
            //this.controller.UpdateRotation(new Vector3(Input.GetAxisRaw(InputNames.AngleY),Input.GetAxisRaw(InputNames.AngleX) ,0.0f));
        }
        else
        {
            this.controller.UpdateRotation(new Vector3(0.0f, 0.0f, 0.0f));
        }

        if(Input.GetAxisRaw(InputNames.RightTrigger) >= -1.0f)
        {
            this.controller.ChangeInitialRotation(Input.GetAxisRaw(InputNames.RightTrigger));
        }

        this.UpdateUI();
    }

    private void UpdateUI()
    {
        if (this.showUI)
        {
            this.text_angleX.text = "Angle X : " + Input.GetAxisRaw(InputNames.RightStickX);
            this.text_angleY.text = "Angle Y : " + Input.GetAxisRaw(InputNames.RightStickY);
            this.text_trigger.text = "Trigger : " + Input.GetAxisRaw(InputNames.RightTrigger);
        }
    }
    #endregion
}
