using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuNavigator : MonoBehaviour {

    public MainGameManager mainGameManager;

    //Enums
    public enum CurrentMenu
    {
        Main,
        Play,
        Options,
        Credits,
        Quit
    }
    //The current menu
    public CurrentMenu currentMenu;

    //First selected
    //Main Menu
    public GameObject firstSelectedMainMenu;

    //Play Menu
    public Scrollbar firstSelectedPlayMenu_J1;
    public Scrollbar firstSelectedPlayMenu_J2;
    public Scrollbar firstSelectedPlayMenu_J3;
    public Scrollbar firstSelectedPlayMenu_J4;

    //Players canvas
    public RectTransform playerCanvas_J1;
    public RectTransform playerCanvas_J2;
    public RectTransform playerCanvas_J3;
    public RectTransform playerCanvas_J4;

    //Menu Players
    public MenuPlayer playerJ1 = null;
    public MenuPlayer playerJ2 = null;
    public MenuPlayer playerJ3 = null;
    public MenuPlayer playerJ4 = null;
    public MenuPlayer[] menuPlayers = null;

    //Options Menu
    public OptionsMenuNavigator optionsMenuNavigator = null;
    //Ref
    public Canvas optionsCanvas;
    public GameObject scrollBarMusique;
    public GameObject scrollBarFX;

    //Main menu navigator
    public MainMenuNavigator mainMenuNavigator = null;

    //Event System Ref
    public EventSystem eventSystem;

    private void Awake()
    {
        mainGameManager = GameObject.Find("MainGameManager").GetComponent<MainGameManager>();
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
    }

    private void Start()
    {
        SetUpMainMenu();
    }

    public void SetUpMainMenu()
    {
        //Change the current Menu
        currentMenu = CurrentMenu.Main;

        mainMenuNavigator = new MainMenuNavigator("InputJ1", "InputJ2", firstSelectedMainMenu);

        eventSystem.SetSelectedGameObject(mainMenuNavigator.currentGameObjectSelected);
        BaseEventData baseEventData = new BaseEventData(eventSystem);
        bool flag = ExecuteEvents.Execute<ISelectHandler>(eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.selectHandler);
        if (flag)
            Debug.Log("Event Selected");
        else
            Debug.Log("Error, can't raise event selected");
    }

    public void SetUpMenuPlay()
    {
        //Change the current Menu
        currentMenu = CurrentMenu.Play;

        //Setup the players

        playerJ1 = new MenuPlayer("InputJ1", Player.PlayerNumber.J1, MenuPlayer.PlayerState.ColorSelection, GameManager.GameColor.Blue, firstSelectedPlayMenu_J1.gameObject, playerCanvas_J1, eventSystem);
        playerJ2 = new MenuPlayer("InputJ2", Player.PlayerNumber.J2, MenuPlayer.PlayerState.ColorSelection, GameManager.GameColor.LightBlue, firstSelectedPlayMenu_J2.gameObject, playerCanvas_J2, eventSystem);
        playerJ3 = new MenuPlayer("InputJ3", Player.PlayerNumber.J3, MenuPlayer.PlayerState.IsWaitingForPlayer, GameManager.GameColor.Green, firstSelectedPlayMenu_J3.gameObject, playerCanvas_J3, eventSystem);
        playerJ4 = new MenuPlayer("InputJ4", Player.PlayerNumber.J4, MenuPlayer.PlayerState.IsWaitingForPlayer, GameManager.GameColor.Red, firstSelectedPlayMenu_J4.gameObject, playerCanvas_J4, eventSystem);

        menuPlayers = new MenuPlayer[] { playerJ1, playerJ2, playerJ3, playerJ4 };
    }

    public void SetUpMenuOptions()
    {
        //Change the current menu
        currentMenu = CurrentMenu.Options;

        //Assign references

        //Setup the menu
        optionsMenuNavigator = new OptionsMenuNavigator("InputJ1", "InputJ2", optionsCanvas, scrollBarMusique, scrollBarFX, mainGameManager.fmodMusiqueGLobalVol, mainGameManager.fmodFXGLobalVol);
    }

    public void ChangeMenu()
    {
        switch(currentMenu)
        {
            case CurrentMenu.Main:
                currentMenu = CurrentMenu.Main;
                mainMenuNavigator.Reset();
                eventSystem.SetSelectedGameObject(mainMenuNavigator.currentGameObjectSelected);
                BaseEventData baseEventData = new BaseEventData(eventSystem);
                bool flag = ExecuteEvents.Execute<ISelectHandler>(eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.selectHandler);
                if (flag)
                    Debug.Log("Event Selected");
                else
                    Debug.Log("Error, can't raise event selected");
                break;
            case CurrentMenu.Play:
                currentMenu = CurrentMenu.Play;
                foreach (MenuPlayer player in menuPlayers)
                {
                    player.Reset();
                }
                break;
            case CurrentMenu.Options:
                SetUpMenuOptions();
                currentMenu = CurrentMenu.Options;
                break;
            case CurrentMenu.Credits:
                break;
            case CurrentMenu.Quit:
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        if(currentMenu == CurrentMenu.Main)
        {
            if(Input.GetButtonDown(mainMenuNavigator.inputJ1) || Input.GetButtonDown(mainMenuNavigator.inputJ2))
            {
                mainMenuNavigator.isInputPressed = true;
                mainMenuNavigator.goNextStateAfterRelease = true;
                BaseEventData baseEventData = new BaseEventData(eventSystem);
                bool flag = ExecuteEvents.Execute<ISubmitHandler>(mainMenuNavigator.currentGameObjectSelected, baseEventData, ExecuteEvents.submitHandler);
                if (flag)
                    Debug.Log("Event Submitted");
            }
            if(mainMenuNavigator.isInputPressed && mainMenuNavigator.goNextStateAfterRelease)
            {
                mainMenuNavigator.timeInputPressed += Time.deltaTime;
                if(mainMenuNavigator.timeInputPressed > MainMenuNavigator.timeInputPressedForNextState)
                {
                    mainMenuNavigator.goNextStateAfterRelease = false;
                    mainMenuNavigator.timeInputPressed = 0f;
                }
            }
            if (Input.GetButtonUp(mainMenuNavigator.inputJ1) || Input.GetButtonUp(mainMenuNavigator.inputJ2) &&
                !(Input.GetButton(mainMenuNavigator.inputJ1) && Input.GetButton(mainMenuNavigator.inputJ2)))
            {
                mainMenuNavigator.isInputPressed = false;
                BaseEventData baseEventData = new BaseEventData(eventSystem);
                bool flag = ExecuteEvents.Execute<ICancelHandler>(mainMenuNavigator.currentGameObjectSelected, baseEventData, ExecuteEvents.cancelHandler);
                if (flag)
                    Debug.Log("Event Canceled");
                if (mainMenuNavigator.goNextStateAfterRelease)
                {
                    mainMenuNavigator.GoNextState();
                    mainMenuNavigator.goNextStateAfterRelease = false;
                }
            }
        }
        else if(currentMenu == CurrentMenu.Options)
        {
            if(Input.GetButtonDown(optionsMenuNavigator.inputMusique))
            {
                optionsMenuNavigator.MusiqueNextValue();
            }
            if (Input.GetButtonDown(optionsMenuNavigator.inputFX))
            {

                optionsMenuNavigator.FXNextValue();
            }
        }
        else if(currentMenu == CurrentMenu.Play)
        {
            bool isGameReady = true;
            int nbrOfPlayerRdy = 0;
            for (int i = 0; i < menuPlayers.Length; i++)
            {
                MenuPlayer currentPlayer = menuPlayers[i];
                if (Input.GetButtonDown(currentPlayer.input))
                {
                    currentPlayer.inputPressed = true;
                    switch (currentPlayer.playerState)
                    {
                        case MenuPlayer.PlayerState.IsWaitingForPlayer:
                            currentPlayer.nextStateOnRelease = true;
                            break;
                        case MenuPlayer.PlayerState.ColorSelection:
                            currentPlayer.isChangingColor = true;
                            currentPlayer.timeInputPressed = 0f;
                            break;
                        case MenuPlayer.PlayerState.Ready:
                            currentPlayer.GoNextPlayerState();
                            break;
                        default:
                            break;
                    }
                }
                if (currentPlayer.isChangingColor || (currentPlayer.isReady && currentPlayer.inputReleased && currentPlayer.inputPressed))
                {
                    if(!currentPlayer.nextStateOnRelease)
                        currentPlayer.timeInputPressed += Time.deltaTime;
                    if (currentPlayer.timeInputPressed > MenuPlayer.timeInputPressedForNextState)
                    {
                        currentPlayer.nextStateOnRelease = true;
                        currentPlayer.isChangingColor = false;
                        currentPlayer.timeInputPressed = 0f;
                    }
                }
                if (Input.GetButtonUp(currentPlayer.input) || currentPlayer.nextStateOnRelease)
                {
                    if ((currentPlayer.timeInputPressed == 0 && currentPlayer.nextStateOnRelease) || currentPlayer.timeInputPressed != 0)
                    {
                        switch (currentPlayer.playerState)
                        {
                            case MenuPlayer.PlayerState.IsWaitingForPlayer:
                                {
                                    currentPlayer.isWaitingPlayerInput = false;
                                    currentPlayer.GoNextPlayerState();
                                    //Adapt the color if choosen
                                    GameManager.GameColor color = currentPlayer.colorSelected;
                                    bool colorCanBeChoosen = false;
                                    while (!colorCanBeChoosen)
                                    {
                                        if ((int)color > 7)
                                            color = (GameManager.GameColor)1;
                                        foreach (MenuPlayer otherPlayer in menuPlayers)
                                        {
                                            GameManager.GameColor otherColor = otherPlayer.colorSelected;
                                            if (color == otherColor)
                                            {
                                                colorCanBeChoosen = false;
                                                color++;
                                                break;
                                            }
                                            else
                                            {
                                                colorCanBeChoosen = true;
                                            }
                                        }
                                    }
                                    currentPlayer.colorSelected = color;
                                    currentPlayer.scrollBar.value = currentPlayer.colorValue[color];
                                    currentPlayer.nextStateOnRelease = false;
                                    break;
                                }
                            case MenuPlayer.PlayerState.ColorSelection:
                                if (currentPlayer.nextStateOnRelease)
                                {
                                    currentPlayer.nextStateOnRelease = false;
                                    currentPlayer.GoNextPlayerState();
                                }
                                else
                                {
                                    //Change the color
                                    GameManager.GameColor color = currentPlayer.colorSelected;
                                    color++;
                                    bool colorCanBeChoosen = false;
                                    while (!colorCanBeChoosen)
                                    {
                                        if ((int)color > 7)
                                            color = (GameManager.GameColor)1;
                                        foreach (MenuPlayer otherPlayer in menuPlayers)
                                        {
                                            GameManager.GameColor otherColor = otherPlayer.colorSelected;
                                            if (color == otherColor)
                                            {
                                                colorCanBeChoosen = false;
                                                color++;
                                                break;
                                            }
                                            else
                                            {
                                                colorCanBeChoosen = true;
                                            }
                                        }
                                    }
                                    currentPlayer.colorSelected = color;
                                    currentPlayer.scrollBar.value = currentPlayer.colorValue[color];
                                }
                                currentPlayer.inputReleased = false;
                                currentPlayer.isChangingColor = false;
                                currentPlayer.timeInputPressed = 0f;
                                break;
                            case MenuPlayer.PlayerState.IsReady:
                                currentPlayer.GoPreviousPlayerState();
                                currentPlayer.nextStateOnRelease = false;
                                break;
                            default:
                                break;
                        }
                    }
                }
                if(Input.GetButtonUp(currentPlayer.input))
                {
                    currentPlayer.inputReleased = true;
                    currentPlayer.inputPressed = false;
                }
                if (currentPlayer.isReady)
                    nbrOfPlayerRdy++;
                isGameReady &= (currentPlayer.playerState == MenuPlayer.PlayerState.IsWaitingForPlayer) ? true : (currentPlayer.playerState == MenuPlayer.PlayerState.IsReady);
            }
            if (isGameReady)
            {
                mainGameManager.numberOfPlayers = nbrOfPlayerRdy;
                Dictionary<Player.PlayerNumber, GameManager.GameColor>.KeyCollection keys = mainGameManager.playersColor.Keys;
                Player.PlayerNumber[] playersNumber = new Player.PlayerNumber[keys.Count];
                keys.CopyTo(playersNumber, 0);
                foreach(Player.PlayerNumber key in playersNumber)
                {
                    MenuPlayer currentPlayer = null;
                    foreach(MenuPlayer player in menuPlayers)
                    {
                        if(player.playerNumber == key)
                        {
                            currentPlayer = player;
                            if(currentPlayer.isReady)
                                mainGameManager.playersPlaying[key] = true;
                            else
                                mainGameManager.playersPlaying[key] = false;
                            break;
                        }
                    }
                    mainGameManager.playersColor[key] = currentPlayer.colorSelected;
                }
                mainGameManager.ChangeGameState(8);
            }   
        }
    }
}

[System.Serializable]
public class MenuPlayer
{
    public enum PlayerState
    {
        IsWaitingForPlayer,
        ColorSelection,
        Ready,
        IsReady
    }

    [Header("Player properties")]
    public bool isWaitingPlayerInput;
    public bool isChangingColor;
    public bool nextStateOnRelease;
    public bool isReady;
    public bool inputReleased;
    public bool inputPressed;
    [ReadOnly]
    public float timeInputPressed;
    public const float timeInputPressedForNextState =1f;
    public string input;

    public Player.PlayerNumber playerNumber;
    public GameManager.GameColor colorSelected;
    public Dictionary<GameManager.GameColor, float> colorValue;
    public PlayerState playerState;

    //References
    public RectTransform canvas;
    public EventSystem eventSystem;

    //Player UI
    public GameObject[] playerUIs;
    public Scrollbar scrollBar;
    public GameObject playerWaitingForInputUI;

    //Button Sprites
    public GameObject spriteButton;
    public GameObject spriteButtonHighlighted;

    [ReadOnly]
    public GameObject firstGameObjectSelected;
    [ReadOnly]
    public GameObject currentGameObjectSelected;

    public MenuPlayer(string input, Player.PlayerNumber playerNumber, PlayerState playerState, GameManager.GameColor firstColorSelected, GameObject firstGameObjectSelected, RectTransform canvas,EventSystem eventSystem)
    {
        this.isReady = false;
        this.inputReleased = false;
        this.input = input;
        this.playerNumber = playerNumber;
        this.playerState = playerState;
        this.firstGameObjectSelected = firstGameObjectSelected;
        this.canvas = canvas;
        this.eventSystem = eventSystem;
        this.colorSelected = firstColorSelected;

        colorValue = new Dictionary<GameManager.GameColor, float>();
        colorValue.Add(GameManager.GameColor.Blue, 0f);
        colorValue.Add(GameManager.GameColor.LightBlue, 0.2f);
        colorValue.Add(GameManager.GameColor.Green, 0.4f);
        colorValue.Add(GameManager.GameColor.Red, 0.5f);
        colorValue.Add(GameManager.GameColor.Rose, 0.7f);
        colorValue.Add(GameManager.GameColor.Orange, 0.8f);
        colorValue.Add(GameManager.GameColor.Yellow, 1f);

        List<GameObject> UIs = new List<GameObject>();
        for (int childIndex = 0; childIndex < canvas.childCount; childIndex++) 
        {
            Transform currentChild = canvas.GetChild(childIndex);
            if (currentChild.name == "PressInputToPlay")
                playerWaitingForInputUI = currentChild.gameObject;
            else
                UIs.Add(currentChild.gameObject);
            if(currentChild.name == "Scrollbar")
            {
                scrollBar = currentChild.GetComponent<Scrollbar>();
                scrollBar.value = colorValue[colorSelected];
            }
        }
        playerUIs = UIs.ToArray();
        switch (playerState)
        {
            case PlayerState.IsWaitingForPlayer:
                isWaitingPlayerInput = true;
                foreach(GameObject playerUI in playerUIs)
                {
                    if (playerUI.name == "ButtonREADY")
                    {
                        for (int i = 0; i < playerUI.transform.childCount; i++)
                        {
                            if (playerUI.transform.GetChild(i).name == "Ready")
                            {
                                spriteButton = playerUI.transform.GetChild(i).gameObject;
                                spriteButton.SetActive(true);
                            }
                            else if (playerUI.transform.GetChild(i).name == "Ready_Highlighted")
                            {
                                spriteButtonHighlighted = playerUI.transform.GetChild(i).gameObject;
                                spriteButtonHighlighted.SetActive(false);
                            }
                        }
                    }
                    playerUI.SetActive(false);
                }
                playerWaitingForInputUI.SetActive(true);
                break;
            case PlayerState.ColorSelection:
                currentGameObjectSelected = firstGameObjectSelected;
                foreach (GameObject playerUI in playerUIs)
                {
                    if (playerUI.name == "ButtonREADY")
                    {
                        for (int i = 0; i < playerUI.transform.childCount; i++)
                        {
                            if (playerUI.transform.GetChild(i).name == "Ready")
                            {
                                spriteButton = playerUI.transform.GetChild(i).gameObject;
                                spriteButton.SetActive(true);
                            }
                            else if (playerUI.transform.GetChild(i).name == "Ready_Highlighted")
                            {
                                spriteButtonHighlighted = playerUI.transform.GetChild(i).gameObject;
                                spriteButtonHighlighted.SetActive(false);
                            }
                        }
                    }
                    playerUI.SetActive(true);
                }
                playerWaitingForInputUI = null;
                break;
            default:
                Debug.Log("Player can't start with " + playerState.ToString() + " state");
                break;
        }
    }

    public void GoNextPlayerState()
    {
        switch (playerState)
        {
            case PlayerState.IsWaitingForPlayer:
                isWaitingPlayerInput = false;
                currentGameObjectSelected = firstGameObjectSelected;
                playerState = PlayerState.ColorSelection;
                foreach (GameObject playerUI in playerUIs)
                {
                    if (playerUI.name == "ButtonREADY")
                    {
                        for (int i = 0; i < playerUI.transform.childCount; i++)
                        {
                            if (playerUI.transform.GetChild(i).name == "Ready")
                            {
                                spriteButton.SetActive(true);
                            }
                            else if (playerUI.transform.GetChild(i).name == "Ready_Highlighted")
                            {
                                spriteButtonHighlighted.SetActive(false);
                            }
                        }
                    }
                    playerUI.SetActive(true);
                }
                playerWaitingForInputUI.SetActive(false);
                break;
            case PlayerState.ColorSelection:
                playerState = PlayerState.Ready;
                GoNextPlayerState();
                break;
            case PlayerState.Ready:
                playerState = PlayerState.IsReady;
                isReady = true;
                spriteButton.SetActive(false);
                spriteButtonHighlighted.SetActive(true);
                break;
            default:
                Debug.Log("Player can't go next after" + playerState.ToString() + " state");
                break;
        }
    }

    public void GoPreviousPlayerState()
    {
        switch (playerState)
        {
            case PlayerState.IsReady:
                playerState = PlayerState.ColorSelection;
                isReady = false;
                spriteButton.SetActive(true);
                spriteButtonHighlighted.SetActive(false);
                break;
            default:
                Debug.Log("Player can't go back before" + playerState.ToString() + " state");
                break;
        }
    }

    public void Reset()
    {
        this.currentGameObjectSelected = this.firstGameObjectSelected;
        switch (playerNumber)
        {
            case Player.PlayerNumber.J1:
            case Player.PlayerNumber.J2:
                this.playerState = PlayerState.ColorSelection;
                foreach (GameObject playerUI in playerUIs)
                {
                    if (playerUI.name == "ButtonREADY")
                    {
                        for(int i = 0; i < playerUI.transform.childCount;i++)
                        {
                            if(playerUI.transform.GetChild(i).name == "Ready")
                            {
                                spriteButton.SetActive(true);
                            }
                            else if (playerUI.transform.GetChild(i).name == "Ready_Highlighted")
                            {
                                spriteButtonHighlighted.SetActive(false);
                            }
                        }
                    }
                    playerUI.SetActive(true);
                }
                playerWaitingForInputUI = null;
                break;
            case Player.PlayerNumber.J3:
            case Player.PlayerNumber.J4:
                this.playerState = PlayerState.IsWaitingForPlayer;
                foreach (GameObject playerUI in playerUIs)
                {
                    playerUI.SetActive(false);
                }
                playerWaitingForInputUI.SetActive(true);
                break;
            default:
                break;
        }

        switch (playerState)
        {
            case PlayerState.IsWaitingForPlayer:
                isWaitingPlayerInput = true;
                break;
            case PlayerState.ColorSelection:
                currentGameObjectSelected = firstGameObjectSelected;
                break;
            default:
                Debug.Log("Player can't start with " + playerState.ToString() + " state");
                break;
        }
    }
}

[System.Serializable]
public class MainMenuNavigator
{
    public enum MainMenuState
    {
        Jouer,
        Options,
        Credits,
        Quit
    }
    public MainMenuState mainMenuState;

    public bool goNextStateAfterRelease;
    public bool isInputPressed;

    public const float timeInputPressedForNextState = 0.5f;
    [ReadOnly]
    public float timeInputPressed;

    public string inputJ1;
    public string inputJ2;

    [ReadOnly]
    public GameObject firstGameObjectSelected;
    [ReadOnly]
    public GameObject currentGameObjectSelected;

    public MainMenuNavigator(string inputJ1, string inputJ2, GameObject firstGameObjectSelected)
    {
        goNextStateAfterRelease = false;
        timeInputPressed = 0f;
        this.inputJ1 = inputJ1;
        this.inputJ2 = inputJ2;
        this.firstGameObjectSelected = firstGameObjectSelected;
        SetUpMainMenu();
    }

    public void SetUpMainMenu()
    {
        this.mainMenuState = MainMenuState.Jouer;
        this.currentGameObjectSelected = firstGameObjectSelected;
        currentGameObjectSelected.GetComponent<Button>().Select();
    }

    public void GoNextState()
    {
        switch(mainMenuState)
        {
            case MainMenuState.Jouer:
                currentGameObjectSelected.GetComponent<Button>().FindSelectableOnDown().Select();
                currentGameObjectSelected = currentGameObjectSelected.GetComponent<Button>().FindSelectableOnDown().gameObject;
                mainMenuState = MainMenuState.Options;
                break;
            case MainMenuState.Options:
                currentGameObjectSelected.GetComponent<Button>().FindSelectableOnDown().Select();
                currentGameObjectSelected = currentGameObjectSelected.GetComponent<Button>().FindSelectableOnDown().gameObject;
                mainMenuState = MainMenuState.Credits;
                break;
            case MainMenuState.Credits:
                currentGameObjectSelected.GetComponent<Button>().FindSelectableOnDown().Select();
                currentGameObjectSelected = currentGameObjectSelected.GetComponent<Button>().FindSelectableOnDown().gameObject;
                mainMenuState = MainMenuState.Quit;
                break;
            case MainMenuState.Quit:
                currentGameObjectSelected.GetComponent<Button>().FindSelectableOnDown().Select();
                currentGameObjectSelected = currentGameObjectSelected.GetComponent<Button>().FindSelectableOnDown().gameObject;
                mainMenuState = MainMenuState.Jouer;
                break;
            default:
                break;
        }
    }

    public void Reset()
    {
        this.isInputPressed = false;
        this.goNextStateAfterRelease = false;
        this.mainMenuState = MainMenuState.Jouer;
        this.currentGameObjectSelected = firstGameObjectSelected;
        currentGameObjectSelected.GetComponent<Button>().Select();
        Debug.Log("Main menu reseted");
    }
}

[System.Serializable]
public class OptionsMenuNavigator
{
    //Inputs
    public string inputMusique;
    public string inputFX;

    //Scrollbar infos
    public float musiqueVol;
    public float FXVol;

    //Canvas
    public Canvas canvas;

    //References
    public Scrollbar scrollBarMusique;
    public Scrollbar scrollBarFX;

    public OptionsMenuNavigator(string inputMusique,string inputFX,Canvas canvas, GameObject scrollBarMusique, GameObject scrollBarFX,float musiqueVol, float FXVol)
    {
        this.inputMusique = inputMusique;
        this.inputFX = inputFX;
        this.canvas = canvas;
        this.scrollBarMusique = scrollBarMusique.GetComponent<Scrollbar>();
        this.scrollBarFX = scrollBarFX.GetComponent<Scrollbar>();
        this.musiqueVol = musiqueVol;
        this.FXVol = FXVol;
        this.scrollBarMusique.value = musiqueVol;
        this.scrollBarFX.value = FXVol;
    }

    public void MusiqueNextValue()
    {
        if (musiqueVol == 1.0f)
        {
            musiqueVol = 0.0f;
        }
        else if (musiqueVol == 0.0f)
        {
            musiqueVol = 0.25f;
        }
        else if (musiqueVol == 0.25f)
        {
            musiqueVol = 0.5f;
        }
        else if (musiqueVol == 0.5f)
        {
            musiqueVol = 0.75f;
        }
        else if (musiqueVol == 0.75f)
        {
            musiqueVol = 1.0f;
        }
        this.scrollBarMusique.value = musiqueVol;
    }

    public void FXNextValue()
    {
        if (FXVol == 1.0f)
        {
            FXVol = 0.0f;
        }
        else if(FXVol == 0.0f)
        {
            FXVol = 0.25f;
        }
        else if (FXVol == 0.25f)
        {
            FXVol = 0.5f;
        }
        else if (FXVol == 0.5f)
        {
            FXVol = 0.75f;
        }
        else if (FXVol == 0.75f)
        {
            FXVol = 1.0f;
        }
        this.scrollBarFX.value = FXVol;
    }
}