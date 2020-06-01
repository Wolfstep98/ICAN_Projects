using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereBehaviour : MonoBehaviour {
    [Header("Explosion gestion properties")]
    public bool randomTimeBetweenMinAndMax = false;

    public float timeBeforeExplosionMin = 1f;
    public float timeBeforeExplosionMax = 2f;

    [Range(0f,10f)]
    public float timeBeforeExplosion = 2f;

    [Header("Debug")]
    public bool canExplode;
    public float coutdown = 0;
    public float coutdownInit;

    public MeshRenderer mesh;
    public Color initialColor;
    public Color meshColor;


	// Use this for initialization
	void Start () {
        canExplode = false;
        mesh = GetComponent<MeshRenderer>();
        initialColor = mesh.material.color;
    }
	
	// Update is called once per frame
	void Update () {
        if(canExplode)
        {
            coutdown -= Time.deltaTime;
            if (coutdown > 0)
            {
                meshColor = Color.Lerp(initialColor, Color.black, 1f - (coutdown / coutdownInit));
                mesh.material.color = meshColor;
            }
            if (coutdown <= 0)
                Destroy(gameObject);
        }
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && !canExplode)
        {
            coutdownInit = coutdown = (randomTimeBetweenMinAndMax) ? Random.Range(timeBeforeExplosionMin, timeBeforeExplosionMax) : timeBeforeExplosion;
            canExplode = true;
        }
    }

    private void OnDestroy()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for(int player =0; player < players.Length;player++)
        {
            if(players[player].GetComponent<PlayerMovement>().circleInCollision == gameObject)
            {
                Destroy(players[player]);
            }
        }
    }
}
