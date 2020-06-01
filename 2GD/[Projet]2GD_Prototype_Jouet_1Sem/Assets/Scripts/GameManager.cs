using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    //public Button playButton;
    //public Button quitButton;
    //public Button restartButton;

    public Canvas menuMain;
    public Canvas menuPause;

    public Camera menuCamera;

    public GameObject playerPrefab;
    private GameObject playerInstance;

    public enum gameStates
    {
        Main,
        Play,
        Restart,
        Pause,
        Quit
    }
    public gameStates gameState;

	// Use this for initialization
	void Start () {
        SetGameState(gameStates.Main);
        playerInstance = null;
    }

    private void Update()
    {
        
    }

    public void SetGameState(gameStates state)
    {
        if(state == gameStates.Main)
        {
            Time.timeScale = 1f;
            menuMain.gameObject.SetActive(true);
            menuPause.gameObject.SetActive(false);
            menuCamera.gameObject.SetActive(true);
            if(playerInstance != null)
            {
                Destroy(playerInstance);
            }
            gameState = state;
        }
        else if (state == gameStates.Play)
        {
            Time.timeScale = 1f;
            menuMain.gameObject.SetActive(false);
            menuPause.gameObject.SetActive(false);
            if(gameState == gameStates.Main)
            {
                playerInstance = Instantiate(playerPrefab, new Vector3(0f, 3f, 27.94f), Quaternion.identity);
                menuCamera.gameObject.SetActive(false);
            }
            gameState = state;
        }
        else if (state == gameStates.Restart)
        {
            SceneManager.LoadScene(0,LoadSceneMode.Single);
        }
        else if (state == gameStates.Pause)
        {
            Time.timeScale = 0f;
            menuMain.gameObject.SetActive(false);
            menuPause.gameObject.SetActive(true);
            gameState = state;
        }
        else if (state == gameStates.Quit)
        {
            Application.Quit();
        }
    }
    public void SetGameState(int state)
    {
        if ((gameStates)state == gameStates.Main)
        {
            SetGameState(gameStates.Main);
        }
        else if ((gameStates)state == gameStates.Play)
        {
            SetGameState(gameStates.Play);
        }
        else if ((gameStates)state == gameStates.Restart)
        {
            SetGameState(gameStates.Restart);
        }
        else if ((gameStates)state == gameStates.Pause)
        {
            SetGameState(gameStates.Pause);
        }
        else if ((gameStates)state == gameStates.Quit)
        {
            SetGameState(gameStates.Quit);
        }
    }

}
