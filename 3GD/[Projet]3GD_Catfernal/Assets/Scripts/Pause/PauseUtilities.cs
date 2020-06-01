using System.Collections;
using System.Collections.Generic;
using DesignPattern;
using UnityEngine;

namespace Game.Pause
{
    [AddComponentMenu("Game/Utilities/Pause")]
    public class PauseUtilities : MonoSingleton<PauseUtilities> {
        
        #region Fields

        public static bool gameIsPaused = false;

        [SerializeField] private GameObject pausePanel;
        
        #endregion

        #region Contructors
        public PauseUtilities()
        {

        }
        #endregion

        #region Methods

        public void SetPausePanel(GameObject panel) {
            pausePanel = panel;
        }

        public void PauseGame() {
            if (gameIsPaused){
                gameIsPaused = !gameIsPaused;
                pausePanel.SetActive(gameIsPaused);
            }
            else{
                gameIsPaused = !gameIsPaused;
                pausePanel.SetActive(gameIsPaused);
            }
        }
        #endregion
    }
}