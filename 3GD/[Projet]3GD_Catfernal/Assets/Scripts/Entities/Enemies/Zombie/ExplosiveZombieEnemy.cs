using UnityEngine;
using Game.Enums;
using FMODUnity;
using Game.Constants;

namespace Game.Entities.Ennemies
{
    [AddComponentMenu("Game/Enemies/Explosive Zombie")]
    public class ExplosiveZombieEnemy : ZombieEnemy
    {
        #region Fields
        [Header("Data")]
        [SerializeField] private float explosionRadius = 2.5f;
        [SerializeField] private LayerMask explosionLayerMask;
        [SerializeField] private int maxExplosionColliders = 10;

        [Header("Sounds")]
        [SerializeField] [EventRef] private string explosionSound = null;

        [Header("References")]
        [SerializeField] protected ParticleSystem explosionParticleSystem = null;
        [SerializeField] private GameObject explosionFX = null;
        #endregion

        #region Init

        #endregion

        #region Properties

        #endregion

        #region Methods

        public override void TakeDamage(int amount, DamageSource source)
        {
            base.TakeDamage(amount, source);
        }

        protected void Explode()
        {
            Debug.Log("[Explosive Zombie] - Explosion !");

            ContactFilter2D filter2D = new ContactFilter2D();
            filter2D.SetLayerMask(this.explosionLayerMask);
            Collider2D[] colliders = new Collider2D[this.maxExplosionColliders];
            this.explosionFX.SetActive(true);
            int collidersOverlap = Physics2D.OverlapCircle(this.transform.position, this.explosionRadius, filter2D, colliders);
            if (collidersOverlap > 0)
            {
                for (int i = 0; i < collidersOverlap; i++)
                {
                    if (colliders[i].gameObject == this.gameObject) continue;

                    Player.PlayerController player = colliders[i].GetComponent<Player.PlayerController>();
                    if (player != null)
                    {
                        player.ReduceHealth(1);
                        continue;
                    }

                    AbstractEnemy enemy = colliders[i].GetComponent<AbstractEnemy>();
                    if (enemy != null)
                    {
                        Debug.Log("[Explosive Zombie] - " + enemy.name + " hit !");
                        enemy.TakeDamage(100, DamageSource.Explosion);
                        continue;
                    }
                }
            }

            this.explosionParticleSystem.Play();
            RuntimeManager.PlayOneShot(this.explosionSound, this.transform.position);

            this.Disable(1.0f);
        }

        protected override void Burn()
        {
            this.Explode();
        }

        #region Serialization

        #endregion

        #region Collisions
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(collision.gameObject.CompareTag("Player"))
            {
                this.Explode();
            }
        }

        protected override void OnParticleCollision(GameObject particleSystem)
        {
            base.OnParticleCollision(particleSystem);

            if (particleSystem.tag.Contains(GameObjectTags.Flame))
            {
                this.Explode();
            }
        }
        #endregion

        #region Debug
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(this.transform.position, this.explosionRadius);
        }
        #endregion
        #endregion
    }
}
