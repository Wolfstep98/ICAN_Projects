using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace World
{
    //Class définissant un chunk dans le monde
    public class Chunk
    {
        //Attributs

        public string chunkID; //Unique ID

        public int xCoord;
        public int yCoord;

        public int width; // Largeur du chunk
        public int height; // Longueur du chunk

        int offsetX;
        int offsetY;

        float scale;
        float minValue;

        int precision; //Distance between tiles

        Vector3[,] positions; //All the tiles in the chunk

        float[,] heights; // La hauteur associés à chaque tiles

        Dictionary<Vector3, GenerateTerrain.Biome> chunkInfo;
        public Dictionary<Vector3, GameObject> chunkGO;

        GameObject plaine;
        GameObject plage;
        GameObject montagneLow;
        GameObject montagneMedium;
        GameObject montagneHigh;
        GameObject ocean;

        GameObject chunk;

        [SerializeField]
        public List<Island> islands; // Toutes les iles dans ce chunk

        //Methodes

        //Constructeur
        public Chunk(int seed, int x, int y, int width, int height, int offsetX, int offsetY, int precision, float minValue, float scale)
        {
            xCoord = x;
            yCoord = y;

            chunkID = "Chunk (" + xCoord + ":" + yCoord + ")";

            this.width = width;
            this.height = height;
            this.minValue = minValue;
            this.scale = scale;
            this.offsetX = offsetX;
            this.offsetY = offsetY;
            this.precision = precision;

            Random.InitState(seed);

            montagneLow = Resources.Load<GameObject>("Montagne_S");
            montagneMedium = Resources.Load<GameObject>("Montagne_M");
            montagneHigh = Resources.Load<GameObject>("Montagne_XL");
            plaine = Resources.Load<GameObject>("Terrain");
            plage = Resources.Load<GameObject>("Plage");
            ocean = Resources.Load<GameObject>("Water");

            chunk = Object.Instantiate(Resources.Load<GameObject>("Chunks"), new Vector3(xCoord, 0, yCoord), Quaternion.identity);
            chunk.name = "Chunk (" + xCoord + ":" + yCoord + ")";
            islands = new List<Island>();

            InitialiseChunk();
        }

        void InitialiseChunk()
        {
            //Initialisation
            positions = new Vector3[width, height];
            heights = new float[width, height];
            chunkInfo = new Dictionary<Vector3, GenerateTerrain.Biome>();
            chunkGO = new Dictionary<Vector3, GameObject>();

            SetPositions();

            //On créé les iles grâce au Perlin noise
            SetHeightsToOne();

            //On fait une 2e fois un PerlinNoise pour la topologie
            SetHeights(Random.Range(-100000, -100000), Random.Range(-100000, -100000));

            //On parse le tableau et on rassemble les tiles qui sont ensembles en créant des iles
            GenerateIslands();

            //On créé les tiles
            SetBiomes();
            GenerateBiomes();
        }

        //On associe la position de chaque tile dans la scene au tableau des positions
        public void SetPositions()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    positions[x, y] = new Vector3(xCoord * width + x, 0, yCoord * height + y);
                }
            }
        }

        //On utilise le PerlinNoise pour affecter une valeur à chaque tile, si elle est plus grande que minValue on la met, sinon on met 0
        public void SetHeightsToOne()
        {
            for (int x = 0; x < positions.GetLength(0); x++)
            {
                for (int y = 0; y < positions.GetLength(1); y++)
                {
                    float height;
                    Vector3 position;
                    position = positions[x, y];
                    height = SetHeight((int)position.x, (int)position.z);
                    height = (height >= minValue) ? 1 : 0f;
                    heights[x, y] = height;
                }
            }
        }

        //On utilise le PerlinNoise pour créer la topologie des iles
        public void SetHeights(int randomValueX, int randomValueY)
        {
            for (int x = 0; x < heights.GetLength(0); x++)
            {
                for (int y = 0; y < heights.GetLength(1); y++)
                {
                    if (heights[x, y] == 0) continue;
                    float height;
                    Vector3 position;
                    position = positions[x, y];
                    height = SetHeight((int)position.x + randomValueX, (int)position.z + randomValueY);
                    heights[x, y] = height;
                }
            }
        }

        //PerlinNoise
        public float SetHeight(int x, int y)
        {
            float xCoord = offsetX + (float)x / width * scale;
            float yCoord = offsetY + (float)y / height * scale;

            return Mathf.PerlinNoise(xCoord, yCoord);
        }

        //Algorithme qui va rechercher les iles dans les tiles de ce chunk
        public void GenerateIslands()
        {
            int nbrOfIsland = 0;
            bool[,] check = new bool[heights.GetLength(0), heights.GetLength(1)];
            for (int x = 0; x < heights.GetLength(0); x++)
            {
                for (int z = 0; z < heights.GetLength(1); z++)
                {
                    if (!check[x, z])
                    {
                        if (heights[x, z] == 0)
                        {
                            check[x, z] = true;
                            continue;
                        }
                        else
                        {
                            Vector3[] tiles;
                            tiles = FindNeighbors(x, z, check);
                            //Debug.Log("island number " + nbrOfIsland + " :");
                            foreach (Vector3 tile in tiles)
                            {
                                //Debug.Log(tile.ToString());
                            }
                            islands.Add(new Island(nbrOfIsland, tiles, chunk.transform, chunkID));
                            nbrOfIsland++;
                        }
                    }
                }
            }
        }

        Vector3[] FindNeighbors(int xInit, int yInit, bool[,] check)
        {
            List<Vector3> temp = new List<Vector3>();
            temp.Add(FindNeighbor(xInit, yInit, temp, check));
            Vector3[] tiles = temp.ToArray();
            return tiles;
        }

        Vector3 FindNeighbor(int x, int y, List<Vector3> temp, bool[,] check)
        {
            check[x, y] = true;
            if (x + 1 < heights.GetLength(0) && heights[x + 1, y] != 0 && !check[x + 1, y])
            {
                temp.Add(FindNeighbor(x + 1, y, temp, check));
            }
            if (x + 1 < heights.GetLength(0) && y + 1 < heights.GetLength(1) && heights[x + 1, y + 1] != 0 && !check[x + 1, y + 1])
            {
                temp.Add(FindNeighbor(x + 1, y + 1, temp, check));
            }
            if (y + 1 < heights.GetLength(1) && heights[x, y + 1] != 0 && !check[x, y + 1])
            {
                temp.Add(FindNeighbor(x, y + 1, temp, check));
            }
            if (x - 1 > -1 && y + 1 < heights.GetLength(1) && heights[x - 1, y + 1] != 0 && !check[x - 1, y + 1])
            {
                temp.Add(FindNeighbor(x - 1, y + 1, temp, check));
            }
            if (x - 1 > -1 && heights[x - 1, y] != 0 && !check[x - 1, y])
            {
                temp.Add(FindNeighbor(x - 1, y, temp, check));
            }
            if (x - 1 > -1 && y - 1 > -1 && heights[x - 1, y - 1] != 0 && !check[x - 1, y - 1])
            {
                temp.Add(FindNeighbor(x - 1, y - 1, temp, check));
            }
            if (y - 1 > -1 && heights[x, y - 1] != 0 && !check[x, y - 1])
            {
                temp.Add(FindNeighbor(x, y - 1, temp, check));
            }
            if (y - 1 > -1 && x + 1 < heights.GetLength(0) && heights[x + 1, y - 1] != 0 && !check[x + 1, y - 1])
            {
                temp.Add(FindNeighbor(x + 1, y - 1, temp, check));
            }
            return new Vector3(precision * positions[x, y].x, 0, precision * positions[x, y].z);
        }

        //On associe à chaque tiles le biome adapté
        public void SetBiomes()
        {
            for (int x = 0; x < heights.GetLength(0); x++)
            {
                for (int y = 0; y < heights.GetLength(1); y++)
                {
                    if (heights[x, y] >= 0.4f)
                    {
                        if (heights[x, y] >= 0.7f)
                        {
                            if (heights[x, y] < 0.8f)
                                chunkInfo.Add(positions[x, y], GenerateTerrain.Biome.montagneLow);
                            else if (heights[x, y] < 0.85f)
                                chunkInfo.Add(positions[x, y], GenerateTerrain.Biome.montagneMedium);
                            else
                                chunkInfo.Add(positions[x, y], GenerateTerrain.Biome.montagneHigh);
                        }
                        else if (heights[x, y] < 0.7f && heights[x, y] >= 0.45f)
                            chunkInfo.Add(positions[x, y], GenerateTerrain.Biome.plaine);
                        else
                            chunkInfo.Add(positions[x, y], GenerateTerrain.Biome.plage);
                    }
                    else if (heights[x, y] != 0f)
                    {
                        chunkInfo.Add(positions[x, y], GenerateTerrain.Biome.ocean);
                    }
                    else
                    {
                        chunkInfo.Add(positions[x, y], GenerateTerrain.Biome.vide);
                    }
                }
            }
        }

        //On génére dans la scene les tiles en fonction de leur biome
        public void GenerateBiomes()
        {
            Vector2 offset = new Vector2(Random.Range(-100000, -100000), Random.Range(-100000, -100000));
            for (int i = 0; i < positions.GetLength(0); i++)
            {
                for (int j = 0; j < positions.GetLength(1); j++)
                {
                    switch (chunkInfo[positions[i, j]])
                    {
                        case GenerateTerrain.Biome.vide:
                            break;
                        case GenerateTerrain.Biome.ocean:
                            GameObject cube0 = GameObject.Instantiate(ocean, new Vector3(precision * positions[i, j].x, 0, precision * positions[i, j].z), Quaternion.identity, chunk.transform);
                            cube0.GetComponent<Renderer>().material.color = new Color(0, 0, 1);
                            cube0.transform.localScale = new Vector3(precision, precision, precision);
                            cube0.GetComponent<Waves_modifier>().tileChunksPos = new Vector3(xCoord, 0, yCoord);
                            cube0.GetComponent<Waves_modifier>().tileInChunk = new Vector3(i, 0, j);
                            cube0.GetComponent<Waves_modifier>().offset = new Vector2(positions[i, j].x, positions[i, j].z);
                            chunkGO.Add(new Vector3(precision * positions[i, j].x, 0, precision * positions[i, j].z), cube0);
                            break;
                        case GenerateTerrain.Biome.plage:
                            GameObject cube1 = GameObject.Instantiate(plage, new Vector3(precision * positions[i, j].x, 0, precision * positions[i, j].z), Quaternion.identity, chunk.transform);
                            cube1.GetComponent<Renderer>().material.color = new Color(1, 1, 0);
                            cube1.transform.localScale = new Vector3(precision, precision, precision);
                            chunkGO.Add(new Vector3(precision * positions[i, j].x, 0, precision * positions[i, j].z), cube1);
                            break;
                        case GenerateTerrain.Biome.plaine:
                            GameObject cube2 = GameObject.Instantiate(plaine, new Vector3(precision * positions[i, j].x, 0, precision * positions[i, j].z), Quaternion.identity, chunk.transform);
                            cube2.GetComponent<Renderer>().material.color = new Color(0, 1, 0);
                            cube2.transform.localScale = new Vector3(precision, precision, precision);
                            chunkGO.Add(new Vector3(precision * positions[i, j].x, 0, precision * positions[i, j].z), cube2);
                            break;
                        case GenerateTerrain.Biome.montagneLow:
                            GameObject cube3 = GameObject.Instantiate(montagneLow, new Vector3(precision * positions[i, j].x, 0, precision * positions[i, j].z), Quaternion.identity, chunk.transform);
                            cube3.GetComponent<Renderer>().material.color = new Color(0, 0, 0);
                            cube3.transform.localScale = new Vector3(precision, precision, precision);
                            chunkGO.Add(new Vector3(precision * positions[i, j].x, 0, precision * positions[i, j].z), cube3);
                            break;
                        case GenerateTerrain.Biome.montagneMedium:
                            GameObject cube4 = GameObject.Instantiate(montagneMedium, new Vector3(precision * positions[i, j].x, 0, precision * positions[i, j].z), Quaternion.identity, chunk.transform);
                            cube4.GetComponent<Renderer>().material.color = new Color(0, 0, 0);
                            cube4.transform.localScale = new Vector3(precision, precision, precision);
                            chunkGO.Add(new Vector3(precision * positions[i, j].x, 0, precision * positions[i, j].z), cube4);
                            break;
                        case GenerateTerrain.Biome.montagneHigh:
                            GameObject cube5 = GameObject.Instantiate(montagneHigh, new Vector3(precision * positions[i, j].x, 0, precision * positions[i, j].z), Quaternion.identity, chunk.transform);
                            cube5.GetComponent<Renderer>().material.color = new Color(0, 0, 0);
                            cube5.transform.localScale = new Vector3(precision, precision, precision);
                            chunkGO.Add(new Vector3(precision * positions[i, j].x, 0, precision * positions[i, j].z), cube5);
                            break;
                        default:
                            break;
                    }
                }
            }
            foreach (Island isle in islands)
            {
                isle.Init(chunkGO);
            }
        }
    }

    [SerializeField]
    public class Island
    {
        public string ID; //unique ID 
        public string chunkID;

        public int x; // World x position
        public int z; // World z position

        int minX;
        int maxX;
        int minZ;
        int maxZ;

        //tiles de l'ile
        public Vector3[] tiles;
        GameObject[] tilesGO;

        GameObject isle;
        BoxCollider collider;

        public GenerateWorld generateWorld;

        public Island(int islandNumber, Vector3[] tiles, Transform parent, string chunkID)
        {
            generateWorld = GameObject.Find("Main Camera").GetComponent<GenerateWorld>();
            isle = new GameObject();
            isle.transform.parent = parent;
            this.tiles = tiles;
            minX = minZ = int.MaxValue;
            maxX = maxZ = int.MinValue;
            GenerateCollider();
            this.chunkID = chunkID;
            ID = chunkID + " Island (" + x + ":" + z + ")";
            isle.name = ID;
            isle.layer = LayerMask.NameToLayer("islandHitBox");
        }

        public void Init(Dictionary<Vector3, GameObject> chunkGO)
        {
            tilesGO = new GameObject[this.tiles.Length];
            for (int i = 0; i < this.tiles.Length; i++)
            {
                tilesGO[i] = chunkGO[this.tiles[i]];
                tilesGO[i].transform.parent = isle.transform;
            }
        }

        //On créé un collider qui va englober l'ile
        public void GenerateCollider()
        {
            foreach (Vector3 tile in tiles)
            {
                if (tile.x < minX)
                {
                    minX = (int)tile.x;
                }
                if (tile.z < minZ)
                {
                    minZ = (int)tile.z;
                }
                if (tile.x > maxX)
                {
                    maxX = (int)tile.x;
                }
                if (tile.z > maxZ)
                {
                    maxZ = (int)tile.z;
                }
            }
            int middleX = (maxX + minX) / 2;
            int middleZ = (maxZ + minZ) / 2;

            x = middleX;
            z = middleZ;

            collider = isle.AddComponent<BoxCollider>();
            collider.center = new Vector3(x, 0, z);
            collider.isTrigger = true;
            collider.size = new Vector3(maxX - minX + generateWorld.precision, 30f, maxZ - minZ + generateWorld.precision);
        }
    }

    public class Bridge
    {
        Island beginIsland;
        Island endIsland;

        GameObject bridge;
        GameObject partOfBridge;

        List<GameObject> partsOfBridge;

        GenerateWorld generateWorld;
        GameObject parent;

        public Bridge(Island begin, Island end, GameObject part, GameObject parent)
        {
            beginIsland = begin;
            endIsland = end;
            partOfBridge = part;
            generateWorld = GameObject.Find("Main Camera").GetComponent<GenerateWorld>();
            partsOfBridge = new List<GameObject>();
            this.parent = parent;
        }

        public void GenerateBridge()
        {
            //Easy way
            /*bridge = new GameObject(beginIsland.ID + "==" + endIsland.ID);
            bridge.transform.parent = parent.transform;
            Vector3 begin;
            Vector3 end;
            begin = new Vector3(beginIsland.x, 0, beginIsland.z);
            end = new Vector3(endIsland.x, 0, endIsland.z);
            Debug.Log(begin.ToString());
            Debug.Log(end.ToString());

            Vector3 middle = (begin + end) / 2f;

            float lenght = Vector3.Distance(begin, end);

            GameObject part = GameObject.Instantiate<GameObject>(partOfBridge, middle, Quaternion.identity, bridge.transform);
            part.transform.forward = part.transform.position - begin;
            part.transform.position += new Vector3(0, -0.05f * generateWorld.precision, 0);
            part.transform.localScale = new Vector3(generateWorld.precision, generateWorld.precision , lenght);
            partsOfBridge.Add(part);*/

            //Hard way
            bridge = new GameObject(beginIsland.ID + "==" + endIsland.ID);
            bridge.transform.parent = parent.transform;
            Vector3 begin = Vector3.zero;
            Vector3 end = Vector3.zero;
            begin = new Vector3(beginIsland.x, 0, beginIsland.z);
            end = new Vector3(endIsland.x, 0, endIsland.z);

            //Find closest tiles to both bounds
            Vector3 tileBeginRdm = beginIsland.tiles[0];
            Vector3 tileEndRdm = endIsland.tiles[0];

            float minDist = float.MaxValue;
            foreach (Vector3 tile in beginIsland.tiles)
            {
                if (Vector3.Distance(tile, tileEndRdm) < minDist)
                {
                    minDist = Vector3.Distance(tile, tileEndRdm);
                    begin = tile;
                }
            }
            minDist = float.MaxValue;
            foreach (Vector3 tile in endIsland.tiles)
            {
                if (Vector3.Distance(tile, begin) < minDist)
                {
                    minDist = Vector3.Distance(tile, begin);
                    end = tile;
                }
            }


            //Converts coord to a smaller array
            /*if (end.z < begin.z)
            {
                begin += end;
                end = begin - end;
                begin -= end;
            }*/


            //Begin is now equal to Vector2(0,0)
            float xToX = begin.x / generateWorld.precision;
            float zToZ = begin.z / generateWorld.precision;
            Vector2 start = new Vector2((begin.x / generateWorld.precision) - xToX + 1, (begin.z / generateWorld.precision) - zToZ + 1);
            Vector2 fin = new Vector2((end.x / generateWorld.precision) - xToX + 1, (end.z / generateWorld.precision) - zToZ + 1);

            int width = (int)Mathf.Abs(start.x - fin.x);
            int lenght = (int)Mathf.Abs(start.y - fin.y);

            /*List<Vector2> pathCoord = new List<Vector2>();
            float[,] grid = new float[width, lenght];

            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    Debug.Log("Grid (" + i + "," + j + ")");
                }
            }*/

            Node startN = new Node((int)start.x, (int)start.y, null);
            Node finN = new Node((int)fin.x, (int)fin.y, null);

            Debug.Log("Starting AStar");
            AStarAlgorithm pathfinding = new AStarAlgorithm();
            Node[] nodes = pathfinding.AStar(startN, finN);

            //On reconverti toutes les nodes en coordonnées du monde
            Vector3[] tilesCoord = new Vector3[nodes.Length];
            for (int i = 0; i < nodes.Length; i++)
            {
                tilesCoord[i] = new Vector3((nodes[i].position.x - 1 + xToX) * generateWorld.precision, 0, (nodes[i].position.y - 1 + zToZ) * generateWorld.precision);
            }

            Debug.Log("Generating bridge");
            //On créé les tiles 
            Chunk chunkStart = null;
            Chunk chunkEnd = null;
            Dictionary<Vector3, Chunk>.KeyCollection keysChunks = generateWorld.chunks.Keys;
            foreach (Vector3 chunk in keysChunks)
            {
                if (generateWorld.chunks[chunk].chunkID == beginIsland.chunkID)
                {
                    chunkStart = generateWorld.chunks[chunk];
                }
                if (generateWorld.chunks[chunk].chunkID == endIsland.chunkID)
                {
                    chunkEnd = generateWorld.chunks[chunk];
                }
            }
            if (chunkStart != null && chunkEnd != null)
            {
                if (chunkStart.chunkID == chunkEnd.chunkID)
                {
                    for (int i = 0; i < nodes.Length; i++)
                    {
                        if (!chunkStart.chunkGO.ContainsKey(tilesCoord[i]))
                       {
                            if (tilesCoord[i].x <= (chunkStart.width * chunkStart.xCoord + chunkStart.width) * generateWorld.precision && tilesCoord[i].x >= chunkStart.width * chunkStart.xCoord * generateWorld.precision
                                && tilesCoord[i].z <= (chunkStart.height * chunkStart.yCoord + chunkStart.height) * generateWorld.precision && tilesCoord[i].z >= chunkStart.height * chunkStart.yCoord * generateWorld.precision)
                            {

                                GameObject tile = GameObject.Instantiate(partOfBridge, tilesCoord[i], Quaternion.identity, bridge.transform);
                                partsOfBridge.Add(tile);
                                chunkStart.chunkGO.Add(tilesCoord[i], tile);
                                continue;
                            }
                            else
                            {
                                bool contains = true;
                                foreach (Vector3 chunk in keysChunks)
                                {
                                    Chunk chunkATM = generateWorld.chunks[chunk];
                                    if (chunk != new Vector3(chunkStart.xCoord, 0, chunkStart.yCoord) && !generateWorld.chunks[chunk].chunkGO.ContainsKey(tilesCoord[i])
                                        && tilesCoord[i].x <= (chunkATM.width * chunkATM.xCoord + chunkATM.width) * generateWorld.precision && tilesCoord[i].x >= chunkATM.width * chunkATM.xCoord * generateWorld.precision
                                        && tilesCoord[i].z <= (chunkATM.height * chunkATM.yCoord + chunkATM.height) * generateWorld.precision && tilesCoord[i].z >= chunkATM.height * chunkATM.yCoord * generateWorld.precision)

                                    {
                                        contains = false;
                                        break;
                                    }
                                }
                                if (!contains)
                                {
                                    GameObject tile = GameObject.Instantiate(partOfBridge, tilesCoord[i], Quaternion.identity, bridge.transform);
                                    partsOfBridge.Add(tile);
                                    chunkStart.chunkGO.Add(tilesCoord[i], tile);
                                    continue;
                                }
                            }
                        }  
                    }
                }
                else
                {
                    for (int i = 0; i < nodes.Length; i++)
                    {
                        if (!chunkStart.chunkGO.ContainsKey(tilesCoord[i])
                            && tilesCoord[i].x <= ((chunkStart.width * chunkStart.xCoord) + chunkStart.width) * generateWorld.precision && tilesCoord[i].x >= chunkStart.width * chunkStart.xCoord * generateWorld.precision
                            && tilesCoord[i].z <= ((chunkStart.height * chunkStart.yCoord) + chunkStart.height) * generateWorld.precision && tilesCoord[i].z >= chunkStart.height * chunkStart.yCoord * generateWorld.precision)
                        {
                            GameObject tile = GameObject.Instantiate(partOfBridge, tilesCoord[i], Quaternion.identity, bridge.transform);
                            partsOfBridge.Add(tile);
                            chunkStart.chunkGO.Add(tilesCoord[i], tile);
                            continue;
                        }
                        else if (!chunkEnd.chunkGO.ContainsKey(tilesCoord[i])
                            && tilesCoord[i].x <= (chunkEnd.width * chunkEnd.xCoord + chunkEnd.width) * generateWorld.precision && tilesCoord[i].x >= chunkEnd.width * chunkEnd.xCoord * generateWorld.precision
                            && tilesCoord[i].z <= (chunkEnd.height * chunkEnd.yCoord + chunkEnd.height) * generateWorld.precision && tilesCoord[i].z >= chunkEnd.height * chunkEnd.yCoord * generateWorld.precision)
                        {
                            GameObject tile = GameObject.Instantiate(partOfBridge, tilesCoord[i], Quaternion.identity, bridge.transform);
                            partsOfBridge.Add(tile);
                            chunkEnd.chunkGO.Add(tilesCoord[i], tile);
                            continue;
                        }
                        else
                        {
                            bool contains = true;
                            foreach (Vector3 chunk in keysChunks)
                            {
                                Chunk chunkATM = generateWorld.chunks[chunk];
                                if (chunk != new Vector3(chunkStart.xCoord, 0, chunkStart.yCoord) && chunk != new Vector3(chunkEnd.xCoord, 0, chunkEnd.yCoord) && !generateWorld.chunks[chunk].chunkGO.ContainsKey(tilesCoord[i])
                                    && tilesCoord[i].x <= (chunkATM.width * chunkATM.xCoord + chunkATM.width) * generateWorld.precision && tilesCoord[i].x >= chunkATM.width * chunkATM.xCoord * generateWorld.precision
                                    && tilesCoord[i].z <= (chunkATM.height * chunkATM.yCoord + chunkATM.height) * generateWorld.precision && tilesCoord[i].z >= chunkATM.height * chunkATM.yCoord * generateWorld.precision)
                                {
                                    contains = false;
                                    break;
                                }
                            }
                            if (!contains)
                            {
                                GameObject tile = GameObject.Instantiate(partOfBridge, tilesCoord[i], Quaternion.identity, bridge.transform);
                                partsOfBridge.Add(tile);
                                chunkStart.chunkGO.Add(tilesCoord[i], tile);
                                continue;
                            }
                        }
                    }
                }
            }
            Debug.Log("bridge Done");
        }
    }
}

