using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObj1 : MonoBehaviour 
{
    [SerializeField]
    Vector3 offset = Vector3.zero;

    [SerializeField]
    Transform target = null;

	void Update () 
	{
        this.transform.position = target.position + offset;
	}
}
