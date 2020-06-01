using System;
using UnityEngine;

public class CustomCameraInput : MonoBehaviour
{
    [SerializeField]
    private CustomCameraController controller = null;

    public void CustomUpdate()
    {
        Vector3 rotation = new Vector3(0.0f, Input.GetAxis(Inputs.MouseVertical), 0.0f);
        controller.UpdateRotation(rotation);

        if(Input.GetButtonDown(Inputs.Run))
        {
            controller.UpdateCamera(false, true, false);
        }
        else if(Input.GetButtonUp(Inputs.Run))
        {
            controller.UpdateCamera(true, false, false);
        }

        if (Input.GetButtonDown(Inputs.Aim))
        {
            controller.UpdateCamera(false, false, true);
        }
        else if (Input.GetButtonUp(Inputs.Aim))
        {
            controller.UpdateCamera(true, false, false);
        }
    }
}
