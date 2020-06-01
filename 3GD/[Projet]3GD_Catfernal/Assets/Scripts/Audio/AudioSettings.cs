using System;
using UnityEngine;
using DesignPattern;
using FMOD.Studio;
using UnityEngine.UI;

namespace Game.Audio
{
    public class AudioSettings : MonoBehaviour {
        #region Fields

        [Header("References")] 
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sFXSlider;

        private Bus master;
        private Bus music;
        private Bus sFX;
        private float masterVolume = 1.0f;
        private float musicVolume = 1.0f;
        private float sFXVolume = 1.0f;

        #endregion

        #region Init

        private void Awake() {
            master = FMODUnity.RuntimeManager.GetBus("bus:/Master");
            music = FMODUnity.RuntimeManager.GetBus("bus:/Master/Music");
            sFX = FMODUnity.RuntimeManager.GetBus("bus:/Master/Sfx");

            masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
            musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
            sFXVolume = PlayerPrefs.GetFloat("SfxVolume", 1.0f);

            masterSlider.value = masterVolume;
            musicSlider.value = musicVolume;
            sFXSlider.value = sFXVolume;
        }

        #endregion

        #region Methods

        public void SaveVolumes() {
            PlayerPrefs.SetFloat("MasterVolume", masterVolume);
            PlayerPrefs.SetFloat("MusicVolume", musicVolume);
            PlayerPrefs.SetFloat("SfxVolume", sFXVolume);
            PlayerPrefs.Save();
        }

        public void SetMasterVolume(float newVolume) {
            masterVolume = newVolume;
            master.setVolume(masterVolume);
        }
        
        public void SetMusicVolume(float newVolume) {
            musicVolume = newVolume;
            music.setVolume(masterVolume);
        }
        
        public void SetVFXVolume(float newVolume) {
            sFXVolume = newVolume;
            sFX.setVolume(masterVolume);
        }
        
        #endregion
    }
}
