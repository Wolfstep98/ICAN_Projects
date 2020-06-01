using System;
using UnityEngine;
using DesignPattern.ObjectPooling;

namespace Game.Entities.Ennemies
{

    [AddComponentMenu("Game/Enemies/Explosive Zombie Pooler")]
    public class ExplosiveZombiePooler : AbstractPool<ExplosiveZombieEnemy>
    {
        #region Fields
        [Header("References")]
        [SerializeField] private GameObject player = null;
        #endregion

        #region Init

        #endregion

        #region Properties

        #endregion

        #region Methods
        protected override ExplosiveZombieEnemy InstantiateEntity()
        {
            ExplosiveZombieEnemy zombieEnemy = base.InstantiateEntity();

            zombieEnemy.Initialize(this.player);

            return zombieEnemy;
        }
        #endregion
    }
}