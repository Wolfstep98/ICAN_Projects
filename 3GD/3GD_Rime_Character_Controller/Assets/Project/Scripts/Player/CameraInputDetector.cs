using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraInputDetector : MonoBehaviour {

	[SerializeField] private CameraController cameraController;
	[SerializeField] private string horizontalInputName;
	[SerializeField] private string verticalInputName;

	// Update is called once per frame
	public void CustomUpdate () {		
		if (Math.Abs(Input.GetAxis(this.horizontalInputName)) > 0.0f || Math.Abs(Input.GetAxis(this.verticalInputName)) > 0.0f ){
			this.cameraController.UpdateAngleManual(new Vector3(
				Input.GetAxis(this.verticalInputName),
				Input.GetAxis(this.horizontalInputName),
				0).normalized);
		}
	}
}
