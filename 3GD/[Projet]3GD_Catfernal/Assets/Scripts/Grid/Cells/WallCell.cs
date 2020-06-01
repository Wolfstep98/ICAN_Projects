using System;
using UnityEngine;

namespace Game.Grid
{

    [AddComponentMenu("Game/Grid/Cells/Wall Cell")]
    public class WallCell : IndestructibleCell
    {
        #region Fields

        #endregion

        #region Init
        public override void CustomAwake()
        {
            this.cellType = CellType.Wall;
        }
        #endregion

        #region Properties

        #endregion

        #region Methods
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(this.transform.position, Vector3.one);
        }
        #endregion
    }
}