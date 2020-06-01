using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Behaviors
{
    [AddComponentMenu("Game/Behavior/Multiple Activable")]
    public class MultipleActivableBehavior : ActivableBehavior
    {
        #region Fields
        [SerializeField] private ActivableBehavior[] activableBehaviors = null;
        #endregion

        #region Properties
        public override bool IsActivated { get { return this.IsActivableBehaviorsActive(); } }
        public virtual int ActivableBehaviorNumber { get { return this.activableBehaviors.Length; } }
        #endregion

        #region Methods
        public override void CustomAwake()
        {
            for(int i = 0; i < this.activableBehaviors.Length;i++)
            {
                this.activableBehaviors[i].onActivated.AddListener(this.OnActivableActivated);
                this.activableBehaviors[i].onDesactivated.AddListener(this.OnActivableDesactivated);
            }
        }

        public int GetActivatedNumber()
        {
            if (this.isActivated) return this.activableBehaviors.Length;

            int activated = 0;
            for (int i = 0; i < this.activableBehaviors.Length; i++)
            {
                if (this.activableBehaviors[i].IsActivated) activated++;
            }
            return activated;
        }

        private void OnActivableActivated()
        {
            bool activated = this.IsActivableBehaviorsActive();
            if(activated)
            {
                this.Activate();
            }
        }

        private void OnActivableDesactivated()
        {
            if(this.isActivated)
            {
                this.Desactivate();
            }
        }

        private bool IsActivableBehaviorsActive()
        {
            for(int i = 0; i < this.activableBehaviors.Length;i++)
            {
                if (!this.activableBehaviors[i].IsActivated)
                    return false;
            }
            return true;
        }

        public override void Activate()
        {
            this.isActivated = true;

            for (int i = 0; i < this.activableBehaviors.Length; i++)
            {
                if(!this.activableBehaviors[i].IsActivated)
                    this.activableBehaviors[i].Activate();
            }

            base.Activate();
        }

        public override void Desactivate()
        {
            this.isActivated = false;

            for (int i = 0; i < this.activableBehaviors.Length; i++)
            {
                if (this.activableBehaviors[i].IsActivated)
                    this.activableBehaviors[i].Desactivate();
            }

            base.Desactivate();
        }

        #endregion
    }
}
