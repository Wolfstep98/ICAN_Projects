using System;
using UnityEngine;

namespace Game.Entities.Projectiles
{
    public interface IProjectile
    {
        #region Properties
        float Speed { get; }
        #endregion

        #region Methods
        void Shoot(Vector2 direction);
        #endregion
    }
}