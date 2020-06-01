using System;
using UnityEngine;

namespace Game.Grid
{
    
    public abstract class MultiCell : Cell
    {
        #region Fields
        [SerializeField] protected int width = 1;
        [SerializeField] protected int height = 1;
        protected Vector2[] positions;
        #endregion

        #region Init
        public override void CustomAwake()
        {
            this.positions = new Vector2[this.width * this.height];
            Vector2 bottomLeftCellPosition = (Vector2)this.transform.position - new Vector2(width / 2.0f, height / 2.0f) + (Vector2.one * 0.5f);
            int index = 0;
            for(int x = 0; x < this.width; x++)
            {
                for (int y = 0; y < this.height; y++)
                {
                    this.positions[index] = bottomLeftCellPosition + new Vector2(x, y);
                    index++;
                }
            }
        }
        #endregion

        #region Properties

        #endregion

        #region Methods

        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireCube(this.transform.position, new Vector3(this.width, this.height));
            if (this.positions != null)
            {
                for (int i = 0; i < this.positions.Length; i++)
                {
                    Gizmos.DrawWireSphere(this.positions[i], 0.5f);
                }
            }

        }
        #endregion
    }
}