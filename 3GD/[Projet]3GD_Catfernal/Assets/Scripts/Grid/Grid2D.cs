using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Grid
{
    [AddComponentMenu("Game/Grid/Grid 2D")]
    public class Grid2D : GameBehavior
    {
        #region Fields
        [Header("References")]
        [SerializeField] private GameManager gameManager = null;
        [SerializeField] private WallCell[] walls = null;

        private Dictionary<Vector2, Cell> cells = new Dictionary<Vector2, Cell>();
        #endregion

        #region Init

        #endregion

        #region Accessors
        public Cell this[Vector2 position]
        {
            get
            {
                if(this.cells.ContainsKey(position))
                {
                    return this.cells[position];
                }
                return null;
            }
        }
        #endregion

        #region Properties
        public Dictionary<Vector2, Cell> Cells { get { return this.cells; } }
        #endregion

        #region Methods
        public override void CustomAwake()
        {
            this.Initialize();
        }

        private void Initialize()
        {
            for(int i = 0; i < this.walls.Length; i++)
            {
                this.cells.Add(this.walls[i].Position, this.walls[i]);
            }

            Dictionary<Vector2, Cell>.KeyCollection keys = this.cells.Keys;
            foreach(Vector2 position in keys)
            {
                this.gameManager.DynamicGameBehavior.Add(this.cells[position]);
            }
        }

        #endregion
    }
}