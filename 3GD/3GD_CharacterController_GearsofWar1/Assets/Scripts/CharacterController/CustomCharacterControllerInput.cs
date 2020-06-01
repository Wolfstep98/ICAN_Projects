using System;
using UnityEngine;

public class CustomCharacterControllerInput : MonoBehaviour
{
    #region Fields & Properties
    [Header("References")]
    [SerializeField]
    private CustomCharacterControllerController controller = null;
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
        if (this.controller == null)
            Debug.LogError("[Missing Reference] - controller is missing !");
#endif
    }
#endregion

    public void CustomUpdate()
    {
        //Character Movement
        if (Input.GetButton(InputNames.KeyboardX) || Input.GetButton(InputNames.KeyboardY))
        {
            if (this.controller.CurrentMovementBehaviour == MovementBehaviours.Cover)
            {
                this.controller.UpdateMovement(MovementBehaviours.Free);
            }
            else
            {
                Vector3 direction = new Vector3(Input.GetAxis(InputNames.KeyboardX), 0.0f, Input.GetAxis(InputNames.KeyboardY)).normalized;
                this.controller.UpdateDirection(direction);
            }
        }
        else
        {
            this.controller.UpdateDirection(Vector3.zero);
        }

        //Character Rotation
        Vector3 rotationDelta = new Vector3(Input.GetAxis(InputNames.MouseX), 0.0f, 0.0f);
        this.controller.UpdateRotation(rotationDelta);

        //Sprint
        if (Input.GetButtonDown(InputNames.Sprint))
        {
            if(this.controller.CurrentMovementBehaviour != MovementBehaviours.Walk)
                this.controller.UpdateMovement(MovementBehaviours.Sprint);
        }
        else if (Input.GetButtonUp(InputNames.Sprint))
        {
            if(this.controller.CurrentMovementBehaviour == MovementBehaviours.Sprint)
                this.controller.UpdateMovement(MovementBehaviours.Free);
        }

        //Aim
        if (Input.GetButtonDown(InputNames.Aim))
        {
            if (this.controller.CurrentMovementBehaviour != MovementBehaviours.Walk)    
                this.controller.UpdateMovement(MovementBehaviours.Shoulder);
        }
        else if (Input.GetButtonUp(InputNames.Aim))
        {
            if (this.controller.CurrentMovementBehaviour == MovementBehaviours.Shoulder)
                this.controller.UpdateMovement(MovementBehaviours.Free);
        }

        //Cover
        if (Input.GetButtonDown(InputNames.Cover))
        {
            if (this.controller.CurrentMovementBehaviour != MovementBehaviours.Cover && this.controller.CurrentMovementBehaviour != MovementBehaviours.Walk)
            {
                if(this.controller.CheckForCover())
                    this.controller.UpdateMovement(MovementBehaviours.Cover);
            }
            else if(this.controller.CurrentMovementBehaviour == MovementBehaviours.Cover)
            {
                this.controller.UpdateMovement(MovementBehaviours.Free);
            }
        }
    }

    public void OnWalkZoneEntered(Vector3 direction)
    {
        if (this.controller.CurrentMovementBehaviour != MovementBehaviours.Walk)
        {
            this.controller.UpdateMovement(MovementBehaviours.Walk);
            this.controller.UpdateDirection(direction);
        }
    }

    public void OnWalkZoneLeaved()
    {
        if(this.controller.CurrentMovementBehaviour == MovementBehaviours.Walk)
            this.controller.UpdateMovement(MovementBehaviours.Free);
    }

	#endregion
}
