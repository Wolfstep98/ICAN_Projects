using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailCollider : MonoBehaviour {

    public float timeBeforeDie;

    public GameObject sphereCollider;
    public PlayerMovement.Player player;

    TrailRenderer parentTrail;

	// Use this for initialization
	void Start () {
        StartCoroutine(SpawnCollider());
        parentTrail = transform.GetComponentInParent<TrailRenderer>();
        timeBeforeDie = parentTrail.time;
    }
	
    IEnumerator SpawnCollider()
    {
        while(true)
        {
            GameObject circle = Instantiate<GameObject>(sphereCollider, transform.position, transform.rotation);
            circle.name = player.ToString();
            circle.tag = "CircleCollider";
            Destroy(circle, timeBeforeDie);
            yield return new WaitForSeconds(0.02f);
        }
    }

}
