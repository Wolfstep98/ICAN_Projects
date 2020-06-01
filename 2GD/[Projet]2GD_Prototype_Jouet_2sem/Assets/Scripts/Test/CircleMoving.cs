using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleMoving : MonoBehaviour {

    public Vector2 direction;
    public float speed = 5f;

	// Use this for initialization
	void Start () {
        direction = Vector2.right;
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(direction * speed * Time.deltaTime,Space.World);
	}
}
