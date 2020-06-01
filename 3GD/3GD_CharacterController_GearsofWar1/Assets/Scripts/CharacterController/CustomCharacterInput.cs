using System;
using UnityEngine;

public class CustomCharacterInput : MonoBehaviour
{
    [SerializeField]
    private CustomCharacterController controller = null;
	
    public void CustomUpdate()
    {
        if (Input.GetButton(Inputs.Horizontal) || Input.GetButton(Inputs.Vertical))
        {
            Vector3 direction = new Vector3(Input.GetAxis(Inputs.Horizontal), 0.0f, Input.GetAxis(Inputs.Vertical)).normalized;
            controller.UpdateDirection(direction);
        }
        else
        {
            controller.UpdateDirection(Vector3.zero);
        }

        Vector3 rotation = new Vector3(Input.GetAxis(Inputs.MouseHorizontal), 0.0f, 0.0f);
        controller.UpdateRotation(rotation);

        if (Input.GetButtonDown(Inputs.Run))
        {
            controller.UpdateCharacter(false, true, false);
        }
        else if (Input.GetButtonUp(Inputs.Run))
        {
            controller.UpdateCharacter(true, false, false);
        }

        if (Input.GetButtonDown(Inputs.Aim))
        {
            controller.UpdateCharacter(false, false, true);
        }
        else if (Input.GetButtonUp(Inputs.Aim))
        {
            controller.UpdateCharacter(true, false, false);
        }
    }
}
