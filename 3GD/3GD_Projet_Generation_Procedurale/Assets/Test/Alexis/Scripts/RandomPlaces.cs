using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPlaces : MonoBehaviour {

	Vector3 targetposition;

	public GameObject bridge;

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if (bridge.gameObject.activeSelf) {


			Vector3 targetposition = new Vector3 (Random.Range (0, 100), Random.Range (0, 100), Random.Range (0, 100));
			transform.Translate(targetposition);
		}
		
	}




}
