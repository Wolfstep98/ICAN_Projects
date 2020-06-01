using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour {

    public MainGameManager mainGameManager;

    public void ChangeGameState(int gameState)
    {
        mainGameManager.ChangeGameState(gameState);
    }
}
