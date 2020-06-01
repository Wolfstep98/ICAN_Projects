using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObj2 : MonoBehaviour 
{
    public Vector3 distance = Vector3.zero;

    [SerializeField]
    Transform obj = null;

	void Awake () 
	{
		
	}

	private void Update () 
	{
        Vector3 position = obj.position + distance;
        transform.position = position;
	}
}
