using System;
using UnityEngine;

namespace Game.Grid
{
    public abstract class Cell : GameBehavior
    {
        #region Fields
        [SerializeField] protected CellType cellType = CellType.None;
        #endregion

        #region Init

        #endregion

        #region Properties
        public Vector2 Position { get { return this.transform.position; } }
        public CellType CellType { get { return this.cellType; } }
        #endregion

        #region Methods

        #endregion
    }
}