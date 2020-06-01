using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterInputDetector : MonoBehaviour {

	[SerializeField] private CompleteCharacterController characterController;
	[SerializeField] private string horizontalInputName;
	[SerializeField] private string verticalInputName;
	[SerializeField] private string jumpInputName;

	void Awake() {
		if (this.characterController == null) {
			this.characterController = this.GetComponent<CompleteCharacterController>();
		}
	}
	
	// Use this for initialization
	public void Init (string vertical, string horizontal){
		this.horizontalInputName = horizontal;
		this.verticalInputName = vertical;
	}
	
	// Update is called once per frame
	public void CustomUpdate () {
		if (this.characterController != null && (Input.GetButton(this.horizontalInputName)  || Input.GetButton(this.verticalInputName) )){
			var tmpVec = new Vector3(Input.GetAxis(this.horizontalInputName), 0, Input.GetAxis(this.verticalInputName)).normalized;
			this.characterController.UpdateDirection(tmpVec);
			this.characterController.UpdateRotation(tmpVec);
		}
		else {
			this.characterController.UpdateDirection(Vector3.zero);
		}

		if (this.characterController != null && Input.GetButtonDown(this.jumpInputName)) {
			this.characterController.UpdateJump();
		}
			
	}
}
