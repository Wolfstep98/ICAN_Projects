using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleSaveBehaviour : MonoBehaviour {

    bool rotateRight = false;

    public float rotateSpeed = 200;

    private void Update()
    {
        transform.Rotate(Vector3.forward, ((rotateRight) ? rotateSpeed : -rotateSpeed) * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 normal = collision.contacts[0].normal;
        Vector2 playerVelocity = collision.gameObject.GetComponent<PlayerBehaviours>().playerTrajectory;
        Vector2 contact = Vector2.zero;
        normal.Normalize();
        playerVelocity.Normalize();
        float sign = Mathf.Sign((normal.x - contact.x) * (playerVelocity.y - normal.y) - (normal.y - contact.y) * (playerVelocity.x - normal.x));
        rotateRight = (sign >= 0) ? false : true;
    }

    private void OnDestroy()
    {
        if (transform.childCount != 0)
        {
            GameObject player = transform.GetChild(0).gameObject;
            player.GetComponent<PlayerBehaviours>().Shoot(player.GetComponent<PlayerBehaviours>().shootType);
            Debug.Log("Shoot");
        }
    }
}
