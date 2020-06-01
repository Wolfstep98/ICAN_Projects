using System;
using UnityEngine;

namespace Game.Behaviors
{
    /// <summary>
    /// Make a GameObject rotate over time.
    /// </summary>
    public class RotateOverTime : GameBehavior
    {
        #region Fields
        [Header("Data")]
        [SerializeField] private float rotationSpeed = 1.0f;
        [SerializeField] private Vector3 rotationAxis = Vector3.forward;
        #endregion

        #region Methods
        public override void CustomUpdate()
        {
            this.transform.Rotate(this.rotationAxis, this.rotationSpeed * Time.deltaTime, Space.World);
        }

        #region Editor
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(this.transform.position, this.transform.position + this.rotationAxis);
        }
        #endregion
        #endregion
    }
}