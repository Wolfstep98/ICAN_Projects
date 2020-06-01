using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public interface IGameBehavior
    {
        #region Methods
        void CustomAwake();
        void CustomUpdate();
        void CustomFixedUpdate();
        #endregion
    }

    [Serializable]
    public abstract class GameBehavior : MonoBehaviour, IGameBehavior
    {
        #region Methods
        public virtual void CustomAwake() { }
        public virtual void CustomUpdate() { }
        public virtual void CustomFixedUpdate() { }
        #endregion
    }
}
