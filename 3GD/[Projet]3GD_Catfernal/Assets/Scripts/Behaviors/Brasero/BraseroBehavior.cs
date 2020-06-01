using System;
using UnityEngine;
using Game.Constants;

namespace Game.Behaviors
{

    [AddComponentMenu("Game/Behavior/Brasero")]
    public class BraseroBehavior : ActivableBehavior
    {
        #region Fields

        [Header("References")] 
        [SerializeField] private Transform player;
        [SerializeField] private ParticleSystem firePS = null;
        [SerializeField] private ParticleSystem fire2 = null;
        [SerializeField] private GameObject[] lights;
        [SerializeField] private float distanceToShowLights = 30;

        private bool lightsOn = false;
        #endregion

        #region Init

        #endregion

        #region Properties

        #endregion

        #region Methods
        public override void Activate()
        {
            base.Activate();

            foreach (var o in lights){
                o.SetActive(true);
            }
            this.lightsOn = true;
            
            //Activate particleSystem
            this.firePS.Play(true);
            this.fire2.Play(true);
        }

        public override void CustomUpdate() {
            if (this.isActivated && Vector3.Distance(this.transform.position, player.position) < distanceToShowLights && !this.lightsOn){
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

        public override void Desactivate()
        {
            base.Desactivate();

            this.firePS.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            this.fire2.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        private void OnParticleCollision(GameObject other)
        {
            if (!this.isActivated)
            {
                if (other.gameObject.tag.Contains(GameObjectTags.Flame))
                {
                    this.Activate();
                }
            }
        }
        #endregion
    }
}