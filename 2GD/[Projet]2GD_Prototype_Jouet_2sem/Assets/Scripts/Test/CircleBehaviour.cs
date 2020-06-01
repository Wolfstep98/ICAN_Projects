using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleBehaviour : MonoBehaviour {

    public bool rotateRight = true;
    public float rotateSpeed = 5f;

    //private Rigidbody2D rigid;

	// Use this for initialization
	void Start ()
    {
        //rigid = GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        transform.Rotate(Vector3.forward, ((rotateRight) ? rotateSpeed : -rotateSpeed) * Time.deltaTime);
	}
}
