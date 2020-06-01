using System;
using UnityEngine;

namespace GameEvents
{
    [Serializable]
    public class StickEventArgs : EventArgs
    {
        #region Fields & Properties
        [SerializeField]
        private GameObject stickObject;
        public GameObject StickObject { get { return this.stickObject; } }
        #endregion

        #region Constructors
        public StickEventArgs(GameObject stickObject)
        {
            this.stickObject = stickObject;
        }
        #endregion
    }
}
