using System;
using UnityEngine;
using DesignPattern.ObjectPooling;

namespace Game.Entities.Ennemies
{

    [AddComponentMenu("Game/Enemies/Big Zombie Pooler")]
    public class BigZombiePooler : AbstractPool<BigZombieEnemy>
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
        protected override BigZombieEnemy InstantiateEntity()
        {
            BigZombieEnemy zombieEnemy = base.InstantiateEntity();

            zombieEnemy.Initialize(this.player);

            return zombieEnemy;
        }
        #endregion
    }
}