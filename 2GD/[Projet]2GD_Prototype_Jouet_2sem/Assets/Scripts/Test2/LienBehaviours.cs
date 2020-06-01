using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LienBehaviours : MonoBehaviour {

    public GameObject playerLinked;

    private void Awake()
    {
        playerLinked = GameObject.Find("Ball");
    }

    public void ChangePlayerLinked(GameObject player)
    {
        playerLinked = player;
        Debug.Log("Player updated");
    }

}
