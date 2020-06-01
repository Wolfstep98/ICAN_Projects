using System;
using UnityEngine;

namespace Game.Behaviors
{

    public abstract class Flammable : GameBehavior, IFlammable
    {
        #region Events
        public delegate void OnBurningEvent();
        protected event OnBurningEvent onBurning;
        #endregion

        #region Fields
        protected bool isBurning = false;

        [Header("Parameters")]
        [SerializeField] protected int hitBeforeBurning = 50;
        protected int currentHits = 0;
        [SerializeField] protected float burningTick = 0.2f;
        protected float currentBurningTick = 0.0f;

        [Header("References")]
        [SerializeField] protected ParticleSystem burningParticleSystem = null;

        #endregion

        #region Init
        public void Initialize(bool isAlreadyBurning = false)
        {
            this.isBurning = isAlreadyBurning;
            if(this.isBurning)
            {
                this.currentHits = this.hitBeforeBurning;
            }
            else
            {
                this.currentHits = 0;
            }
        }
        #endregion

        #region Properties
        public bool IsBurning { get { return this.isBurning; } }
        public OnBurningEvent OnBurning { get { return this.onBurning; } set { this.onBurning = value; } }
        #endregion

        #region Methods
        public override void CustomUpdate()
        {
            if(this.isBurning)
            {
                this.OnBurningStay();
            }
        }

        public virtual void Burn(int amount)
        {
            this.currentHits += Math.Abs(amount);

            if(!this.isBurning && this.currentHits >= this.hitBeforeBurning)
            {
                this.Burn();
            }
        }

        protected virtual void Burn()
        {
            this.isBurning = true;
            this.burningParticleSystem.Play();
            Debug.Log("[Flammable " + this.name + "] - Is burning.");
        }

        protected virtual void OnBurningStay()
        {
            this.currentBurningTick += GameTime.deltaTime;

            if(this.currentBurningTick >= this.burningTick)
            {
                this.currentBurningTick -= this.burningTick;
                this.BurnTick();
            }
        }

        protected virtual void BurnTick()
        {
            
        }

        protected virtual void OnParticleCollision(GameObject other)
        {
            if(other.tag.Contains(Constants.GameObjectTags.Flame))
            {
                this.Burn(1);
            }
        }

        private void OnDisable()
        {
            if(this.burningParticleSystem.isPlaying)
            {
                this.burningParticleSystem.Stop(false, ParticleSystemStopBehavior.StopEmitting);
            }
        }
        #endregion
    }
}