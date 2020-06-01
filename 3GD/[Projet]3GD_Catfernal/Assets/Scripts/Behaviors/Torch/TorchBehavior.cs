using System;
using UnityEngine;
using Game.Constants;

namespace Game.Behaviors
{
    [RequireComponent(typeof(Collider2D))]
    public class TorchBehavior : ActivableBehavior
    {
        #region Fields
        [Header("References")]
        [SerializeField] private LightsData lightsData = null;

        [Header("Parameters")]
        [SerializeField] private float activationTime = 2.0f;
        private float activationTimer = 0.0f;
        [SerializeField] private Vector2 lightSize = Vector2.zero;
        #endregion

        #region Methods
        public override void CustomAwake()
        {
            this.UpdateLight(false);
        }

        private void UpdateLight(bool enable)
        {
            this.lightsData.Size = enable ? this.lightSize : Vector2.zero;
        }

        public override void CustomUpdate()
        {
            if (this.isActivated)
            {
                this.activationTimer += GameTime.deltaTime;
                if (this.activationTimer >= this.activationTime)
                {
                    this.Desactivate();
                }
            }
        }

        public override void Activate()
        {
            this.isActivated = true;
            this.activationTimer = 0.0f;
            this.UpdateLight(true);

            base.Activate();
        }

        public override void Desactivate()
        {
            this.isActivated = false;
            this.activationTimer = 0.0f;
            this.UpdateLight(false);

            base.Desactivate();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            
        }
        #endregion
    }

}
