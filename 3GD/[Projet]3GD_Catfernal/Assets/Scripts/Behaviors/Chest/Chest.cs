using System;
using UnityEngine;
using Game.Entities.Swarm;
using Game.Entities.Player;

namespace Game.Behaviors
{

    [AddComponentMenu("Game/Behavior/Chest")]
    public class Chest : Flammable
    {
        #region Fields
        protected bool isChestOpen = false;

        [Header("Parameters")]
        [SerializeField] private int life = 300;
        [SerializeField] private int lifeLostWhileBurningPerTick = 1;

        [Header("KnockBack")]
        [SerializeField] private float knockBackForce = 2.0f;
        [SerializeField] private float knockBackRadius = 5.0f;
        [SerializeField] private LayerMask knockBackLayerMask;

        private float disableTime = 1.0f;
        private float disableTimer = 0.0f;

        [Header("References")]
        [SerializeField] private HeartSwarmPooler heartSwarmPooler = null;
        [SerializeField] private ParticleSystemForceField forceField = null;
        [SerializeField] private GameObject explosionObject = null;
        #endregion

        #region Init

        #endregion

        #region Properties

        #endregion

        #region Methods
        public override void CustomUpdate()
        {
            base.CustomUpdate();

            if(this.isChestOpen)
            {
                this.disableTimer += GameTime.deltaTime;

                if(this.disableTimer >= this.disableTime)
                {
                    this.gameObject.SetActive(false);
                }
            }
        }

        protected void OpenChest()
        {
            // Knockback Player
            ContactFilter2D filter2D = new ContactFilter2D();
            this.explosionObject.SetActive(true);
            filter2D.SetLayerMask(this.knockBackLayerMask);
            Collider2D[] results = new Collider2D[2];
            int collisions = Physics2D.OverlapCircle(this.transform.position, this.knockBackRadius, filter2D, results);
            if(collisions > 0)
            {
                PlayerController playerController = results[0].GetComponent<PlayerController>();
                if(playerController != null)
                {
                    Vector2 direction = playerController.transform.position - this.transform.position;
                    direction.Normalize();
                    direction *= this.knockBackForce;
                    playerController.AddBackfireForce(direction);
                }
            }

            // Spawn HeartSwarm
            HearthSwarmBehavior hearthSwarmBehavior = this.heartSwarmPooler.GetEntity();
            hearthSwarmBehavior.transform.position = this.transform.position;
            hearthSwarmBehavior.SetupHeartSwarm();

            // Activate Force Field
            this.forceField.gameObject.SetActive(true);

            // Desactivate Chest
            this.isChestOpen = true;
        }

        protected void RemoveLife(int amount)
        {
            this.life -= amount;
            if (this.life <= 0)
            {
                this.life = 0;
                if(!this.isChestOpen)
                    this.OpenChest();
            }
        }
        public override void Burn(int amount)
        {
            base.Burn(amount);

            this.RemoveLife(amount);
        }

        protected override void BurnTick()
        {
            this.RemoveLife(this.lifeLostWhileBurningPerTick);
        }

        #region Debug
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(this.transform.position, this.knockBackRadius);
        }
        #endregion
        #endregion
    }
}