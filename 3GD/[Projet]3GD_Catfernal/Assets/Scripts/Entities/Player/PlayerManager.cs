using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Entities.Player
{
    public class PlayerManager : GameBehavior
    {
        #region Fields & Properties
        [Header("References")]
        [SerializeField] private PlayerInput input = null;
        [SerializeField] private PlayerController controller = null;
        #endregion

#if UNITY_EDITOR
        #region Check for null reference
        private void Awake()
        {
            if (this.input == null)
                Debug.LogError("[Null Reference] - input are not properly set !");
            if (this.controller == null)
                Debug.LogError("[Null Reference] - controller are not properly set !");
        }
        #endregion
#endif

        #region Methods
        public override void CustomAwake()
        {
            //Input
            this.controller.CustomAwake();
        }

        public override void CustomUpdate()
        {
            //Input 
            this.input.CustomUpdate();

            //Controller
            this.controller.CustomUpdate();
        }

        public override void CustomFixedUpdate()
        {
            //Controller
            this.controller.CustomFixedUpdate();
        }
        #endregion
    }
}
