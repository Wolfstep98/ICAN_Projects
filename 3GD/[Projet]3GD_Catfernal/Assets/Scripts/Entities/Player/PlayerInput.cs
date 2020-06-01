using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Constants;
using Game.Entities.FlameThrower;
using Game.Pause;

namespace Game.Entities.Player
{
    public class PlayerInput : GameBehavior
    {
        #region Fields
        [Header("References")]
        [SerializeField] private PlayerController playerController = null;
        [SerializeField] private FlameThrowerController flameThrower = null;
        #endregion

#if UNITY_EDITOR
        #region Check for null reference
        private void Awake()
        {
            if (this.playerController == null)
                Debug.LogError("[Null Reference] - playerController are not properly set !");
        }
        #endregion
#endif

        #region Methods

        public override void CustomUpdate()
        {
            if(PauseUtilities.gameIsPaused) return;
            
            this.playerController.SetDirection(new Vector2(Input.GetAxis(InputNames.Horizontal), Input.GetAxis(InputNames.Vertical)));
            this.playerController.SetRotation(Input.mousePosition);

            // --- FlameThrower
            if(Input.GetButtonDown(InputNames.FlameThrower))
            {
                this.flameThrower.ActivateFlameThrower();
            }
            else if(Input.GetButtonUp(InputNames.FlameThrower))
            {
                this.flameThrower.DesactivatedFlameThrower();
            }

            if(Input.GetButtonDown(InputNames.SwitchFlames))
            {
                this.flameThrower.SwitchMode();
            }

            if(Input.GetButtonDown(InputNames.Backfire))
            {
                this.flameThrower.Backfire(Input.mousePosition);
            }

            if (Input.GetButtonDown(InputNames.Cancel)){
                PauseUtilities.Instance.PauseGame();
            }

            if (this.playerController.IsDead){
                this.flameThrower.DesactivatedFlameThrower();
            }
        }
        #endregion
    }
}
