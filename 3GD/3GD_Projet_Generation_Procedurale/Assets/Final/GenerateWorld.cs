using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using World;

public class GenerateWorld : MonoBehaviour {

    public int precision = 10;

    public int seed = 42;

    //public int nbrOfChunks = 1;

    public int nbrOfChunksOnSide = 3;
    public int nbrOfChunksOnFront = 3;

    public int width = 128;
    public int height = 128;

    public int offsetX;
    public int offsetY;

    public float scale = 20f;
    [Range(0f, 1f)]
    public float minValue = 0.7f;

    [SerializeField]
    public Dictionary<Vector3, Chunk> chunks;

    public GameObject avatar;

    private void Awake()
    {
        Random.InitState(seed);

        offsetX = Random.Range(-100000, 100000);
        offsetY = Random.Range(-100000, 100000);

        //Generating world
        chunks = new Dictionary<Vector3, Chunk>();
        
        GenerateChunks();

        int chunkNbr = Random.Range(0, chunks.Count);
        foreach(Island isle in chunks[Vector3.zero].islands)
        {
            Instantiate<GameObject>(avatar, new Vector3(isle.x, 5f, isle.z), Quaternion.identity);
            break;
        }

    }

    public void GenerateChunks()
    {
        for (int i = -nbrOfChunksOnSide; i < nbrOfChunksOnSide; i++)
        {
            for (int j = -nbrOfChunksOnFront; j < nbrOfChunksOnFront; j++)
            {
                Chunk chunk = new Chunk(seed, i, j, width, height, offsetX, offsetY, precision, minValue, scale);
                chunks.Add(new Vector3(i, 0, j), chunk);
            }
        }
    }   
}
