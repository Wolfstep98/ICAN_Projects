using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycast : MonoBehaviour {

	public float maxDistance = 100;
	public GameObject bridge;




	void Start () {
		
	}
	

	void Update () {
		
	}

	void FixedUpdate () {

		Raycast hit;

		//if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance) && hit.collider.gameObject.CompareTag("Player")){

			//bridge.gameObject.SetActive (true);

		//}

	}
}
