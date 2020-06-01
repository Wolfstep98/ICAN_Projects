using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : MonoBehaviour {

    public float bounceForce = 10;

    public float bounceThreshold = 0.5f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Boule")
        {
            collision.rigidbody.velocity.Set(collision.rigidbody.velocity.x, 0f, collision.rigidbody.velocity.z);
            if (tag == "Sol")
                collision.rigidbody.AddForce((Vector3.up + new Vector3(Random.Range(-bounceThreshold, bounceThreshold),0,0)) * bounceForce * 2, ForceMode.Impulse);
            else if (name == "Mur Droit")
                collision.rigidbody.AddForce(Vector3.left * bounceForce, ForceMode.Impulse);
            else if (name == "Mur Gauche")
                collision.rigidbody.AddForce(Vector3.right * bounceForce, ForceMode.Impulse);
            else if (name == "Mur Haut")
                collision.rigidbody.AddForce(Vector3.down * bounceForce, ForceMode.Impulse);
            else if (tag == "Platform")
            {
                Vector3 normalCol = collision.contacts[0].normal;
            }
        }
    }

}
