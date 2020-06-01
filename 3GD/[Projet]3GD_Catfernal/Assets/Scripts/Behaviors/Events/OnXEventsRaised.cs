using System;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{

    [AddComponentMenu("Game/Behavior/On X Events Raised")]
    public class OnXEventsRaised : GameBehavior
    {
        #region Class
        [Serializable]
        public class OnEventsRaisedEvent : UnityEvent
        {
            public OnEventsRaisedEvent() : base() { }
        }
        #endregion

        #region Fields

        [Header("References")]
        [SerializeField] private Transform player;
        [SerializeField] private GameObject[] lights;
        [SerializeField] private float distanceToShowLights = 30;
        
        [Header("Parameters")]
        [SerializeField] private int numberOfEventsRaised = 1;
        private int currentNumberOfEventsRaised = 0;

        [Header("Events")]
        [SerializeField] private OnEventsRaisedEvent onEventsRaised = null;

        private bool lightsOn = false;
        #endregion

        #region Methods
        public override void CustomAwake()
        {
            this.currentNumberOfEventsRaised = 0;
        }

        public override void CustomUpdate() {
            if (this.currentNumberOfEventsRaised >= this.numberOfEventsRaised && Vector3.Distance(this.transform.position, player.position) < distanceToShowLights && !this.lightsOn){
                foreach (var o in lights){
                    o.SetActive(true);
                }
                this.lightsOn = true;
            }
            
            if(Vector3.Distance(this.transform.position, player.position) > distanceToShowLights && this.lightsOn){
                foreach (var o in lights){
                    o.SetActive(false);
                }
                this.lightsOn = false;
            }
        }

        public void OnEventRaised()
        {
            this.currentNumberOfEventsRaised++;

            if (this.currentNumberOfEventsRaised >= this.numberOfEventsRaised)
            {
                this.onEventsRaised?.Invoke();

                foreach (var o in lights){
                    o.SetActive(true);
                }
                this.lightsOn = true;
                
                this.enabled = false;
            }
        }
        #endregion
    }
}