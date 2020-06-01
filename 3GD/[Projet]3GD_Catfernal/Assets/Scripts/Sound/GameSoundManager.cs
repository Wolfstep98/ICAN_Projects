using System;
using UnityEngine;
using DesignPattern;
using FMODUnity;
using FMOD;
using FMOD.Studio;

namespace Game.Sound
{
    [AddComponentMenu("Game/Sound/Game Sound Manager")]
    public class GameSoundManager : MonoSingleton<GameSoundManager>
    {
        #region Fields
        [Header("Sound References")]
        [SerializeField] [EventRef] private string ambianceMusic = null;

        [Header("FlameThrower")]
        [SerializeField] [EventRef] private string spraySound = null; // Loop
        private EventInstance sprayInstance;
        [SerializeField] [EventRef] private string lazerSound = null; // Loop
        private EventInstance lazerInstance;
        [SerializeField] [EventRef] private string slowCooldownSound = null; // Loop
        private EventInstance slowCooldownInstance;
        [SerializeField] [EventRef] private string switchSound = null; // One shot
        [SerializeField] [EventRef] private string backfireSound = null; // One shot
        [SerializeField] [EventRef] private string cooldownSound = null; // Loop
        private EventInstance cooldownInstance;
        [SerializeField] [EventRef] private string triggerSound = null; // One shot
        #endregion

        #region Init
        private void Awake()
        {
            this.InitFlameThrower();
        }

        private void InitFlameThrower()
        {
            this.InitSpray();
            this.InitLazer();
            this.InitCooldown();
            this.InitSlowCooldown();
        }

        private void InitSpray()
        {
            this.sprayInstance = RuntimeManager.CreateInstance(this.spraySound);
        }
        private void InitLazer()
        {
            this.lazerInstance = RuntimeManager.CreateInstance(this.lazerSound);
        }
        private void InitSlowCooldown()
        {
            this.slowCooldownInstance = RuntimeManager.CreateInstance(this.slowCooldownSound);
        }
        private void InitCooldown()
        {
            this.cooldownInstance = RuntimeManager.CreateInstance(this.cooldownSound);
        }
        #endregion

        #region Properties

        #endregion

        #region Methods
        #region FlameThrower
        // --- Spray
        public void PlaySpray()
        {
            this.PlayInstance(ref this.sprayInstance);
            UnityEngine.Debug.Log("[Game Sound Manager] - Start spray.");
        }
        public void StopSpray()
        {
            this.StopInstance(ref this.sprayInstance, FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            UnityEngine.Debug.Log("[Game Sound Manager] - Stop spray.");
        }

        // --- Lazer
        public void PlayLazer()
        {
            this.PlayInstance(ref this.lazerInstance);
            UnityEngine.Debug.Log("[Game Sound Manager] - Start lazer.");
        }
        public void StopLazer()
        {
            this.StopInstance(ref this.lazerInstance, FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            UnityEngine.Debug.Log("[Game Sound Manager] - Stop lazer.");
        }

        // --- SlowCooldown
        public void PlaySlowCooldown()
        {
            this.PlayInstance(ref this.slowCooldownInstance);
            UnityEngine.Debug.Log("[Game Sound Manager] - Start slow cooldown.");
        }
        public void StopSlowCooldown()
        {
            this.StopInstance(ref this.slowCooldownInstance, FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            UnityEngine.Debug.Log("[Game Sound Manager] - Stop slow cooldown.");
        }

        // --- Switch
        public void PlaySwitch()
        {
            RuntimeManager.PlayOneShot(this.switchSound);
            UnityEngine.Debug.Log("[Game Sound Manager] - Play switch.");
        }

        // --- Backfire
        public void PlayBackfire()
        {
            RuntimeManager.PlayOneShot(this.backfireSound);
            UnityEngine.Debug.Log("[Game Sound Manager] - Play backfire.");
        }

        // --- Cooldown
        public void PlayCooldown()
        {
            this.PlayInstance(ref this.cooldownInstance);
            UnityEngine.Debug.Log("[Game Sound Manager] - Play cooldown.");
        }
        public void StopCooldown()
        {
            this.StopInstance(ref this.cooldownInstance, FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            UnityEngine.Debug.Log("[Game Sound Manager] - Stop cooldown.");
        }

        // --- Trigger
        public void PlayTrigger()
        {
            RuntimeManager.PlayOneShot(this.triggerSound);
            UnityEngine.Debug.Log("[Game Sound Manager] - Play trigger.");
        }
        #endregion


        #region Play & Stop Instance
        /// <summary>
        /// Play a given <paramref name="eventInstance"/>.
        /// </summary>
        /// <param name="eventInstance">The event instance to play.</param>
        private void PlayInstance(ref EventInstance eventInstance)
        {
            RESULT result = eventInstance.start();
            if (result != RESULT.OK)
            {
                UnityEngine.Debug.LogError("[Play Sound " + eventInstance.ToString() + "] - Can't play sound instance : " + result.ToString());
            }
        }

        /// <summary>
        /// Stop a given <paramref name="eventInstance"/> with a <paramref name="stopMode"/> parameter.
        /// </summary>
        /// <param name="eventInstance">The event instance to stop.</param>
        /// <param name="stopMode">The stop mode for the sound played.</param>
        private void StopInstance(ref EventInstance eventInstance, FMOD.Studio.STOP_MODE stopMode = FMOD.Studio.STOP_MODE.IMMEDIATE)
        {
            RESULT result = eventInstance.stop(stopMode);
            if (result != RESULT.OK)
            {
                UnityEngine.Debug.LogError("[Stop Sound " + eventInstance.ToString() + "] - Can't play sound instance : " + result.ToString());
            }
        }
        #endregion
        #endregion
    }
}