using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Wall : MonoBehaviour {

    public float bounceThreshold = 0.05f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Player script = collision.gameObject.GetComponent<Player>();
            //Debug.Log("Collision with player " + collision.gameObject.name + " / " + collision.contacts.Length + " wall name : " + gameObject.name);
            switch(gameObject.name)
            {
                case "MurHaut":
                    if (script.playerTrajectory.y > 0f)
                    {
                        script.playerTrajectory.y *= -1;
                        script.playerTrajectory.y += Random.Range(-bounceThreshold, bounceThreshold);
                    }
                    break;
                case "MurBas":
                    if (script.playerTrajectory.y < 0f)
                    {
                        script.playerTrajectory.y *= -1;
                        script.playerTrajectory.y += Random.Range(-bounceThreshold, bounceThreshold);
                    }
                    break;
                case "MurGauche":
                    if (script.playerTrajectory.x < 0)
                    {
                        script.playerTrajectory.x *= -1;
                        script.playerTrajectory.x += Random.Range(-bounceThreshold, bounceThreshold);
                    }
                    break;
                case "MurDroit":
                    if (script.playerTrajectory.x > 0)
                    {
                        script.playerTrajectory.x *= -1;
                        script.playerTrajectory.x += Random.Range(-bounceThreshold, bounceThreshold);
                    }
                    break;
                default:
                    Debug.Log("This is not a wall");
                    break;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Player script = collision.gameObject.GetComponent<Player>();
            //Debug.Log("Collision with player " + collision.gameObject.name + " / " + collision.contacts.Length + " wall name : " + gameObject.name);
            switch (gameObject.name)
            {
                case "MurHaut":
                    if (script.playerTrajectory.y > 0f)
                    {
                        script.playerTrajectory.y *= -1;
                        script.playerTrajectory.y += Random.Range(-bounceThreshold, bounceThreshold);
                    }
                    break;
                case "MurBas":
                    if (script.playerTrajectory.y < 0f)
                    {
                        script.playerTrajectory.y *= -1;
                        script.playerTrajectory.y += Random.Range(-bounceThreshold, bounceThreshold);
                    }
                    break;
                case "MurGauche":
                    if (script.playerTrajectory.x < 0)
                    {
                        script.playerTrajectory.x *= -1;
                        script.playerTrajectory.x += Random.Range(-bounceThreshold, bounceThreshold);
                    }
                    break;
                case "MurDroit":
                    if (script.playerTrajectory.x > 0)
                    {
                        script.playerTrajectory.x *= -1;
                        script.playerTrajectory.x += Random.Range(-bounceThreshold, bounceThreshold);
                    }
                    break;
                default:
                    Debug.Log("This is not a wall");
                    break;
            }
        }
    }
}
