using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
using Game.Pause;

namespace Game.Entities.Swarm
{
    public class SwarmBehavior : GameBehavior
    {
        #region Properties
        [Header("Parameters")]
        [SerializeField] private bool isTicking = true;
        [SerializeField] private float tickDuration = 1;

        private float time = 0;

        public static float SwarmTime;

        public delegate void SwarmTick();
        public static event SwarmTick OnTick;
        #endregion

        #region Methods
        public override void CustomUpdate() 
        {
            if (PauseUtilities.gameIsPaused) return;
            
            time += GameTime.deltaTime;
            if (time >= tickDuration)
            {
                time -= tickDuration;
                if(this.isTicking) OnTick?.Invoke();
                //System.Delegate[] delegates = OnTick.GetInvocationList();
                //Debug.Log("[Swarm Behavior] - Currenctly " + delegates?.Length + " swarm objects are ticking.");
                //if(delegates != null)
                //{
                //    for(int i = 0; i < delegates.Length; i++)
                //    {
                //        Debug.Log("Sarm Tick : " + delegates[i].Target.ToString());
                //    }
                //}
                
            }
            SwarmTime = time / tickDuration;
        }

        private void OnDisable()
        {
            OnTick = null;
        }
        #endregion
    }
}