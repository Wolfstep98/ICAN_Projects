using Game.Constants;
using UnityEngine;

namespace Game.Behaviors
{
    /// <summary>
    /// Make an object explose on particle collision.
    /// </summary>
    public class ExploseOnHit : MonoBehaviour
    {
        #region Fields
        [SerializeField] protected ParticleSystem explosionPS;
        [SerializeField] protected new BoxCollider2D collider;
        [SerializeField] protected SpriteRenderer spriteRenderer;
        #endregion

        #region Methods
        private void OnParticleCollision(GameObject particleSystem)
        {
            if (particleSystem.tag.Contains(GameObjectTags.Flame))
            {
                this.Explose();
            }
        }

        /// <summary>
        /// Make the GameObject explode.
        /// </summary>
        protected virtual void Explose()
        {
            this.explosionPS.Play(true);
            this.spriteRenderer.enabled = false;
            this.collider.enabled = false;
        }
        #endregion
    }
}