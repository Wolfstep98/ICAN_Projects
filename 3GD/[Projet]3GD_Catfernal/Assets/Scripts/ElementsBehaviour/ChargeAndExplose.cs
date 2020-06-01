using Game.Constants;
using UnityEngine;

namespace Game.Behaviors
{
    /// <summary>
    /// Make a GameObject Charge on Particle Collision and Explode on collision with the world.
    /// </summary>
    public class ChargeAndExplose : MonoBehaviour
    {
        [SerializeField] ParticleSystem explosion;
        [SerializeField] new BoxCollider2D collider;
        [SerializeField] SpriteRenderer sprite;
        [SerializeField] Rigidbody2D rigid;
        //[SerializeField] Vector2 direction;
        [SerializeField] float chargeSpeed;
        private bool lit;

        private void OnParticleCollision(GameObject particleSystem)
        {
            if (particleSystem.tag.Contains(GameObjectTags.Flame) && !lit)
            {
                Charge();
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Explose();
        }

        private void Charge()
        {
            this.lit = true;
            //this.rigid.AddForce(this.direction * this.chargeSpeed);
            this.rigid.velocity = transform.right * chargeSpeed;
        }

        private void Explose()
        {
            this.explosion.Play(true);
            this.sprite.enabled = false;
            this.collider.enabled = false;
            this.rigid.bodyType = RigidbodyType2D.Static;
        }
    }
}