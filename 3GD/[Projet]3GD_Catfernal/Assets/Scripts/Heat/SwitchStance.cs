using System;
using UnityEngine;

namespace Game.Heat
{
    public class SwitchStance : GameBehavior
    {
        #region Fields
        [Header("References")]
        [SerializeField] private Animator animator;

        private int sprayID;
        #endregion

        #region Init
        public override void CustomAwake()
        {
            this.sprayID = Animator.StringToHash("spray");
        }
        #endregion

        #region Methods
        
        public void SwitchToSpray()
        {
            //this.animator.SetBool(this.sprayID, true);
        }

        public void SwitchToLazer()
        {
            // this.animator.SetBool(this.sprayID, false);
        }
        
        #endregion
    }
}