using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Corriger la detection du sol pour prendre en compte les plateforme sortant du sol et montantes
//améliorer la detection du sol
[RequireComponent(typeof(CharacterController))]
public partial class CompleteCharacterController : MonoBehaviour {

	[SerializeField] private CharacterController characterController;
	private Vector3 direction = Vector3.zero;
	private Vector3 orientation = Vector3.zero;
	private Vector3 gravity = Vector3.zero;
	private Vector3 finalDirection = Vector3.zero;
	[SerializeField] private Transform cameraReferential;
	[SerializeField] [Range(1,15)] private float speed = 10;
	private Vector3 groundPosition;
	private bool groundDetected;
	[SerializeField] private bool grounded;
	private RaycastHit lastGroundDetectedInfos;
	[SerializeField] private float gravityForce = 9.81f;
	[SerializeField] private float gravityModifier = 1;
	[SerializeField] private float gravityMaxSpeed = 50;
	[SerializeField] private float sphereGroundDetectionRadius = 0.4f;
	[SerializeField] private float groundTolerance = 0.05f;
	[SerializeField] private float characterControllerRadiusCompensator = 0.1f;
	[SerializeField] private bool jumping;
	[SerializeField] private float jumpHeight = 8;
	[SerializeField] private float jumpTimeToReachMax = 0.5f;
	[SerializeField] private AnimationCurve jumpBehaviour;
	private float jumpTimer;
	
	// Use this for initialization
	void Awake () {
		if (this.characterController == null) {
			this.characterController = this.GetComponent<CharacterController>();
		}
	}
	
	// Update is called once per frame
	public void CustomUpdate () {
		this.DetectGround();
		this.UpdateGravity();
		this.finalDirection = this.direction + this.gravity;
		this.characterController.Move(this.finalDirection * Time.deltaTime);
		this.transform.rotation = Quaternion.LookRotation(this.orientation, Vector3.up);
		this.GroundPositionCorrection();
	}

	public void UpdateDirection(Vector3 dir) {
		var cameraAngle = this.cameraReferential.rotation;
		this.cameraReferential.eulerAngles = new Vector3(0,this.cameraReferential.eulerAngles.y,0);
		dir = this.cameraReferential.rotation * dir;
		this.cameraReferential.rotation = cameraAngle;
		this.direction = dir * this.speed;
	}

	
	public void UpdateRotation(Vector3 rot) {
		var cameraAngle = this.cameraReferential.rotation;
		this.cameraReferential.eulerAngles = new Vector3(0,this.cameraReferential.eulerAngles.y,0);
		rot = this.cameraReferential.rotation * rot;
		this.cameraReferential.rotation = cameraAngle;
		this.orientation = rot;
	}

	private void DetectGround() {
		this.groundDetected = Physics.SphereCast(new Vector3(this.transform.position.x, this.transform.position.y - this.characterController.height/2 + this.sphereGroundDetectionRadius - this.characterControllerRadiusCompensator, this.transform.position.z),
			this.sphereGroundDetectionRadius,
			Vector3.down,
			out this.lastGroundDetectedInfos,
			4,
			9);	
	}

	private void UpdateGravity() {
		if (!this.grounded && !this.jumping) {
			this.gravity = new Vector3(0,this.gravity.y - this.gravityForce * this.gravityModifier * Time.deltaTime,0);

			if (Mathf.Abs(this.gravity.y) > this.gravityMaxSpeed) {
				this.gravity = new Vector3(0,-this.gravityMaxSpeed,0);
			}
		}
		else if(this.jumping) {
			this.jumpTimer += Time.deltaTime / this.jumpTimeToReachMax;
			if (this.jumpTimer >= 1.0f) {
				this.jumping = false;
			}
			else {
				var velocity = this.jumpBehaviour.Evaluate(this.jumpTimer + Time.deltaTime / this.jumpTimeToReachMax) - this.jumpBehaviour.Evaluate(this.jumpTimer);
				this.gravity = new Vector3(0,velocity * this.jumpHeight / Time.deltaTime,0);
			}
		}
		else {
			this.gravity = new Vector3(0,0,0);
		}
	}

	private void GroundPositionCorrection() {

		if (this.transform.position.y - this.characterControllerRadiusCompensator - this.characterController.height/2 < this.lastGroundDetectedInfos.point.y - this.groundTolerance) {
			this.characterController.Move(new Vector3(0, Vector3.Distance(this.lastGroundDetectedInfos.point,new Vector3(this.transform.position.x,this.transform.position.y - this.characterControllerRadiusCompensator - this.characterController.height/2, this.transform.position.z)), 0));
			this.grounded = true;
		}
		else if(Mathf.Abs(this.transform.position.y - this.characterControllerRadiusCompensator - this.characterController.height/2 - this.lastGroundDetectedInfos.point.y) > this.groundTolerance){
			this.grounded = false;
		}
		else {
			this.grounded = true;
		}
	}

	public void UpdateJump() {
		if (this.grounded) {
			this.jumping = true;
			this.jumpTimer = 0.0f;
		}
	}

	private void OnDrawGizmos() {
		
	}

	private void OnDrawGizmosSelected() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(new Vector3(this.transform.position.x, this.transform.position.y - this.characterController.height/2 + this.sphereGroundDetectionRadius - characterControllerRadiusCompensator, this.transform.position.z),
			this.sphereGroundDetectionRadius);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(this.lastGroundDetectedInfos.point, 0.05f);
		Gizmos.color = Color.green;
		Gizmos.DrawRay(this.transform.position,this.direction);
		Gizmos.color = Color.blue;
		Gizmos.DrawRay(new Vector3(this.transform.position.x,this.transform.position.y + 0.4f,this.transform.position.z),this.orientation * 4);
		Gizmos.color = Color.magenta;
		Gizmos.DrawRay(this.transform.position,this.gravity);
	}
}
