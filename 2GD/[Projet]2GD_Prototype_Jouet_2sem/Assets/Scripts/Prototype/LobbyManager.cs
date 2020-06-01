using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviour {


    //Game informations
    public int nbrOfPlayers;

    //Level Selection informations
    public bool startColldown;
    public float timeBeforeMaps = 3.0f;
    [ReadOnly]
    public float timeBeforeMapsleft = 0.0f;

    //Cirles
    public Vector2[] spawnPoints;
    public GameObject[] playersPrefabs;
    public GameObject[] players;
    public GameObject[] circles;
    public GameObject[] levelSelector;

    public Dictionary<GameObject, int> levelSelectorIndexes;

    //References
    public MainGameManager mainGameManager;


    private void Awake()
    {
        mainGameManager = GameObject.Find("MainGameManager").GetComponent<MainGameManager>();

        timeBeforeMapsleft = 0.0f;

        playersPrefabs = Resources.LoadAll<GameObject>("Prefabs/Final/2D/Balls");
        spawnPoints = new Vector2[4]
        {
            new Vector2(-0.4f,-3.4f),
            new Vector2(-0.1f,-3.4f),
            new Vector2(0.1f,-3.4f),
            new Vector2(0.4f,-3.4f)
        };
        levelSelector = GameObject.FindGameObjectsWithTag("LevelSelector");
        levelSelectorIndexes = new Dictionary<GameObject, int>();
        levelSelectorIndexes.Add(GameObject.Find("CircleSprite10"), 3);
        levelSelectorIndexes.Add(GameObject.Find("CircleSprite11"), 4);
        levelSelectorIndexes.Add(GameObject.Find("CircleSprite12"), 5);
    }

    public void Initialise(int nbrOfPlayers)
    {
        this.nbrOfPlayers = nbrOfPlayers;
        SpawnPlayers();
        foreach(GameObject circle in levelSelector)
        {
            Circle script = circle.GetComponentInChildren<Circle>();
            script.isDiscolored = true;
            script.canBeDiscolored = true;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        foreach (GameObject circle in levelSelector)
        {
            Circle script = circle.GetComponentInChildren<Circle>();
            script.isDiscolored = true;
            script.canBeDiscolored = true;
            script.color = GameManager.GameColor.White;
            script.animator.SetInteger(script.circleColorAnimParam, (int)script.color);
        }

        //Check if all the players are on the levels selector
        if (!startColldown)
        {
            int tempNbrOfPlayer = 0;
            foreach (GameObject circle in levelSelector)
            {
                Circle script = circle.GetComponentInChildren<Circle>();
                tempNbrOfPlayer += script.nbrOfPlayerOnCircle;
            }
            if (tempNbrOfPlayer == nbrOfPlayers)
            {
                startColldown = true;
                timeBeforeMapsleft = timeBeforeMaps;
            }
        }
        else
        {
            timeBeforeMapsleft -= Time.deltaTime;
            int tempNbrOfPlayer = 0;
            foreach (GameObject circle in levelSelector)
            {
                Circle script = circle.GetComponentInChildren<Circle>();
                tempNbrOfPlayer += script.nbrOfPlayerOnCircle;
            }
            if(tempNbrOfPlayer < nbrOfPlayers)
            {
                startColldown = false;
                timeBeforeMapsleft = 0.0f;
            }
            else if(timeBeforeMapsleft <= 0.0f)
            {
                switch(nbrOfPlayers)
                {
                    case 2:
                        GameObject[] circles = new GameObject[2];
                        int i = 0;
                        foreach(GameObject circle in levelSelector)
                        {
                            Circle script = circle.GetComponentInChildren<Circle>();
                            if(script.nbrOfPlayerOnCircle == 1)
                            {
                                if(script.nbrOfPlayerOnCircle == 1)
                                {
                                    circles[i] = circle;
                                    i++;
                                }
                            }
                            else if(script.nbrOfPlayerOnCircle == 2)
                            {
                                mainGameManager.ChangeGameScene(levelSelectorIndexes[circle]);
                            }
                        }
                        if (i != 0)
                        {
                            int randomNbr = Random.Range(0, circles.Length);
                            mainGameManager.ChangeGameScene(levelSelectorIndexes[circles[randomNbr]]);
                        }
                        break;
                    case 3:
                        {
                            int sum = 0;
                            foreach (GameObject circle in levelSelector)
                            {
                                Circle script = circle.GetComponentInChildren<Circle>();
                                int randomValue = Random.Range(1, 91);
                                int value = script.nbrOfPlayerOnCircle * 30;
                                sum += value;
                                if (sum < 90 && randomValue < value)
                                {
                                    mainGameManager.ChangeGameScene(levelSelectorIndexes[circle]);
                                    break;
                                }
                                else if (sum == 90)
                                {
                                    mainGameManager.ChangeGameScene(levelSelectorIndexes[circle]);
                                    break;
                                }
                            }
                        }
                        break;
                    case 4:
                        {
                            int sum = 0;
                            foreach (GameObject circle in levelSelector)
                            {
                                Circle script = circle.GetComponentInChildren<Circle>();
                                int randomValue = Random.Range(1, 101);
                                int value = script.nbrOfPlayerOnCircle * 25;
                                sum += value;
                                if (sum < 100 && randomValue < value)
                                {
                                    mainGameManager.ChangeGameScene(levelSelectorIndexes[circle]);
                                    break;
                                }
                                else if (sum == 100)
                                {
                                    mainGameManager.ChangeGameScene(levelSelectorIndexes[circle]);
                                    break;
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
                mainGameManager.ChangeGameState(5);
            }
        }
    }

    public void SpawnPlayers()
    {
        for (int i = 0; i < nbrOfPlayers; i++)
        {
            Player.PlayerNumber playerNumber = (Player.PlayerNumber)i + 1;
            if (!mainGameManager.playersPlaying[playerNumber])
                playerNumber++;
            GameObject playerPrefab = FindPlayerPrefab(playerNumber);
            GameObject playerInstance = Instantiate<GameObject>(playerPrefab, spawnPoints[i], Quaternion.identity);
            playerInstance.GetComponent<Player>().playerColor = mainGameManager.playersColor[playerNumber];
        }
    }

    GameObject FindPlayerPrefab(Player.PlayerNumber player)
    {
        GameObject playerGO = null;
        for (int i = 0; i < playersPrefabs.Length; i++)
        {
            if (playersPrefabs[i].GetComponent<Player>().player == player)
            {
                playerGO = playersPrefabs[i];
                break;
            }
        }
        return playerGO;
    }
}
