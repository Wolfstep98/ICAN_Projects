using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputDetection : MonoBehaviour {

    #region Properties

    [Header("References")]
    [SerializeField] private CustomCharacterController2 controller = null;

    private Vector2 leftDir = Vector2.zero;
    private Vector2 rightDir = Vector2.zero;

	#endregion

	#region Initialization

	private void Awake()
	{
		this.Init();
	}

	private void Init()
	{
#if UNITY_EDITOR
		if (this.controller == null)
			Debug.LogError("[Missing References] - controller is not properly set !");
#endif

		this.Initiate();
	}

	private void Initiate()
	{
		
	}

	#endregion

	// Update is called once per frame
	public void CustomUpdate () {
        if (this.GetRightStickInput() != this.rightDir) {
            this.rightDir = this.GetRightStickInput();
            this.controller.SetArmRotation(this.rightDir);
        }
	}

	#region Methods

	private Vector2 GetRightStickInput() {
        Vector2 rightInput = Vector2.zero;
        rightInput.x = Input.GetAxis(InputNames.RightStickX);
        rightInput.y = Input.GetAxis(InputNames.RightStickY);
        return rightInput;
    }

    private Vector2 GetLeftStickInput() {
        Vector2 leftInput = Vector2.zero;
        leftInput.x = Input.GetAxis(InputNames.LeftStickX);
        leftInput.y = Input.GetAxis(InputNames.LeftStickY);
        return leftInput;
    }

    #endregion
}
