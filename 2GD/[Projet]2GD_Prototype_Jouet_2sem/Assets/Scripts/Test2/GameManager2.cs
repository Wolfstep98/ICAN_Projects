using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager2 : MonoBehaviour {

    public enum GameType
    {
        normal,
        rotateCustom,
        twoPlayers,
        destructiveSphere,
        lighting,
        sphereMovable,
        timing,
        recoltables,
        lien
    }
    [Header("Game Type")]
    public GameType gameType = GameType.normal;

    
    public enum WallType
    {
        bounce,
        wrap
    }
    [Header("Wall behaviour")]
    public WallType wallType = WallType.bounce;
    WallType ancienWallType = WallType.bounce;

    //[Header("Moving sphere scene")]
    //public bool sphereNotMoving = true;

    [Header("GameObjects")]
    public GameObject sunSource;
    public GameObject[] circles;
    public GameObject ball;
    public GameObject[] spawners;
    public GameObject[] walls;
    public GameObject[] players;
    public GameObject[] playersPrefab;

    [Header("Other")]
    public bool auraActive;
    public float distanceUpDown;
    public float distanceRightLeft;
    public float auraTime;
    public float auraTimeRemain;
    public float timeBeforeRespawn = 2f;


    public Vector2[] playersSpawnPos;
    public Text auraTimeRemaining;

    public int nbrDeLienMax = 2;

	// Use this for initialization
	void Awake () {
        spawners = GameObject.FindGameObjectsWithTag("Spawner");
        walls = GameObject.FindGameObjectsWithTag("Mur");
        players = GameObject.FindGameObjectsWithTag("Player");
        playersPrefab = Resources.LoadAll<GameObject>("Prefabs/Final/Balls");
        circles = GameObject.FindGameObjectsWithTag("Circle");
        Vector3 upPos, downPos, leftPos, rightPos ;
        upPos = downPos = leftPos = rightPos = Vector3.zero;
        foreach (GameObject mur in walls)
        {
            if(mur.name == "MurHaut")
            {
                upPos = mur.transform.position;
            }
            else if (mur.name == "MurdBas")
            {
                downPos = mur.transform.position;
            }
            else if (mur.name == "MurGauche")
            {
                leftPos = mur.transform.position;
            }
            else if (mur.name == "MurDroit")
            {
                rightPos = mur.transform.position;
            }
        }
        distanceUpDown = Mathf.Abs(Vector3.Distance(upPos, downPos));
        distanceRightLeft = Mathf.Abs(Vector3.Distance(rightPos, leftPos));

        switch (gameType)
        {
            case GameType.normal:
                foreach (GameObject circle in circles)
                {
                    CircleBehaviours script = circle.GetComponent<CircleBehaviours>();
                    script.isRotating = true;
                }
                break;
            case GameType.rotateCustom:
                foreach (GameObject circle in circles)
                {
                    CircleBehaviours script = circle.GetComponent<CircleBehaviours>();
                    script.isRotatingTyToPlayer = true;
                }
                break;
            case GameType.recoltables:
                break;
            case GameType.twoPlayers:
                foreach (GameObject player in players)
                {
                    PlayerBehaviours script = player.GetComponent<PlayerBehaviours>();
                    script.isMulti = true;
                }
                break;
            case GameType.destructiveSphere:
                foreach (GameObject circle in circles)
                {
                    CircleBehaviours script = circle.GetComponent<CircleBehaviours>();
                    script.isExploding = true;
                    script.isRotating = true;
                }
                break;
            case GameType.lighting:
                sunSource.SetActive(false);
                foreach(GameObject circle in circles)
                {
                    CircleBehaviours script = circle.GetComponent<CircleBehaviours>();
                    script.isLighting = true;
                    script.isRotating = true;
                }
                break;
            case GameType.sphereMovable:
                foreach (GameObject circle in circles)
                {
                    CircleBehaviours script = circle.GetComponent<CircleBehaviours>();
                    script.isInteractible = true;
                    script.isRotating = true;
                }
                break;
            case GameType.timing:
                foreach (GameObject circle in circles)
                {
                    CircleBehaviours script = circle.GetComponent<CircleBehaviours>();
                    script.isTiming = true;
                    script.isRotating = true;
                }
                break;
            case GameType.lien:
                break;
            default:
                break;
        }
        if (gameType == GameType.recoltables)
            auraTimeRemaining = GameObject.Find("TimeRemain").GetComponent<Text>();
        foreach(GameObject circle in circles)
        {
            circle.GetComponent<CircleBehaviours>().nbrDeLienMax = nbrDeLienMax;
        }

    }
	
	// Update is called once per frame
	void Update ()
    {

        if (gameType == GameType.recoltables)
            auraTimeRemaining.text = auraTimeRemain.ToString();
        if (wallType != ancienWallType)
        {
            for (int i = 0; i < walls.Length; i++)
            {
                if (wallType == WallType.bounce)
                    walls[i].GetComponent<BoxCollider2D>().isTrigger = false;
                else if (wallType == WallType.wrap)
                    walls[i].GetComponent<BoxCollider2D>().isTrigger = true;
            }
        }
        ancienWallType = wallType;
        if(Input.GetKey(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if(auraActive)
        {
            auraTimeRemain -= Time.deltaTime;
            if(auraTimeRemain <= 0)
            {
                GameObject auraVerte = GameObject.Find("Aura_Green");
                GameObject auraRouge = GameObject.Find("Aura_Red");
                if(auraVerte != null)
                {
                    PlayerBehaviours script = auraVerte.GetComponentInParent<PlayerBehaviours>();
                    script.points++;
                    Destroy(script.aura);
                    script.aura = null;
                    script.auraStoled = false;
                }
                else
                {
                    PlayerBehaviours script = auraRouge.GetComponentInParent<PlayerBehaviours>();
                    script.points--;
                    Destroy(script.aura);
                    script._StopCoroutine();
                    script.aura = null;
                    script.auraStoled = false;  
                }
                for (int i = 0; i < players.Length; i++)
                {
                    players[i].GetComponent<PlayerBehaviours>()._StopCoroutine();
                }
                int rdmNbr = Random.Range(0, spawners.Length - 1);
                GameObject recoltable = (GameObject)Instantiate(Resources.Load("Prefabs/Test/Recoltable"),spawners[rdmNbr].transform.position, Quaternion.identity);
                recoltable.name = "Recoltable";
                auraTimeRemain = 0;
                auraActive = false;
            }
        }
    }

    public void AuraTimer()
    {
        auraActive = true;
        auraTimeRemain = auraTime;
    }

    public void OnPlayerDestroyed(PlayerBehaviours playerBehaviour, GameObject destroyer)
    {
        StartCoroutine(OnPlayerDestroy(playerBehaviour, destroyer));
    }

    IEnumerator OnPlayerDestroy(PlayerBehaviours playerBehaviour,GameObject destroyer)
    {
        Debug.Log("Fonction OnPlayerDestroy called");
        PlayerBehaviours.Player player = playerBehaviour.player;
        int playerIndex = -1;
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == playerBehaviour.gameObject)
            {
                playerIndex = i;
                players[i] = null;
            }
        }
        yield return new WaitForSeconds(timeBeforeRespawn);
        Debug.Log("Fonction OnPlayerDestroy continue");
        //foreach(GameObject circle in circles)
        //{
        //    circle.GetComponent<Renderer>().material.color = Color.white;
        //}
        GameObject farthestSpawner = FarthestSpawner(destroyer.transform.position);
        GameObject playerPrefab = FindPlayerPrefab(player);
        GameObject playerInstance = Instantiate<GameObject>(playerPrefab, farthestSpawner.transform.position, Quaternion.identity);
        if (playerIndex >= 0)
            players[playerIndex] = playerInstance;
    }

    float CheckDistanceBetweenSpawn(Vector2 position, GameObject spawn)
    {
        float distance = Vector2.Distance(position, spawn.transform.position);
        return distance;
    }

    GameObject FarthestSpawner(Vector2 position)
    {
        GameObject result = null;
        float maxDistance = 0f;
        foreach(GameObject spawn in spawners)
        {
            float distance = CheckDistanceBetweenSpawn(position, spawn);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                result = spawn;
            }
        }
        return result;
    }

    GameObject FindPlayerPrefab(PlayerBehaviours.Player player)
    {
        GameObject playerPrefab = null;
        for(int i = 0; i< playersPrefab.Length;i++)
        {
            if(playersPrefab[i].GetComponent<PlayerBehaviours>().player == player)
            {
                playerPrefab = playersPrefab[i];
                break;
            }
        }
        return playerPrefab;
    }
}
