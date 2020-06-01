using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Entities.Projectiles
{
    public class Bullet : AbstractProjectile
    {
        #region Methods
        public override void Shoot(Vector2 direction)
        {
            base.Shoot(direction);

            this.rigidbody.velocity = direction.normalized * this.speed;
        }

        public override void Disable()
        {
            this.rigidbody.velocity = Vector2.zero;

            base.Disable();
        }
        #endregion
    }
}
