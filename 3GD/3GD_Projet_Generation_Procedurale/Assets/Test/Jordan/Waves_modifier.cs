using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waves_modifier : MonoBehaviour {

    public bool isActive;

	public Vector2 scale = Vector2.one;

	public Vector2 offset = Vector2.zero;

	public float amplitude = 1f;

	Mesh mesh;

	Vector3[] vertices;

    GenerateWorld generateWorld;

    GameObject player;

    public Vector3 tileChunksPos;

    public Vector3 tileInChunk;

    // Use this for initialization
    void Start()
    {
        isActive = false;
        generateWorld = GameObject.Find("Main Camera").GetComponent<GenerateWorld>();
        player = GameObject.FindGameObjectWithTag("Player");

        //tileChunksPos = new Vector3(Mathf.CeilToInt(transform.position.x / generateWorld.width) - 1,0, Mathf.CeilToInt(transform.position.z / generateWorld.height) - 1);
        mesh = GetComponent<MeshFilter>().mesh;

        vertices = mesh.vertices;
        for (int index = 0; index < vertices.Length; ++index)
        {
            float hauteur;

            hauteur = Mathf.PerlinNoise((vertices[index].x * scale.x + offset.x * 2), (vertices[index].z * scale.y + offset.y));

            vertices[index].y = (hauteur - 0.5f) * amplitude;
        }
        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
    }

	// Update is called once per frame
	void Update ()
    {
        if (isActive)
        {
            for(int index = 0; index < vertices.Length; ++index)
            {

                float hauteur;

                hauteur = Mathf.PerlinNoise(((vertices[index].x + offset.x) * scale.x), ((vertices[index].z + offset.y) * scale.y));

                vertices[index].y = (hauteur - 0.5f) * amplitude;
            }
            mesh.vertices = vertices;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals ();
            mesh.RecalculateTangents ();   
        }
        //pour animer
        offset.x += Time.deltaTime;
        offset.y += Time.deltaTime / 2f;
    }

    //Set the waves active when the player is near
    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "HitBoxActivation")
        {
            isActive = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.name == "HitBoxActivation" && !isActive)
        {
            isActive = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "HitBoxActivation")
        {
            isActive = false;
        }
    }
}
