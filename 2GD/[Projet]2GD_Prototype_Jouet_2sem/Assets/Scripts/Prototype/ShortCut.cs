using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShortCut : MonoBehaviour {

    //Singleton
    public static ShortCut instance;

    #region Variables
    [Header("Menu")]
    public string SC_ValidatePlayerColors = "return";

    [Header("Lobby")]
    public string SC_GoToMap1 = "1";
    public string SC_GoToMap2 = "2";
    public string SC_GoToMap3 = "3";
    public string SC_GoToMap4 = "4";

    [Header("Play mode")]
    public string SC_RebootGame = "f1";
    public string SC_RebootEntities = "f2";
    public string SC_EndGame = "f3";
    public string SC_AllColorsShuffled = "f4";
    public string SC_InfiniteTime = "f5";
    public string SC_GameDeltaTimeSpeed = "f6";
    public string SC_StartEventBlackout = "f7";
    public string SC_StartEventIEM = "f8";
    public string SC_ToggleEvent = "f9";
    public string SC_StartBonusBoost = "1";
    public string SC_StartBonusHack = "2";
    public string SC_StartBonusShield = "3";

    #endregion

    //References
    public MainGameManager mainGameManager;
    public LobbyManager lobbyManager;
    public GameManager gameManager;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else if (instance != this)
            Destroy(gameObject);

        //Assign references
        mainGameManager = GameObject.Find("MainGameManager").GetComponent<MainGameManager>();
        if (mainGameManager.currentGameState == MainGameManager.GameState.InGame)
        {
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        }

        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if(mainGameManager.currentGameState == MainGameManager.GameState.InGame)
        {
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        }
        else if (mainGameManager.currentGameState == MainGameManager.GameState.Lobby)
        {
            lobbyManager = GameObject.Find("LobbyManager").GetComponent<LobbyManager>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(SC_RebootGame) && mainGameManager.currentGameState == MainGameManager.GameState.InGame)
        {
            gameManager.ReloadScene();
        }
        else if (Input.GetKeyDown(SC_RebootEntities) && mainGameManager.currentGameState == MainGameManager.GameState.InGame)
        {
            foreach (GameObject circle in gameManager.circles)
            {
                Circle script = circle.GetComponent<Circle>();
                script.Reset();
            }
        }
        else if (Input.GetKeyDown(SC_EndGame) && mainGameManager.currentGameState == MainGameManager.GameState.InGame)
        {
            gameManager.gameDurationLeft = 0.0f;
        }
        else if (Input.GetKeyDown(SC_AllColorsShuffled) && mainGameManager.currentGameState == MainGameManager.GameState.InGame)
        {
            foreach (GameObject player in gameManager.players)
            {
                Player script = player.GetComponent<Player>();
                script.nbrOfCircleColored = 0;
            }
            List<Player.PlayerNumber> playersPlaying = new List<Player.PlayerNumber>();
            foreach(Player.PlayerNumber number in mainGameManager.playersPlaying.Keys)
            {
                if(mainGameManager.playersPlaying[number])
                {
                    playersPlaying.Add(number);
                }
            }
            foreach (GameObject circle in gameManager.circles)
            {
                Circle script = circle.GetComponent<Circle>();
                if (!script.isBonus)
                {
                    script.ResetColor();
                }
            }
            foreach (GameObject circle in gameManager.circles)
            {
                Circle script = circle.GetComponent<Circle>();
                if (!script.isBonus)
                {
                    int randomIndex = Random.Range(0, playersPlaying.Count);
                    script.ChangeColor(gameManager.FindPlayer(playersPlaying[randomIndex]).GetComponent<Player>());
                }
            }
            foreach (GameObject player in gameManager.players)
            {
                Player script = player.GetComponent<Player>();
                script.UpdateCircleColored();
            }
        }
        else if (Input.GetKeyDown(SC_InfiniteTime) && mainGameManager.currentGameState == MainGameManager.GameState.InGame)
        {
            if (gameManager.gameDurationLeft != Mathf.Infinity)
                gameManager.gameDurationLeft = Mathf.Infinity;
            else if (gameManager.gameDurationLeft == Mathf.Infinity)
                gameManager.gameDurationLeft = 120f;
        }
        else if (Input.GetKeyDown(SC_GameDeltaTimeSpeed) && mainGameManager.currentGameState == MainGameManager.GameState.InGame)
        {
            if (mainGameManager.currentGameState == MainGameManager.GameState.InGame)
            {
                if (Time.timeScale == 1f)
                {
                    Time.timeScale = 0.75f;
                }
                else if (Time.timeScale == 0.75f)
                {
                    Time.timeScale = 0.5f;
                }
                else if (Time.timeScale == 0.5f)
                {
                    Time.timeScale = 0.25f;
                }
                else if (Time.timeScale == 0.25f)
                {
                    Time.timeScale = 1f;
                }
            }
        }
        else if (Input.GetKeyDown(SC_StartEventBlackout) && mainGameManager.currentGameState == MainGameManager.GameState.InGame)
        {
            if (gameManager.eventsSystem.isEventActive)
            {
                gameManager.eventsSystem.DeactivateCurrentEvent();
            }
            gameManager.eventsSystem.ActivateBlackoutEvent();
            gameManager.eventsSystem.isEventActive = true;
            gameManager.eventsSystem.currentTimer = 0.0f;
            gameManager.eventsSystem.eventsSystemActive = true;
        }
        else if (Input.GetKeyDown(SC_StartEventIEM) && mainGameManager.currentGameState == MainGameManager.GameState.InGame)
        {
            if (gameManager.eventsSystem.isEventActive)
            {
                gameManager.eventsSystem.DeactivateCurrentEvent();
            }
            gameManager.eventsSystem.ActivateBouncingBallEvent();
            gameManager.eventsSystem.isEventActive = true;
            gameManager.eventsSystem.currentTimer = 0.0f;
            gameManager.eventsSystem.eventsSystemActive = true;
        }
        else if (Input.GetKeyDown(SC_StartBonusBoost) && mainGameManager.currentGameState == MainGameManager.GameState.InGame)
        {
            gameManager.ActivateBonus(Circle.BonusType.Boost);
        }
        else if (Input.GetKeyDown(SC_StartBonusHack) && mainGameManager.currentGameState == MainGameManager.GameState.InGame)
        {
            gameManager.ActivateBonus(Circle.BonusType.Hack);
        }
        else if (Input.GetKeyDown(SC_StartBonusShield) && mainGameManager.currentGameState == MainGameManager.GameState.InGame)
        {
            gameManager.ActivateBonus(Circle.BonusType.Shield);
        }
        else if (Input.GetKeyDown(SC_ToggleEvent) && mainGameManager.currentGameState == MainGameManager.GameState.InGame)
        {
            if (gameManager.eventsSystem.eventsSystemActive)
            {
                gameManager.eventsSystem.DeactivateCurrentEvent();
                gameManager.eventsSystem.currentTimer = 0.0f;
            }
            gameManager.eventsSystem.eventsSystemActive = !gameManager.eventsSystem.eventsSystemActive;
        }
        else if (Input.GetKeyDown(SC_ValidatePlayerColors) && mainGameManager.currentGameState == MainGameManager.GameState.MenuPlay)
        {
            foreach (MenuPlayer player in mainGameManager.menuNavigator.menuPlayers)
            {
                if (!player.isWaitingPlayerInput)
                {
                    player.playerState = MenuPlayer.PlayerState.Ready;
                    player.GoNextPlayerState();
                }
            }
        }
        else if(Input.GetKeyDown(SC_GoToMap1) && mainGameManager.currentGameState == MainGameManager.GameState.Lobby)
        {
            mainGameManager.ChangeGameScene(3);
            mainGameManager.ChangeGameState(5);
        }
        else if (Input.GetKeyDown(SC_GoToMap2) && mainGameManager.currentGameState == MainGameManager.GameState.Lobby)
        { 
            mainGameManager.ChangeGameScene(4);
            mainGameManager.ChangeGameState(5);
        }
        else if (Input.GetKeyDown(SC_GoToMap3) && mainGameManager.currentGameState == MainGameManager.GameState.Lobby)
        {
            mainGameManager.ChangeGameScene(5);
            mainGameManager.ChangeGameState(5);
        }
        else if (Input.GetKeyDown(SC_GoToMap4) && mainGameManager.currentGameState == MainGameManager.GameState.Lobby)
        {
            mainGameManager.ChangeGameScene(6);
            mainGameManager.ChangeGameState(5);
        }
    }
}
