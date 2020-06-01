using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTerrain : MonoBehaviour
{

    public enum Biome

    {
        plaine,
        plage,
        ocean,
        montagneLow,
        montagneMedium,
        montagneHigh,
        vide

    }

    public int depth = 20;

    public int width = 256;
    public int height = 256;

    public int offsetX;
    public int offsetY;

    public float scale = 20f;
    [Range(0f,1f)]
    public float minValue = 0.7f;

    public Terrain terrain;

    // Use this for initialization
    void Start()
    {
        terrain = GetComponent<Terrain>();
        //terrain.terrainData = GenerateTerrains(terrain.terrainData);
    }

    TerrainData GenerateTerrains (TerrainData terrainData)
    {
        terrainData.heightmapResolution = width + 1;

        terrainData.size = new Vector3(width, depth, height);

        terrainData.SetHeights(0, 0, GenerateHeights());

        return terrainData;
    }

    float[,] GenerateHeights()
    {
        float[,] heights = new float[width, height]; 
        for(int x = 0; x < width; x++)
        {
            for(int y =0; y < height; y++)
            {
                float value = GenerateHeight(x, y);
                if (value >= minValue)
                    heights[x, y] = value;
                else
                    heights[x, y] = 0f;

            }
        }
        return heights;
    }

    float GenerateHeight(int x, int y)
    {
        float xCoord = offsetX + (float)x / width * scale;
        float yCoord = offsetY + (float)y / width * scale;

        return Mathf.PerlinNoise(xCoord, yCoord);
    }

    private void Update()
    {
        terrain.terrainData = GenerateTerrains(terrain.terrainData);
    }
}
