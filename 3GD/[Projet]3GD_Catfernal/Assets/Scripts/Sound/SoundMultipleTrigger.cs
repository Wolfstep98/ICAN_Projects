using System;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

namespace Game.Sound
{
    [AddComponentMenu("Game/Sound/Multiple Trigger")]
    public class SoundMultipleTrigger : MonoBehaviour
    {
        #region Fields
        [Header("References")]
        [SerializeField] [EventRef] private string sound;
        private EventInstance soundInstance;

        [Header("Parameters")]
        [SerializeField] private float timeBeforeSoundPlay = 10.0f;
        private float timer = 0.0f;
        #endregion

        #region Init
        private void Awake()
        {
            this.timer = 0.0f;
            this.soundInstance = RuntimeManager.CreateInstance(this.sound);
                
        }
        #endregion

        #region Properties

        #endregion

        #region Methods
        private void Update()
        {
            this.timer += Time.deltaTime;

            if(this.timer >= this.timeBeforeSoundPlay)
            {
                Debug.Log("[Sound] - Play sound");
                this.soundInstance.start();
                this.timer -= this.timeBeforeSoundPlay;
            }
        }
        #endregion
    }
}