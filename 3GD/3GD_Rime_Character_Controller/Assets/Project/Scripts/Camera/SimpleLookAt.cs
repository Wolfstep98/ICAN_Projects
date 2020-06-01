using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleLookAt : MonoBehaviour {

public bool EnableLookAt = false;
	public Transform CharacterToFollow;
	private Transform selfTransform;
	// Use this for initialization
	void Awake () {
		this.selfTransform = transform;
	}
	
	// Update is called once per frame
	void Update () {
		
		
		if(this.EnableLookAt && this.CharacterToFollow != null){
			this.selfTransform.LookAt(this.CharacterToFollow);
		}
	}
}
