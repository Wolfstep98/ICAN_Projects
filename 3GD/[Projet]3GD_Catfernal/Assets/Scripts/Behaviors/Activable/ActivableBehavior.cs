using System;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Behaviors
{
    [Serializable]
    public abstract class ActivableBehavior : GameBehavior, IActivable
    {
        #region Class
        [Serializable]
        public class OnActivatedEvent : UnityEvent
        {
            public OnActivatedEvent() : base() { }
        }
        [Serializable]
        public class OnDesactivatedEvent : UnityEvent
        {
            public OnDesactivatedEvent() : base() { }
        }
        #endregion

        #region Fields
        protected bool isActivated = false;
        #endregion

        #region Properties
        public virtual bool IsActivated { get { return this.isActivated; } }

        [Header("Events")]
        public OnActivatedEvent onActivated = null;
        public OnDesactivatedEvent onDesactivated = null;
        #endregion

        #region Methods
        public virtual void Activate()
        {
            this.isActivated = true;
            if (this.onActivated != null)
                this.onActivated.Invoke();
        }

        public virtual void Desactivate()
        {
            this.isActivated = false;
            if (this.onDesactivated != null)
                this.onDesactivated.Invoke();
        }
        #endregion
    }
}
