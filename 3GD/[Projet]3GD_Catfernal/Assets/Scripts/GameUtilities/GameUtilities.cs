using System;
using UnityEngine;
using DesignPattern;

namespace Game
{
    [AddComponentMenu("Game/Utilities/Game")]
    public class GameUtilities : MonoSingleton<GameUtilities>
    {
        #region Fields

        #endregion

        #region Init
        #endregion

        #region Properties

        #endregion

        #region Methods
        public void QuitGame()
        {
            Application.Quit();
        }
        #endregion
    }
}