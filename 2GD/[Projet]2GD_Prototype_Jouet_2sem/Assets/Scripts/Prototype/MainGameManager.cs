using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainGameManager : MonoBehaviour {

    //Instance
    public static MainGameManager instance = null;

    //Current Game State
    public enum GameState
    {
        MenuInit,
        MenuPlay,
        MenuOptions,
        MenuCredits,
        MenuQuit,
        InGame,
        IsPaused,
        IsFinish,
        Lobby
    }
    public GameState currentGameState;

    public Dictionary<GameState, GameObject> canvas;
    //Dictionary<GameState, Scene> scenes;

    //Game Informations
    public int numberOfPlayers;
    public int gameSceneIndex = 3;
    public Dictionary<Player.PlayerNumber, GameManager.GameColor> playersColor;
    public Dictionary<Player.PlayerNumber, bool> playersPlaying;

    //References
    public GameManager gameManager;
    public MenuManager menuManager;
    public MenuNavigator menuNavigator;
    public LobbyManager lobbyManager;

    //Scenes
    public Scene mainMenu;
    public Scene inGame;

    //FMOD
    [Header("FMOD")]
    [Range(0f, 1f)]
    public float fmodMusiqueGLobalVol = 0.5f;
    [Range(0f, 1f)]
    public float fmodFXGLobalVol = 0.5f;

    [FMODUnity.EventRef]
	public string main_state;

    private FMOD.Studio.EventInstance main_fmod;
    public FMOD.Studio.ParameterInstance menu_fmod;

    private void Awake()
    {
        //Create Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);

            //Initialise 
            //currentGameState = GameState.MenuInit;
            canvas = new Dictionary<GameState, GameObject>();
            //scenes = new Dictionary<GameState, Scene>();
            playersColor = new Dictionary<Player.PlayerNumber, GameManager.GameColor>();
            playersPlaying = new Dictionary<Player.PlayerNumber, bool>();
            //InitScenes();

            //Event subscriber
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        }
        else if (instance != this)
            Destroy(gameObject);
    }
		

    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode arg1)
    {
        switch(currentGameState)
        {
            case GameState.MenuInit:
                InitMenuScene();
                break;
            case GameState.InGame:
                InitInGame();
                break;
            case GameState.Lobby:
                InitLobby();
                break;
            default:
                break;
        }
    }

    public void InitMenuScene()
    {
		main_fmod = FMODUnity.RuntimeManager.CreateInstance(main_state);
		main_fmod.getParameter ("menu", out menu_fmod);
		menu_fmod.setValue (0.5f);
		main_fmod.setVolume (1f);
		main_fmod.start ();
	
        //Set the references
        menuManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<MenuManager>();
        menuNavigator = GameObject.Find("MenuNavigator").GetComponent<MenuNavigator>();
        menuManager.mainGameManager = instance;
        //Get the canvas
        if (canvas.Count > 0)
            canvas.Clear();
        canvas.Add(GameState.MenuInit, GameObject.Find("Canvas_Init"));
        canvas.Add(GameState.MenuPlay, GameObject.Find("Canvas_Play"));
        canvas.Add(GameState.MenuOptions, GameObject.Find("Canvas_Options"));
        canvas.Add(GameState.MenuCredits, GameObject.Find("Canvas_Credits"));

        //Set the players default color
        if (playersColor.Count > 0)
            playersColor.Clear();
        playersColor.Add(Player.PlayerNumber.J1, GameManager.GameColor.LightBlue);
        playersColor.Add(Player.PlayerNumber.J2, GameManager.GameColor.Red);
        playersColor.Add(Player.PlayerNumber.J3, GameManager.GameColor.Yellow);
        playersColor.Add(Player.PlayerNumber.J4, GameManager.GameColor.Green);

        //Set the bool to true if the player is playing
        if (playersPlaying.Count > 0)
            playersPlaying.Clear();
        playersPlaying.Add(Player.PlayerNumber.J1, false);
        playersPlaying.Add(Player.PlayerNumber.J2, false);
        playersPlaying.Add(Player.PlayerNumber.J3, false);
        playersPlaying.Add(Player.PlayerNumber.J4, false);

        //Reset listeners
        //GameObject.Find("ButtonJOUER").GetComponent<Button>().onClick.AddListener(delegate { ChangeGameState(1); });
        //GameObject.Find("ButtonOPTIONS").GetComponent<Button>().onClick.AddListener(delegate { ChangeGameState(2); });
        //GameObject.Find("ButtonCREDITS").GetComponent<Button>().onClick.AddListener(delegate { ChangeGameState(3); });
        //GameObject.Find("ButtonQUIT").GetComponent<Button>().onClick.AddListener(delegate { ChangeGameState(4); });
        //GameObject.Find("ButtonREADY").GetComponent<Button>().onClick.AddListener(delegate { ChangeGameState(5); });
        //Set the active canvas
        SetActiveCanvas();
    }

    public void InitInGame()
    {
        //Set the references
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        gameManager.mainGameManager = instance;
        //Get the canvas
        if (canvas.Count > 0)
            canvas.Clear();
        canvas.Add(GameState.IsPaused, GameObject.Find("Canvas_Pause"));
        canvas.Add(GameState.IsFinish, GameObject.Find("Canvas_End"));

        //Set the players default color
        if (playersPlaying.Count == 0)
        {
            playersPlaying.Add(Player.PlayerNumber.J1, true);
            playersPlaying.Add(Player.PlayerNumber.J2, true);
            if(numberOfPlayers >= 3)
            {
                if (numberOfPlayers >= 4)
                {
                    playersPlaying.Add(Player.PlayerNumber.J3, true);
                    playersPlaying.Add(Player.PlayerNumber.J4, true);
                }
                else
                {
                    playersPlaying.Add(Player.PlayerNumber.J3, true);
                    playersPlaying.Add(Player.PlayerNumber.J4, false);
                }
            }
            else
            {
                playersPlaying.Add(Player.PlayerNumber.J3, false);
                playersPlaying.Add(Player.PlayerNumber.J4, false);
            }
            
        }
        //Set the players default color
        if (playersColor.Count == 0)
        {
            playersColor.Add(Player.PlayerNumber.J1, GameManager.GameColor.Red);
            playersColor.Add(Player.PlayerNumber.J2, GameManager.GameColor.Yellow);
            playersColor.Add(Player.PlayerNumber.J3, GameManager.GameColor.Rose);
            playersColor.Add(Player.PlayerNumber.J4, GameManager.GameColor.Green);
        }

        //Initialise
        gameManager.Initialise(instance);
        //Reset listeners
        //GameObject.Find("ButtonCONTINUE").GetComponent<Button>().onClick.AddListener(delegate { ChangeGameState(5); });
        //GameObject.Find("ButtonOPTIONS").GetComponent<Button>().onClick.AddListener(delegate { ChangeGameState(5); });
        //GameObject.Find("ButtonQUIT").GetComponent<Button>().onClick.AddListener(delegate { ChangeGameState(0); });
        //Set the active canvas
        SetActiveCanvas();
    }

    public void InitLobby()
    {
        //Set the references
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        lobbyManager = GameObject.Find("LobbyManager").GetComponent<LobbyManager>();
        lobbyManager.mainGameManager = instance;

        //Set the players default color
        if (playersPlaying.Count == 0)
        {
            playersPlaying.Add(Player.PlayerNumber.J1, true);
            playersPlaying.Add(Player.PlayerNumber.J2, true);
            if (numberOfPlayers >= 3)
            {
                if (numberOfPlayers >= 4)
                {
                    playersPlaying.Add(Player.PlayerNumber.J3, true);
                    playersPlaying.Add(Player.PlayerNumber.J4, true);
                }
                else
                {
                    playersPlaying.Add(Player.PlayerNumber.J3, true);
                    playersPlaying.Add(Player.PlayerNumber.J4, false);
                }
            }
            else
            {
                playersPlaying.Add(Player.PlayerNumber.J3, false);
                playersPlaying.Add(Player.PlayerNumber.J4, false);
            }
        }
        //Set the players default color
        if (playersColor.Count == 0)
        {
            playersColor.Add(Player.PlayerNumber.J1, GameManager.GameColor.Orange);
            playersColor.Add(Player.PlayerNumber.J2, GameManager.GameColor.Yellow);
            playersColor.Add(Player.PlayerNumber.J3, GameManager.GameColor.Rose);
            playersColor.Add(Player.PlayerNumber.J4, GameManager.GameColor.Green);
        }

        lobbyManager.Initialise(numberOfPlayers);
        gameManager.Initialise(instance);
    }

    //Change the game scene ot load
    public void ChangeGameScene(int gameScene)
    {
        gameSceneIndex = gameScene;
    }

    //Call this function when you need to change the current scene
    public void ChangeGameState(int gameState)
    {
        GameState lastGameState = currentGameState;
        currentGameState = (GameState)gameState;
        switch(currentGameState)
        {
            case GameState.MenuInit:
                if (lastGameState == GameState.IsPaused || lastGameState == GameState.IsFinish)
                {
                    UnPauseGame();
                    SceneManager.LoadScene(1, LoadSceneMode.Single);
                }
                else
                {
                    LoadMenuInit();
			        //main_fmod = FMODUnity.RuntimeManager.CreateInstance(main_state);
				    //main_fmod.getParameter ("menu", out menu_fmod);
				    //menu_fmod.setValue (0.5f);
                  //if(lastGameState != GameState.MenuOptions || lastGameState != GameState.MenuCredits || lastGameState != GameState.MenuPlay)
				        //main_fmod.start();
                    menuNavigator.currentMenu = MenuNavigator.CurrentMenu.Main;
                    menuNavigator.ChangeMenu();
                    if(lastGameState == GameState.MenuOptions)
                    {
                        fmodMusiqueGLobalVol = menuNavigator.optionsMenuNavigator.musiqueVol; 
					    fmodFXGLobalVol = menuNavigator.optionsMenuNavigator.FXVol;
                    }
                }
                break;
            case GameState.MenuPlay:
                menuNavigator.currentMenu = MenuNavigator.CurrentMenu.Play;
                menuNavigator.ChangeMenu();
                LoadMenuPlay();
                break;
            case GameState.MenuOptions:
                menuNavigator.currentMenu = MenuNavigator.CurrentMenu.Options;
                menuNavigator.ChangeMenu();
                LoadMenuOptions();
                break;
            case GameState.MenuCredits:
                menuNavigator.currentMenu = MenuNavigator.CurrentMenu.Credits;
                menuNavigator.ChangeMenu();
                LoadMenuCredits();
                break;
		case GameState.InGame:
                if (lastGameState == GameState.MenuPlay || lastGameState == GameState.IsFinish || lastGameState == GameState.Lobby)
                {
                    menu_fmod.setValue(0f);
                    SceneManager.LoadScene(gameSceneIndex, LoadSceneMode.Single);
                }
                if(lastGameState == GameState.IsFinish || lastGameState == GameState.IsPaused)
                    UnPauseGame();
                break;
            case GameState.IsPaused:
                PauseGame();
                break;
            case GameState.IsFinish:
                LoadMenuEnd();
                break;
            case GameState.MenuQuit:
                menuNavigator.currentMenu = MenuNavigator.CurrentMenu.Quit;
                Application.Quit();
                break;
            case GameState.Lobby:
                if (lastGameState == GameState.MenuPlay)
                {
                    //main_fmod.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                    menu_fmod.setValue(0f);
                    SceneManager.LoadScene(2, LoadSceneMode.Single);
                }
                UnPauseGame();
                break;
            default:
                break;
        }
    }

    private void LoadMenuInit()
    {
        //Active and deactive canvas
        SetActiveCanvas();
    }
    private void LoadMenuPlay()
    {
        menuNavigator.SetUpMenuPlay();
        //Active and deactive canvas
        SetActiveCanvas();
    }
    private void LoadMenuOptions()
    {
        //Active and deactive canvas
        SetActiveCanvas();
    }
    private void LoadMenuCredits()
    {
        //Active and deactive canvas
        SetActiveCanvas();
    }

    private void PauseGame()
    {
		foreach(GameObject player in gameManager.players)
		{
			Player script = player.GetComponent<Player>();
			script.black_fmod.setValue (1f);
		}
        Time.timeScale = 0f;
        //gameManager.eventSystem.SetSelectedGameObject(gameManager.eventSystem.firstSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown().gameObject);
        //BaseEventData baseEventData = new BaseEventData(gameManager.eventSystem);
        //bool flag = ExecuteEvents.Execute<ISelectHandler>(gameManager.eventSystem.firstSelectedGameObject, baseEventData, ExecuteEvents.selectHandler);
        //if (flag)
            //Debug.Log("Event Selected");
        SetActiveCanvas();
    }

    private void UnPauseGame()
    {
        if (currentGameState == GameState.InGame || currentGameState == GameState.IsPaused)
        {
            foreach (GameObject player in gameManager.players)
            {
                Player script = player.GetComponent<Player>();
                if(gameManager.eventsSystem.currentGameEvents != EventsSystem.GameEvents.Blackout)
                    script.black_fmod.setValue(0f);
            }
        }
        Time.timeScale = 1f;
        SetActiveCanvas();
    }

    private void LoadMenuEnd()
    {
		foreach(GameObject player in gameManager.players)
		{
			Player script = player.GetComponent<Player>();
			script.black_fmod.setValue (1f);
		}
        Time.timeScale = 0f;
        EndGameGraph[] graphs = canvas[currentGameState].GetComponentsInChildren<EndGameGraph>();
        foreach (EndGameGraph graph in graphs)
        {
            GameObject player = gameManager.FindPlayer(graph.playerGraph);
            if (player != null)
            {
                graph.gameObject.SetActive(true);
                graph.Initialise(player.GetComponent<Player>(), gameManager.FindPlayer(graph.playerGraph).GetComponent<Player>().nbrOfCircleColored);
            }
            else
            {
                graph.gameObject.SetActive(false);
            }
        }
        SetActiveCanvas();
    }

    private void SetActiveCanvas()
    {
        Dictionary<GameState, GameObject>.KeyCollection keys = canvas.Keys;
        foreach(GameState key in keys)
        {
            if (currentGameState == key)
                canvas[key].SetActive(true);
            else
                canvas[key].SetActive(false);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            OnEscapePressed();
        }
    }

    private void OnEscapePressed()
    {
        switch(currentGameState)
        {
            case GameState.MenuInit:
                break;
            case GameState.MenuPlay:
            case GameState.MenuOptions:
            case GameState.MenuCredits:
                ChangeGameState(0);
                break;
            case GameState.InGame:
                gameManager.eventSystem.SetSelectedGameObject(gameManager.buttonContinue.gameObject);
                ChangeGameState(6);
                break;
            case GameState.IsPaused:
                ChangeGameState(5);
                break;
            default:
                break;
        }
    }
}
