using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
//using UnityEditor;

public class GameManager : MonoBehaviour {

    #region Variables

    [Header("GameManager properties")]
    //Values
    public bool partyEnded;
    public bool isInLobby = false;
    public int numberOfPlayers;
    public float gameDuration = 120f;
    [ReadOnly]
    public float gameDurationLeft;
    public float timeBeforeRespawn = 2f;

	[FMODUnity.EventRef]
	public string hack_state;

	private FMOD.Studio.EventInstance hack_fmod;
    //Color Type
    public enum GameColor
    {
        White,
        Blue,
        LightBlue,
        Green,
        Red,
        Rose,
        Orange,
        Yellow
    }

    //Bonus
    public float timeBetweenBonusSwitches = 15f;
    [ReadOnly]
    public float timeBetweenBonusSwitchesLeft;

    //UI
    public bool isPressingInput = false;
    public float timeBeforeValidate = 1.0f;
    [ReadOnly]
    public float timeBeforeValidateLeft = 0.0f;
    public Text timerUI;
    public Image contourUI;


    //References
    public MainGameManager mainGameManager;
    public EventSystem eventSystem;
    public EventsSystem eventsSystem;
    public LienUpdater lienUpdater;
    public GameObject[] players;
    public GameObject[] playersPrefab;
    //All the circles
    public GameObject[] circles;
    public GameObject[] spawners;
    public GameObject[] bonus;

    public GameObject[] walls;
    public List<Link> links;

    //Game infos at the end
    public Dictionary<Player, bool> isPlayerAlive;
    public List<GameObject> winners;

    //UI pause
    public Button buttonContinue;
    public Button buttonOptions;
    public Button buttonQuitter;

    //UI End
    public Button buttonRecommencer;
    public Button buttonRetourMenu;

    #endregion

    private void Awake()
    {
        partyEnded = false;
        isPressingInput = false;

        timeBeforeValidateLeft = timeBeforeValidate;
        gameDurationLeft = gameDuration;

        lienUpdater = GetComponent<LienUpdater>();
        eventsSystem = GetComponent<EventsSystem>();
        playersPrefab = Resources.LoadAll<GameObject>("Prefabs/Final/2D/Balls");
        links = new List<Link>();
        winners = new List<GameObject>();
        isPlayerAlive = new Dictionary<Player, bool>();

        //Set the references
        walls = GameObject.FindGameObjectsWithTag("Mur");
        circles = GameObject.FindGameObjectsWithTag("Circle");
        //Don't do that when u build
        //players = GameObject.FindGameObjectsWithTag("Player");

        spawners = FindSpawns();
        bonus = FindBonus();

        //UI

        timerUI = GameObject.Find("TimerText").GetComponent<Text>();
        contourUI = GameObject.Find("Canvas").GetComponentInChildren<Image>();

        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();

    }

    public void Initialise(MainGameManager mainGameManager)
    {
        partyEnded = false;

        this.mainGameManager = mainGameManager;
        numberOfPlayers = mainGameManager.numberOfPlayers;


        //Instanciate the players and set the references
        if (!isInLobby)
        {
            InstanciatePlayers(mainGameManager);
            SetRandomSpawn();
            players = GameObject.FindGameObjectsWithTag("Player");
            SpawnPlayers();

            foreach (GameObject player in players)
            {
                Player script = player.GetComponent<Player>();
                isPlayerAlive.Add(script, true);
            }
        }
    }

    public void InstanciatePlayers(MainGameManager mainGameManager)
    {
        for (int i = 0; i < mainGameManager.numberOfPlayers; i++) 
        {
            Player.PlayerNumber playerNumber = (Player.PlayerNumber)i + 1;
            if (!mainGameManager.playersPlaying[playerNumber])
                playerNumber++;
            GameObject playerPrefab = FindPlayerPrefab(playerNumber);
            GameObject playerInstance = Instantiate<GameObject>(playerPrefab);
            playerInstance.GetComponent<Player>().playerColor = mainGameManager.playersColor[playerNumber];
            playerInstance.GetComponent<Player>().feedBackSpawn.GetComponent<Renderer>().material.color = GameColorToColor(mainGameManager.playersColor[playerNumber]);
            playerInstance.GetComponent<Player>().playerSprite = GetPlayerSprite(mainGameManager.playersColor[playerNumber]);
        }
    }

