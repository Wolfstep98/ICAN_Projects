using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using World;

public class Player : MonoBehaviour {

    public Camera mainCam;

    public GenerateWorld generateWorld;
    public BridgeConstructor bridgeConstructor;

    public Vector3 playerChunksPos;

    //Island check
    public string currentIsland;

    public string islandToSearchFor;

    //Path
    public string lastLookingAt;
    public string islandLookingAt;
    public float timeBeforeBridge = 5f;
    public float timeLookingAt;

    // Use this for initialization
    void Start () {
        bridgeConstructor = GameObject.Find("Bridges").GetComponent<BridgeConstructor>();
        generateWorld = GameObject.Find("Main Camera").GetComponent<GenerateWorld>();

        playerChunksPos = new Vector3(Mathf.CeilToInt(transform.position.x / (generateWorld.width * generateWorld.precision)) - 1, 0, Mathf.CeilToInt(transform.position.z / (generateWorld.height * generateWorld.precision)) - 1);

        currentIsland = "";
        islandToSearchFor = "";
        islandLookingAt = "";
        timeLookingAt = 0f;

        mainCam = GameObject.Find("MainCamera").GetComponent<Camera>();
    }
	
	// Update is called once per frame
	void Update () {
        playerChunksPos = new Vector3(Mathf.CeilToInt(transform.position.x / (generateWorld.width * generateWorld.precision)) - 1, 0, Mathf.CeilToInt(transform.position.z / (generateWorld.height * generateWorld.precision)) - 1);
        Ray ray = new Ray(mainCam.transform.position,mainCam.transform.forward);
        RaycastHit hitInfos = new RaycastHit();
        Debug.DrawLine(mainCam.transform.position, mainCam.transform.forward * 5000f,Color.red);
        if (Physics.Raycast(ray, out hitInfos, 5000f, 1 << LayerMask.NameToLayer("islandHitBox")))
        {
            islandLookingAt = hitInfos.collider.gameObject.name;
        }
        else
        {
            islandLookingAt = "";
        }
        if(islandLookingAt != "" && lastLookingAt == islandLookingAt && currentIsland != "")
        {
            timeLookingAt += Time.deltaTime;
            if (timeLookingAt >= timeBeforeBridge)
            {
                //Construct the bridge
                Island beginIsle = null;
                Island endIsle = null;

                beginIsle = generateWorld.chunks[playerChunksPos].islands.Find(isle =>
                {
                    return (isle.ID == currentIsland) ? true : false;
                });
                Dictionary<Vector3, Chunk>.KeyCollection keys = generateWorld.chunks.Keys;
                foreach (Vector3 key in keys)
                {
                    endIsle = generateWorld.chunks[key].islands.Find(isle =>
                    {
                        return (isle.ID == islandLookingAt) ? true : false;
                    });
                    if (endIsle != null)
                        break;
                }

                if (beginIsle != null && endIsle != null)
                {
                    bridgeConstructor.GenerateBridge(beginIsle, endIsle);
                    Debug.Log("Construct");
                    timeLookingAt = 0;
                }
            }
        }
        else
        {
            timeLookingAt = 0f;
        }
        lastLookingAt = islandLookingAt;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("islandHitBox"))
        {
            islandToSearchFor = "Chunk (" + playerChunksPos.x + ":" + playerChunksPos.z + ") Island (" + other.GetComponent<BoxCollider>().center.x+ ":" + other.GetComponent<BoxCollider>().center.z + ")";
            Island island = generateWorld.chunks[playerChunksPos].islands.Find(isle =>
            {
                Debug.Log(isle.ID + " == " + islandToSearchFor);
                return (isle.ID == islandToSearchFor) ? true : false;
            });
            if(island != null)
                currentIsland = island.ID;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("islandHitBox"))
        {
            islandToSearchFor = "Chunk (" + playerChunksPos.x + ":" + playerChunksPos.z + ") Island (" + other.GetComponent<BoxCollider>().center.x + ":" + other.GetComponent<BoxCollider>().center.z + ")";
            Island island = generateWorld.chunks[playerChunksPos].islands.Find(isle =>
            {
                return (isle.ID == islandToSearchFor);
            });
            if (island != null)
                currentIsland = island.ID;
        }
    }
    private void OnTriggerExit (Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("islandHitBox"))
        {
            currentIsland = "";
        }
    }
}
