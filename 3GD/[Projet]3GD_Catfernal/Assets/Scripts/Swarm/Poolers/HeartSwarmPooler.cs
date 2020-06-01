using System;
using UnityEngine;
using Game.Grid;
using DesignPattern.ObjectPooling;

namespace Game.Entities.Swarm
{
    [AddComponentMenu("Game/Swarm/Heart Swarm Pooler")]
    public class HeartSwarmPooler : AbstractPool<HearthSwarmBehavior>
    {
        #region Fields
        [Header("References")]
        [SerializeField] private Grid2D grid = null;
        [SerializeField] private CellsPooler cellsPooler = null;
        #endregion

        #region Init

        #endregion

        #region Properties

        #endregion

        #region Methods
        protected override HearthSwarmBehavior InstantiateEntity()
        {
            HearthSwarmBehavior hearthSwarmBehavior = base.InstantiateEntity();

            hearthSwarmBehavior.Initialize(this.grid, this.cellsPooler);

            return hearthSwarmBehavior;
        }
        #endregion
    }
}