    void SetRandomSpawn()
    {
        for(int i = 0; i < numberOfPlayers;i++)
        {
            GameObject[] spawnersAvailable = GetAvailableSpawners();
            int randomIndex = Random.Range(0, spawnersAvailable.Length);
            Player.PlayerNumber playerNumber = (Player.PlayerNumber)i + 1;
            spawnersAvailable[randomIndex].GetComponent<Circle>().playerSpawn = playerNumber;
        }
    }

    void SpawnPlayers()
	{
        for (int i = 0; i < players.Length; i++)
        {
            //Spawn the player

            GameObject player = players[i];
            Vector2 spawnPoint = PickRandomPointAroundCircle(GetSpawnForPlayer(player.GetComponent<Player>().player), player);
            player.transform.position = spawnPoint;
        }
    }

    GameObject GetSpawnForPlayer(Player.PlayerNumber playerNumber)
    {
        GameObject spawner = null;
        foreach(GameObject spawn in spawners)
        {
            if(spawn.GetComponent<Circle>().playerSpawn == playerNumber)
            {
                spawner = spawn;
                break;
            }
        }
        return spawner;
    }

    GameObject[] GetAvailableSpawners()
    {
        List<GameObject> spawnersAvailable = new List<GameObject>();
        foreach(GameObject spawn in spawners)
        {
            Circle script = spawn.GetComponent<Circle>();
            if (script.playerSpawn == Player.PlayerNumber.None)
                spawnersAvailable.Add(spawn);
            if (isInLobby)
                spawnersAvailable.Add(spawn);
        }
        return spawnersAvailable.ToArray();
    }

    Vector2 PickRandomPointAroundCircle(GameObject circle, GameObject player)
    {
        //Spawn on the cirle
        return circle.transform.position;
        //Spawn around the circle
        //float rdmAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        //Vector2 circlePos = circle.transform.position;
        //float circleRadius = circle.GetComponent<CircleCollider2D>().radius * circle.transform.lossyScale.x;
        //float playerRadius = player.GetComponent<CircleCollider2D>().radius * player.transform.lossyScale.x;
        //Vector2 spawnPoint = new Vector2(
        //    circlePos.x + Mathf.Cos(rdmAngle) * (circleRadius + playerRadius),
        //    circlePos.y + Mathf.Sin(rdmAngle) * (circleRadius + playerRadius));
        //return spawnPoint;
    }

