using System;
using UnityEngine;
using DesignPattern.ObjectPooling;

namespace Game.Entities.Ennemies
{

    [AddComponentMenu("Game/Enemies/Zombie Pooler")]
    public class ZombiePooler : AbstractPool<ZombieEnemy>
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
        protected override ZombieEnemy InstantiateEntity()
        {
            ZombieEnemy zombieEnemy = base.InstantiateEntity();

            zombieEnemy.Initialize(this.player);

            return zombieEnemy;
        }
        #endregion
    }
}