using System;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Behaviors
{
    [AddComponentMenu("Game/Behavior/On Trigger Enter")]
    public class OnTriggerEnterWithPlayerEvent : GameBehavior
    {
        #region Events
        [Serializable]
        public class OnTriggerEnter2DEvent : UnityEvent
        {
            public OnTriggerEnter2DEvent() : base()
            {

            }
        }
        #endregion

        #region Fields
        [Header("Events")]
        [SerializeField] private OnTriggerEnter2DEvent TriggerEnter2DEvent;
        #endregion

        #region Init

        #endregion

        #region Properties

        #endregion

        #region Methods
        protected virtual void OnTriggerEnter()
        {
            this.TriggerEnter2DEvent?.Invoke();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag.Contains(Constants.GameObjectTags.Player))
            {
                this.OnTriggerEnter();
            }
        }
        #endregion
    }
}