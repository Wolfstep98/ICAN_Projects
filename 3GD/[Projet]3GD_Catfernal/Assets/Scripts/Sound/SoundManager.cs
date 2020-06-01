using System;
using UnityEngine;
using DesignPattern;
using FMODUnity;

namespace Game.Sound
{
    [Obsolete("Use Menu/Game Sound Manager instead !")]
    [AddComponentMenu("Game/Sound/Sound Manager")]
    public class SoundManager : MonoSingleton<SoundManager>
    {
        #region Fields
        [Header("Sound References")]
        [SerializeField] [EventRef] private string menuMusic = null;
        #endregion

        #region Init

        #endregion

        #region Properties

        #endregion

        #region Methods

        #endregion
    }
}