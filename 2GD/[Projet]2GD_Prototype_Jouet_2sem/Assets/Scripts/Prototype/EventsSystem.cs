using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsSystem : MonoBehaviour {

    //Instance
    public static EventsSystem instance = null;

    public bool eventsSystemActive = true;
    public bool soundEventPlayed = false;

	[FMODUnity.EventRef]
	public string black2_state;

	[FMODUnity.EventRef]
	public string power2_state;

	private FMOD.Studio.EventInstance black2_fmod;
	private FMOD.Studio.EventInstance power2_fmod;
    #region Variables

    public enum GameEvents
    {
        None,
        Blackout,
        BouncingBall,
        Warp
    }
    public GameEvents currentGameEvents;

    //Timers
    public bool isEventActive;
    public float timeBetweenEvents;
    public float eventDuration;
    [ReadOnly]
    public float currentTimer;

    //Bouncing Ball
    public GameObject bouncingBallPrefab;
    [ReadOnly]
    public GameObject bouncingBallInstance;

    public GameObject[] bouncingBallSpawners;

    //References
    public MainGameManager mainGameManager;
    public GameManager gameManager;

    #endregion

    private void Awake()
    {
        //Create Singleton
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        currentGameEvents = GameEvents.None;

        //Assign the references
        mainGameManager = GameObject.Find("MainGameManager").GetComponent<MainGameManager>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        //Set the prefabs
        bouncingBallPrefab = Resources.Load<GameObject>("Prefabs/Final/2D/Events/BouncingBall");

        //Find spawners
        bouncingBallSpawners = GameObject.FindGameObjectsWithTag("Spawner");
    }

    private void Start()
    {
		black2_fmod = FMODUnity.RuntimeManager.CreateInstance(black2_state);
		black2_fmod.setVolume (0.5f);
		power2_fmod = FMODUnity.RuntimeManager.CreateInstance(power2_state);
		power2_fmod.setVolume (2f);

        //Declare the variables
        isEventActive = false;
        currentTimer = 0f;
    }

    private void Update()
    {
        if (eventsSystemActive)
        {
            currentTimer += Time.deltaTime;
            if (currentTimer >= timeBetweenEvents)
            {
                ActivateRandomEvent();
                isEventActive = true;
                currentTimer = 0.0f;
            }
            if (!isEventActive && currentTimer >= timeBetweenEvents - 2.0f && !soundEventPlayed)
            {
                soundEventPlayed = true;
                power2_fmod.start();
            }
            if (isEventActive && currentTimer >= eventDuration)
            {
                DeactivateCurrentEvent();
                isEventActive = false;
                soundEventPlayed = false;
            }
        }
    }

    //Choose a random event between all the events
    public void ActivateRandomEvent()
    {
        float value = Random.value;
        if(value < 0.5f)
            ActivateBlackoutEvent();
        else
            ActivateBouncingBallEvent();
        if(soundEventPlayed)
            soundEventPlayed = false;
    }
    //Deactivate the current event
    public void DeactivateCurrentEvent()
    {
        switch(currentGameEvents)
        {
            case GameEvents.Blackout:
                DeactivateBlackoutEvent();
                break;
            case GameEvents.BouncingBall:
                DeactivateBouncingBallEvent();
                break;
            case GameEvents.Warp:
                //DeactivateBlackoutEvent();
                break;
            default:
                Debug.Log("No current event is active");
                break;
        }
    }

    //Activate/Deactivate the Blackout event
    public void ActivateBlackoutEvent()
    {
		black2_fmod.start();
		foreach(GameObject player in gameManager.players)
		{
			Player script = player.GetComponent<Player>();
			script.black_fmod.setValue (1f);
		}
        currentGameEvents = GameEvents.Blackout;
        foreach(GameObject circle in gameManager.circles)
        {
            Circle script = circle.GetComponent<Circle>();
            script.ActivateBlackout();
        }
        foreach(GameObject lien in gameManager.lienUpdater.liens)
        {
            Lien script = lien.GetComponent<Lien>();
            script.DeactivateLink(eventDuration);
        }
        if (soundEventPlayed)
            soundEventPlayed = false;
    }

    public void DeactivateBlackoutEvent()
    {
		foreach(GameObject player in gameManager.players)
		{
			Player script = player.GetComponent<Player>();
			script.black_fmod.setValue (0f);
		}
        currentGameEvents = GameEvents.None;
        foreach (GameObject circle in gameManager.circles)
        {
            Circle script = circle.GetComponent<Circle>();
            script.DeactivateBlackout();
        }
        foreach(GameObject player in gameManager.players)
        {
            Player script = player.GetComponent<Player>();
            script.UpdateCircleColored();
        }
        foreach (GameObject lien in gameManager.lienUpdater.liens)
        {
            Lien script = lien.GetComponent<Lien>();
            script.ActivateLink();
        }
        gameManager.lienUpdater.UpdateLinks();
    }

    //Activate/Deactivate the Bouncing Ball event
    public void ActivateBouncingBallEvent()
    {
		foreach(GameObject player in gameManager.players)
		{
			Player script = player.GetComponent<Player>();
			script.melo5_fmod.setValue (0.5f);
		}
        currentGameEvents = GameEvents.BouncingBall;
        int randomIndex = Random.Range(0, bouncingBallSpawners.Length);
        bouncingBallInstance = Instantiate<GameObject>(bouncingBallPrefab, bouncingBallSpawners[randomIndex].transform.position, Quaternion.identity);
        if (soundEventPlayed)
            soundEventPlayed = false;
    }

    public void DeactivateBouncingBallEvent()
    {
		foreach(GameObject player in gameManager.players)
		{
			Player script = player.GetComponent<Player>();
			script.melo5_fmod.setValue (0f);
		}
        currentGameEvents = GameEvents.None;
        Destroy(bouncingBallInstance);
    }

    //Activate/Deactivate the Warp event
    public void ActivateWarpEvent()
    {
        currentGameEvents = GameEvents.Warp;
        if (soundEventPlayed)
            soundEventPlayed = false;
    }

    public void  DeactivateWarpEvent()
    {
        currentGameEvents = GameEvents.None;
    }
}
