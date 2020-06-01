using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class ShowWindow : MonoBehaviour
    {
        #region Fields
        [Header("References")]
        [SerializeField]
        protected Menu window = null;
        #endregion

        #region Methods
        public virtual void OpenWindow()
        {
            this.window.SetVisibility(true);
        }

        public virtual void CloseWindow()
        {
            this.window.SetVisibility(false);
        }
        #endregion
    }
}