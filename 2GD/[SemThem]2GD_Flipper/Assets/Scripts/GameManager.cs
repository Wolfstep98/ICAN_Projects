using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{

    public GameObject[] Platforms = new GameObject[7];

    public GameObject[] Walls = new GameObject[4];

    public GameObject[] Boules = new GameObject[10];

    public float changeBounceValueAllWall;
    public float changeBounceValueGround;
    public float changeBounceValueLeftWall;
    public float changeBounceValueRightWall;
    public float changeBounceValueTopWall;

    public float bounceThreshold = 0.5f;

    public float bouleMass = 1;

    private float newValue;

    public AudioSource musicDauphin;


    // Use this for initialization
    void Start()
    {
        musicDauphin = GetComponent<AudioSource>();
        if (Platforms[0] == null)
        {
            Platforms = GameObject.FindGameObjectsWithTag("Platform");
        }
        if(Walls[0] == null)
        {
            Walls = GameObject.FindGameObjectsWithTag("Mur");
            Walls[2] = GameObject.FindGameObjectWithTag("Plafond");
            Walls[3] = GameObject.FindGameObjectWithTag("Sol");
        }
        if(Boules[0] == null)
        {
            Boules = GameObject.FindGameObjectsWithTag("Boule");
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        Walls[0].GetComponent<Bounce>().bounceForce = changeBounceValueGround;
        Walls[1].GetComponent<Bounce>().bounceForce = changeBounceValueTopWall;
        Walls[2].GetComponent<Bounce>().bounceForce = changeBounceValueLeftWall;
        Walls[3].GetComponent<Bounce>().bounceForce = changeBounceValueRightWall;
        if(changeBounceValueAllWall != newValue)
        {
            changeBounceValueGround = changeBounceValueLeftWall = changeBounceValueRightWall = changeBounceValueTopWall = changeBounceValueAllWall;
            foreach (GameObject mur in Walls)
            {
                Bounce scriptMur = mur.GetComponent<Bounce>();
                scriptMur.bounceForce = changeBounceValueAllWall;
                scriptMur.bounceThreshold = bounceThreshold;
            }
        }
        newValue = changeBounceValueAllWall;
        foreach(GameObject boule in Boules)
        {
            if (boule != null)
            {
                boule.GetComponent<Rigidbody>().mass = bouleMass;
            }
        }
    }
}
