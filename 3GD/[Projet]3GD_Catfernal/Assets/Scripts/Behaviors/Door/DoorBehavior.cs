using System;
using UnityEngine;

namespace Game.Behaviors
{

    [AddComponentMenu("Game/Behavior/Door")]
    public class DoorBehavior : MultipleActivableBehavior
    {
        #region Fields
        [Header("References")]
        [SerializeField] private Animator animator = null;

        private int openHashID = Animator.StringToHash("Open");
        #endregion

        #region Init
        public override void CustomAwake()
        {
            base.CustomAwake();
        }
        #endregion

        #region Properties

        #endregion

        #region Methods
        public override void Activate()
        {
            base.Activate();

            this.animator.SetBool(this.openHashID,true);
        }
        #endregion
    }
}