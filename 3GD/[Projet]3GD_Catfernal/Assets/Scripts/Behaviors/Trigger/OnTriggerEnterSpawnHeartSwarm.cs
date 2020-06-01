using System;
using UnityEngine;
using Game.Entities.Swarm;

namespace Game.Behaviors
{

    [AddComponentMenu("Game/Behavior/On Trigger Enter Spawn Heart Swarm")]
    public class OnTriggerEnterSpawnHeartSwarm : OnTriggerEnterWithPlayerEvent
    {
        #region Fields
        private bool spawned = false;

        [Header("References")]
        [SerializeField] private Transform[] spawns = null;
        [SerializeField] private HeartSwarmPooler heartSwarmPooler = null;
        #endregion

        #region Methods
        private void SpawHeartSwarms()
        {
            for(int i = 0; i < this.spawns.Length; i++)
            {
                HearthSwarmBehavior hearthSwarmBehavior = this.heartSwarmPooler.GetEntity();
                var position = new Vector3(Mathf.Round(this.spawns[i].position.x), Mathf.Round(this.spawns[i].position.y), Mathf.Round(this.spawns[i].position.z));
                hearthSwarmBehavior.transform.position = position;
                hearthSwarmBehavior.SetupHeartSwarm();
            }

            this.spawned = true;
        }

        protected override void OnTriggerEnter()
        {
            if (!this.spawned)
            {
                this.SpawHeartSwarms();

                base.OnTriggerEnter();
            }
        }
        #endregion
    }
}