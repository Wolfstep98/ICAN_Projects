using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using DesignPattern;

namespace Game.Level
{
    [AddComponentMenu("Game/Utilities/Level")]
    [RequireComponent(typeof(Animator))]
    public class LevelUtilities : MonoSingleton<LevelUtilities>
    {
        #region Fields

        [SerializeField] private Animator animator;
        
        private static readonly int FadeOut = Animator.StringToHash("FadeOut");
        private int levelToLoad;
        
        
        #endregion

        #region Contructors
        public LevelUtilities()
        {

        }
        #endregion

        #region Properties

        #endregion

        #region Methods
        
        
        
        public void LoadLevel(int levelIndex) {
            animator.SetTrigger(FadeOut);
            levelToLoad = levelIndex;
        }

        public void OnFadeComplete() {
            FMODUnity.RuntimeManager.GetBus("bus:/Master").stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
            SceneManager.LoadScene(levelToLoad, LoadSceneMode.Single);
        }
        
        #endregion
    }
}