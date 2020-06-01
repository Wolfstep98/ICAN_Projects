using System;
using Game.Enums;

namespace Game.Entities.Ennemies
{
    /// <summary>
    /// Interface for all enemy entity.
    /// </summary>
    public interface IEnemy
    {
        #region Properties
        int Life { get; }
        #endregion

        #region Methods
        void TakeDamage(int amount, DamageSource source);
        #endregion
    }
}
