using System;
using UnityEngine;

namespace Game.Heat
{

    [Serializable]
    [RequireComponent(typeof(RectTransform))]
    public class GaugeArrow : GameBehavior
    {
        #region Fields
        [Header("References")]
        [SerializeField] private RectTransform rectTransform = null;

        [Header("Data")]
        [SerializeField] private float startRotation = 90.0f;
        [SerializeField] private float endRotation = -90.0f;
        [SerializeField] private Vector3 rotationAxis = Vector3.forward;
        #endregion

        #region Init

        #endregion

        #region Properties

        #endregion

        #region Methods
        public void UpdateArrowRotation(float heatPercent)
        {
            float rotation = Mathf.Lerp(this.startRotation, this.endRotation, heatPercent);
            this.rectTransform.rotation = Quaternion.AngleAxis(rotation, this.rotationAxis);
        }

        public void UpdateArrowPosition(float heatPercent)
        {
           
        }
        #endregion
    }
}