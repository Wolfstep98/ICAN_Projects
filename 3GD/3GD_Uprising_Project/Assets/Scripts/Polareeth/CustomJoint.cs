using System;
using UnityEngine;
using TMPro;

namespace Polareeth
{
    [RequireComponent(typeof(SpringJoint))]
    public class CustomJoint : MonoBehaviour
    {
        #region Fields
        [Header("Parameters")]
        [SerializeField]
        private float forceHorizontal = 0.0f;
        [SerializeField]
        private float forceHorizontalColor = 0.0f;
        [SerializeField]
        private float highForceMax = 8.0f;
        [Header("Data")]
        [SerializeField]
        private Color normalColor = Color.blue;
        [SerializeField]
        private Color highForceColor = Color.red;
        [Header("References")]
        [SerializeField]
        private MeshRenderer meshRenderer = null;
        [SerializeField]
        private SpringJoint springJoint = null;
        [SerializeField]
        private TextMeshProUGUI textForce = null;
        [SerializeField]
        new private ConstantForce constantForce = null;
        #endregion

        #region Properties
        public float ForceHorizontal { get { return this.forceHorizontal; } }
        #endregion

        #region Methods
        private void FixedUpdate()
        {
            this.UpdateMeshColor();
            this.textForce.text = this.springJoint.currentForce.ToString();
        }

        private void UpdateMeshColor()
        {
            this.forceHorizontal = Math.Abs(this.springJoint.currentForce.x) + Math.Abs(this.springJoint.currentForce.z);
            this.forceHorizontalColor = Mathf.Abs(forceHorizontal / highForceMax);
            if (this.forceHorizontalColor > 1.0f)
                this.forceHorizontalColor = 1.0f;
            else if (this.forceHorizontalColor < 0.0f)
                this.forceHorizontalColor = 0.0f;
            Color color = Color.Lerp(this.normalColor, this.highForceColor, this.forceHorizontalColor);
            this.meshRenderer.material.color = color;
        }

        public void UpdateGravity(float gravity)
        {
            this.constantForce.Force = gravity * -0.7f;
        }
        #endregion

        #region Editor
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(this.transform.position, this.springJoint.currentForce);
        }

        private void OnValidate()
        {
            if (this.meshRenderer == null)
                this.meshRenderer = this.GetComponent<MeshRenderer>();
            if (this.springJoint == null)
                this.springJoint = this.GetComponent<SpringJoint>();
            if (this.constantForce == null)
                this.constantForce = this.GetComponent<ConstantForce>();
            if (this.textForce == null)
            {
                Transform canvas = this.transform.GetChild(0);
                this.textForce = canvas.GetChild(0).GetComponent<TextMeshProUGUI>();
            }
        }
        #endregion
    }
}