    private void Start()
    {
        if (!isInLobby)
        {
            hack_fmod = FMODUnity.RuntimeManager.CreateInstance(hack_state);
            hack_fmod.setVolume(0.25f);
            //Set up the UI
            timerUI.text = ((int)Mathf.Floor(gameDuration / 60f)).ToString() + " min " + ((int)Mathf.Floor(gameDuration % 60)).ToString() + " sec";

            //Start the bonus coroutine
            StartCoroutine(ActivateBonusOverTime());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInLobby)
        {

            //Update the UI
            gameDurationLeft -= Time.deltaTime;
            if (gameDurationLeft > 0f && !float.IsInfinity(gameDurationLeft))
                timerUI.text = ((int)Mathf.Floor(gameDurationLeft / 60f)).ToString() + " min " + ((int)Mathf.Floor(gameDurationLeft % 60)).ToString() + " sec";
            else if (float.IsInfinity(gameDurationLeft))
            {
                timerUI.text = "Infinity";
            }
            else if (!partyEnded && gameDurationLeft <= 0f)
            {
                partyEnded = true;
                gameDurationLeft = 0f;
                timerUI.text = ((int)Mathf.Floor(gameDurationLeft / 60f)).ToString() + " min " + ((int)Mathf.Floor(gameDurationLeft % 60)).ToString() + " sec";
                EndParty();

            }

            foreach (GameObject player in players)
            {
                Player script = player.GetComponent<Player>();
                if (isPlayerAlive[script] && script.nbrOfCircleColored == circles.Length - bonus.Length)
                {
                    bool onlyPlayerAlive = true;
                    foreach (Player key in isPlayerAlive.Keys)
                    {
                        if (key != script && isPlayerAlive[key])
                        {
                            onlyPlayerAlive = false;
                            break;
                        }
                    }
                    if (onlyPlayerAlive)
                        gameDurationLeft = 0f;
                }
            }

            Player playerWithTheMoreCircle = null;
            bool twoOrMorePlayerWithSameNbr = false;
            foreach (GameObject player in players)
            {
                Player script = player.GetComponent<Player>();
                if (playerWithTheMoreCircle == null)
                {
                    playerWithTheMoreCircle = script;
                }
                else
                {
                    if (playerWithTheMoreCircle.nbrOfCircleColored < script.nbrOfCircleColored)
                    {
                        playerWithTheMoreCircle = script;
                        twoOrMorePlayerWithSameNbr = false;
                    }
                    else if (playerWithTheMoreCircle.nbrOfCircleColored == script.nbrOfCircleColored)
                    {
                        twoOrMorePlayerWithSameNbr = true;
                    }
                }
            }
            if (!twoOrMorePlayerWithSameNbr && playerWithTheMoreCircle != null)
                contourUI.color = GameColorToColor(playerWithTheMoreCircle.playerColor);
            else
                contourUI.color = Color.white;

            //Pause Menu
            if (mainGameManager.currentGameState == MainGameManager.GameState.IsPaused)
            {
                if (Input.GetButtonDown(FindPlayer(Player.PlayerNumber.J1).GetComponent<Player>().playerInput))
                {
                    isPressingInput = true;
                    timeBeforeValidateLeft = timeBeforeValidate;
                }
                if (isPressingInput)
                {
                    timeBeforeValidateLeft -= Time.fixedUnscaledDeltaTime;
                }
                if ((Input.GetButtonUp(FindPlayer(Player.PlayerNumber.J1).GetComponent<Player>().playerInput) && timeBeforeValidateLeft <= 0.0f) || timeBeforeValidateLeft <= 0.0f)
                {
                    BaseEventData baseEventData = new BaseEventData(eventSystem);
                    bool flag = ExecuteEvents.Execute<ISubmitHandler>(eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.submitHandler);
                    if (flag)
                        Debug.Log("Event Selected");
                    timeBeforeValidateLeft = timeBeforeValidate;
                    isPressingInput = false;
                }
                else if ((Input.GetButtonUp(FindPlayer(Player.PlayerNumber.J1).GetComponent<Player>().playerInput) && timeBeforeValidateLeft > 0.0f))
                {
                    eventSystem.SetSelectedGameObject(eventSystem.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown().gameObject);
                    BaseEventData baseEventData = new BaseEventData(eventSystem);
                    bool flag = ExecuteEvents.Execute<ISelectHandler>(eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.selectHandler);
                    if (flag)
                        Debug.Log("Event Selected");
                    isPressingInput = false;
                    timeBeforeValidateLeft = timeBeforeValidate;
                    isPressingInput = false;
                }

            }
            //End Menu
            if (mainGameManager.currentGameState == MainGameManager.GameState.IsFinish)
            {
                if (Input.GetButtonDown(FindPlayer(Player.PlayerNumber.J1).GetComponent<Player>().playerInput))
                {
                    isPressingInput = true;
                    timeBeforeValidateLeft = timeBeforeValidate;
                }
                if (isPressingInput)
                {
                    timeBeforeValidateLeft -= Time.fixedUnscaledDeltaTime;
                }
                if ((Input.GetButtonUp(FindPlayer(Player.PlayerNumber.J1).GetComponent<Player>().playerInput) && timeBeforeValidateLeft <= 0.0f) || timeBeforeValidateLeft <= 0.0f)
                {
                    BaseEventData baseEventData = new BaseEventData(eventSystem);
                    bool flag = ExecuteEvents.Execute<ISubmitHandler>(eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.submitHandler);
                    if (flag)
                        Debug.Log("Event Selected");
                    timeBeforeValidateLeft = timeBeforeValidate;
                    isPressingInput = false;
                }
                else if ((Input.GetButtonUp(FindPlayer(Player.PlayerNumber.J1).GetComponent<Player>().playerInput) && timeBeforeValidateLeft > 0.0f))
                {
                    eventSystem.SetSelectedGameObject(eventSystem.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown().gameObject);
                    BaseEventData baseEventData = new BaseEventData(eventSystem);
                    bool flag = ExecuteEvents.Execute<ISelectHandler>(eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.selectHandler);
                    if (flag)
                        Debug.Log("Event Selected");
                    isPressingInput = false;
                    timeBeforeValidateLeft = timeBeforeValidate;
                    isPressingInput = false;
                }
            }
        }
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator ActivateBonusOverTime()
    {
        //GameObject lastBonus = null;
        GameObject currentBonus = null;
        while (!partyEnded)
        {
            yield return new WaitForSeconds(timeBetweenBonusSwitches);
            //Pick a random bonus circle but not the last one
            bool allBonusActive = true;
            foreach(GameObject circle in bonus)
            {
                Circle script = circle.GetComponent<Circle>();
                if(!script.bonusActive)
                {
                    allBonusActive = false;
                    break;
                }
            }
            if (allBonusActive)
                yield break;
            hack_fmod.start();
            currentBonus = bonus[0];
            while (currentBonus.GetComponent<Circle>().bonusActive)
            {
                int randomIndex = Random.Range(0, bonus.Length);
                currentBonus = bonus[randomIndex];
            }
            currentBonus.GetComponent<Circle>().SetActive(true);
            //Pick a random bonus type
            Circle.BonusType bonusType = (Circle.BonusType)Random.Range(1, 4);
            currentBonus.GetComponent<Circle>().ChangeBonusType(bonusType);
            Debug.Log("Bonus " + bonusType.ToString() + " activated !");
            yield return new WaitForSeconds(timeBetweenBonusSwitches);
            //currentBonus.GetComponent<Circle>().SetActive(false);
            //lastBonus = currentBonus;
        }
    }

    public void EndParty()
    {
        gameDurationLeft = 0f;
        int maxCircleColorised = -1;
        bool equality = false;
        foreach(GameObject player in players)
        {
            Player script = player.GetComponent<Player>();
            if (script.nbrOfCircleColored > maxCircleColorised)
            {
                if (winners.Count > 0)
                    winners.Clear();
                maxCircleColorised = script.nbrOfCircleColored;
                winners.Add(player);
            }
            else if (script.nbrOfCircleColored == maxCircleColorised)
            {
                equality = true;
                winners.Add(player);
            }
        }
        if(!equality)
        {
            partyEnded = true;
            eventsSystem.enabled = false;
            Debug.Log("The winner is " + winners[0].name + "color : " + winners[0].GetComponent<Player>().playerColor);
            mainGameManager.ChangeGameState(7);
            foreach(GameObject player in players)
            {
                Player script = player.GetComponent<Player>();
                Debug.Log("Player number : " + script.player.ToString() + " with color : " + script.playerColor.ToString() + " / had : " + script.nbrOfCircleColored + " circles colored");
            }
            mainGameManager.canvas[mainGameManager.currentGameState].transform.Find("Contour").GetComponent<Image>().color = GameColorToColor(winners[0].GetComponent<Player>().playerColor);
        }
        else
        {
            partyEnded = true;
            eventsSystem.enabled = false;
            Debug.Log("The winner is " + winners[0].name + "color : " + winners[0].GetComponent<Player>().playerColor);
            mainGameManager.ChangeGameState(7);
            foreach (GameObject player in players)
            {
                Player script = player.GetComponent<Player>();
                Debug.Log("Player number : " + script.player.ToString() + " with color : " + script.playerColor.ToString() + " / had : " + script.nbrOfCircleColored + " circles colored");
            }
            mainGameManager.canvas[mainGameManager.currentGameState].transform.Find("Contour").GetComponent<Image>().color = Color.white;
        }
        eventSystem.SetSelectedGameObject(buttonRecommencer.gameObject);
    }

    public void OnPlayerDestroyed(Player playerBehaviour, Player.PlayerNumber destroyer)
    {
        StartCoroutine(OnPlayerDestroy(playerBehaviour, destroyer));
    }

    IEnumerator OnPlayerDestroy(Player playerBehaviour, Player.PlayerNumber destroyer)
    {
        Debug.Log("Fonction OnPlayerDestroy called");
        Vector2 playerDiedPosition = playerBehaviour.transform.position;
        playerBehaviour.transform.parent = null;
        playerBehaviour.gameObject.SetActive(false);
        yield return new WaitForSeconds(timeBeforeRespawn);
        Debug.Log("Fonction OnPlayerDestroy continue");
        GameObject farthestSpawner;
        GameObject otherPlayer = FindPlayer(destroyer);
        if (otherPlayer != null)
            farthestSpawner = FarthestSpawner(otherPlayer.transform.position, playerBehaviour);
        else
            farthestSpawner = FarthestSpawner(playerDiedPosition, playerBehaviour);
        if(farthestSpawner == null)
        {
            Debug.Log(playerBehaviour.name + "lose, no more circle colored left");
            isPlayerAlive[playerBehaviour] = false;
            yield break;
        }
        playerBehaviour.transform.position = farthestSpawner.transform.position;
        playerBehaviour.gameObject.SetActive(true);
        playerBehaviour.PlayRespawnFeedbacks();
        playerBehaviour.Reset();

        //Sound
		if (playerBehaviour.player == Player.PlayerNumber.J1) {
			playerBehaviour.melo1_fmod.setValue (0.5f);
		} else if (playerBehaviour.player == Player.PlayerNumber.J2) {
			playerBehaviour.melo2_fmod.setValue (0.5f);
		} else if (playerBehaviour.player == Player.PlayerNumber.J3) {
			playerBehaviour.melo3_fmod.setValue (0.5f);
		} else if (playerBehaviour.player == Player.PlayerNumber.J4) {
			playerBehaviour.melo4_fmod.setValue (0.5f);
		}
	}

    float CheckDistanceBetweenSpawn(Vector2 position, GameObject spawn)
    {
        float distance = Vector2.Distance(position, spawn.transform.position);
        return distance;
    }

    GameObject FarthestSpawner(Vector2 position,Player player)
    {
        GameObject result = null;
        float maxDistance = 0f;
        Dictionary<GameObject, bool>.KeyCollection keys = player.circlesWithSameColor.Keys;
        foreach(GameObject key in keys)
        {
            if(player.circlesWithSameColor[key] && !key.GetComponent<Circle>().isBonus)
            {
                float distance = CheckDistanceBetweenSpawn(position, key);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    result = key;
                }
            }
        }
        if(result == null)
        {
            maxDistance = 0f;
            foreach (GameObject key in keys)
            {
                if (key.GetComponent<Circle>().color == GameColor.White && !key.GetComponent<Circle>().isBonus)
                {
                    float distance = CheckDistanceBetweenSpawn(position, key);
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        result = key;
                    }
                }
            }
        }
        return result;
    }

    public GameObject FindPlayer(Player.PlayerNumber player)
    {
        GameObject playerGO = null;
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<Player>().player == player)
            {
                playerGO = players[i];
                break;
            }
        }
        if (playerGO == null)
            Debug.Log("ERROR JE TROUVE PAS LE JOUEUR");
        return playerGO;
    }

    GameObject FindPlayerPrefab(Player.PlayerNumber player)
    {
        GameObject playerGO = null;
        for (int i = 0; i < playersPrefab.Length; i++)
        {
            if (playersPrefab[i].GetComponent<Player>().player == player)
            {
                playerGO = playersPrefab[i];
                break;
            }
        }
        return playerGO;
    }

    GameObject[] FindSpawns()
    {
        List<GameObject> spawners = new List<GameObject>();
        foreach(GameObject circle in circles)
        {
            Circle script = circle.GetComponent<Circle>();
            try
            {
                if (script.isSpawn)
                    spawners.Add(circle);
            }
            catch(System.NullReferenceException e)
            {
                Debug.Break();
                Debug.Log(e.Message + " : " + circle.name + " " + circle.transform.parent.name);
            }
        }
        return spawners.ToArray();
    }

    GameObject[] FindBonus()
    {
        List<GameObject> bonus = new List<GameObject>();
        foreach (GameObject circle in circles)
        {
            Circle script = circle.GetComponent<Circle>();
            if (script.isBonus)
                bonus.Add(circle);
        }
        return bonus.ToArray();
    }

    public void ChangeGameState(int gameState)
    {
        mainGameManager.ChangeGameState(gameState);
    }

    public Color GameColorToColor(GameColor gameColor)
    {
        Color color = new Color();
        switch(gameColor)
        {
            case GameColor.Blue:
                color = Color.blue;
                break;
            case GameColor.LightBlue:
                color = Color.cyan;
                break;
            case GameColor.Green:
                color = Color.green;
                break;
            case GameColor.Orange:
                color = new Color(1f, 0.5f, 0f);
                break;
            case GameColor.Rose:
                color = Color.magenta;
                break;
            case GameColor.Red:
                color = Color.red;
                break;
            case GameColor.Yellow:
                color = Color.yellow;
                break;
            default:
                break;
        }
        return color;
    }

    public Sprite GetPlayerSprite(GameColor playerColor)
    {
        Sprite sprite = new Sprite();
        string path = "Sprites/Players/";
        switch(playerColor)
        {
            case GameColor.Blue:
                path += "Sprite_PlayerBleu";
                break;
            case GameColor.LightBlue:
                path += "Sprite_PlayerBleuClair";
                break;
            case GameColor.Green:
                path += "Sprite_PlayerVert";
                break;
            case GameColor.Orange:
                path += "Sprite_PlayerOrange";
                break;
            case GameColor.Rose:
                path += "Sprite_PlayerViolet";
                break;
            case GameColor.Red:
                path += "Sprite_PlayerRouge";
                break;
            case GameColor.Yellow:
                path += "Sprite_PlayerJaune";
                break;
            default:
                break;
        }
        sprite = Resources.Load<Sprite>(path);
        return sprite;
    }


    //Bonus gestion

    public void ActivateBonus(Circle.BonusType bonusType)
    {
        StopCoroutine(ActivateBonusOverTime());
        GameObject currentBonus = null;
        bool allBonusActive = true;
        hack_fmod.start();
        foreach (GameObject circle in bonus)
        {
            Circle script = circle.GetComponent<Circle>();
            if (!script.bonusActive)
            {
                currentBonus = circle;
                allBonusActive = false;
                break;
            }
        }
        if (allBonusActive)
            return;
        currentBonus = bonus[0];
        while (currentBonus.GetComponent<Circle>().bonusActive)
        {
            int randomIndex = Random.Range(0, bonus.Length);
            currentBonus = bonus[randomIndex];
        }
        currentBonus.GetComponent<Circle>().SetActive(true);
        //Pick a random bonus type
        currentBonus.GetComponent<Circle>().ChangeBonusType(bonusType);
        Debug.Log("Bonus " + bonusType.ToString() + " activated !");
        StartCoroutine(ActivateBonusOverTime());
    }
}
