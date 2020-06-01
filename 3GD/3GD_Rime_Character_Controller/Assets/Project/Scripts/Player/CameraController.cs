using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CameraController : MonoBehaviour {


	[SerializeField] private float distance = 10.0f;
	[SerializeField] private float speed = 10.0f;
	private float angleY;
	[SerializeField] private bool inverseY;
	[SerializeField][Range(0.4f, 4.0f)] private float angleYSpeed = 1.0f;
	private float angleX;
	[SerializeField]private bool inverseX;
	[SerializeField][Range(0.4f,4.0f)] private float angleXSpeed = 1.0f;
	[SerializeField] private Vector2 angleXLimits = new Vector2(85,-85);
	[SerializeField] private Transform character;
	[SerializeField] private Transform camera;
	private RaycastHit rayHit;
	private float trueDistance;
	[SerializeField] private float timeUntilAutomatedControl = 5.0f;
	private float timerUntilAutomatedControl = 0.0f;
	
	public void CustomUpdate () {
		this.camera.rotation = Quaternion.Euler(this.angleX, this.angleY, 0.0f);
		this.trueDistance = Physics.Raycast(this.character.position, this.camera.rotation * new Vector3(0, 0, -this.distance),out this.rayHit,this.distance,9) ? Vector3.Distance(this.character.position, this.rayHit.point) : this.distance;
		this.camera.position = this.character.position + this.camera.rotation * new Vector3(0, 0, -this.trueDistance);
		if (this.timerUntilAutomatedControl < this.timeUntilAutomatedControl) {
			this.timerUntilAutomatedControl += Time.deltaTime;
		}
	}

	public void UpdateAngleManual(Vector3 vec) {
		this.angleX = this.angleX + vec.x * this.speed * this.angleXSpeed * Time.deltaTime * (this.inverseX ?  1 : - 1);
		this.AngleXVerification();
		this.angleY = this.angleY + vec.y * this.speed * this.angleYSpeed * Time.deltaTime * (this.inverseY ?  - 1 : 1);
		this.AngleYVerification();
		this.timerUntilAutomatedControl = 0.0f;
	}
	public void UpdateAngleAutomated(Vector3 vec) {
		if (this.timerUntilAutomatedControl >= this.timeUntilAutomatedControl) {
			this.angleX = this.angleX +
			              vec.x * this.speed * this.angleXSpeed * Time.deltaTime * (this.inverseX ? 1 : -1);
			this.AngleXVerification();
			this.angleY = this.angleY +
			              vec.y * this.speed * this.angleYSpeed * Time.deltaTime * (this.inverseY ? -1 : 1);
			this.AngleYVerification();
		}
	}

	private void AngleYVerification() {
		if (this.angleY > 360) {
			this.angleY = this.angleY - 360;
		} 
		else if(this.angleY < 0) {
			this.angleY = this.angleY + 360;
		}
	}
	
	private void AngleXVerification() {
		if (this.angleX > this.angleXLimits.x) {
			this.angleX = this.angleXLimits.x;
		} 
		else if(this.angleX < this.angleXLimits.y) {
			this.angleX = this.angleXLimits.y;
		}
	}
}
