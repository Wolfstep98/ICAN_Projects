using System.Collections;
using System.Collections.Generic;
using Game.Behaviors;
using Game.Heat;
using Game.Constants;
using Game.Entities.Swarm;
using Game.Entities.Player;
using Game.Entities.Ennemies;
using UnityEngine;

namespace Game.Cheat 
{
    public class CheatManager : GameBehavior 
    {
        #region Fields

		[Header("References")] 
		[SerializeField] private new Camera camera;
		[SerializeField] private Animator animator;
		[SerializeField] private PlayerController playerController;
        [SerializeField] private HeatGauge heatGauge;
		[SerializeField] private Transform[] tpPoints;
        [SerializeField] private ZombiePooler zombiePooler = null;
        [SerializeField] private BigZombiePooler bigZombiePooler = null;
        [SerializeField] private ExplosiveZombiePooler explosiveZombiePooler = null;
        [SerializeField] private HeartSwarmPooler heartSwarmPooler = null;
		[SerializeField] private GameObject barrelPrefab;
		[SerializeField] private GameObject rocketPrefab;
		[SerializeField] private DoorBehavior door;

		private bool playerIsInvulnerable = false;
		private int tpIndex = 0;
		private static readonly int FadeTp = Animator.StringToHash("FadeTP");

		#endregion

#if UNITY_EDITOR
        #region Check for null reference
        private void Awake() {
			if (this.camera == null)
				Debug.Log("[Null Reference] - camera are not properly set !");
            if (this.playerController == null)
                Debug.LogError("[Null Reference] - playerController are not properly set !");
            /*if (this.tpPoints == null){
				Debug.LogError("[Null Reference] - tpPoints are not properly set !");
			}
			else{
				for (var i = 0; i < tpPoints.Length; i++){
					if (this.tpPoints[i] == null)
						Debug.LogError("[Null Reference] - tpPoints " + i +" are not properly set !");
				}
			}*/
            if (this.zombiePooler == null)
                Debug.LogError("[Null Reference] - zombiePooler are not properly set !");
            if (this.bigZombiePooler == null)
                Debug.LogError("[Null Reference] - bigZombiePooler are not properly set !");
            if (this.explosiveZombiePooler == null)
                Debug.LogError("[Null Reference] - explosiveZombiePooler are not properly set !");
            if (this.heartSwarmPooler == null)
				Debug.LogError("[Null Reference] - heartSwarmPooler are not properly set !");
			if (this.barrelPrefab == null)
				Debug.LogError("[Null Reference] - barrelPrefab are not properly set !");
			if (this.rocketPrefab == null)
				Debug.LogError("[Null Reference] - rocketPrefab are not properly set !");
        }
        #endregion
#endif

        #region Methods
        
		public override void CustomAwake()
		{
			
		}
		
		public override void CustomUpdate()
        {
			if (Input.GetButtonDown(InputNames.TP1)){
				animator.SetTrigger(FadeTp);
				tpIndex = 0;
			}
			
			if (Input.GetButtonDown(InputNames.TP2)){
				animator.SetTrigger(FadeTp);
				tpIndex = 1;
			}
			
			if (Input.GetButtonDown(InputNames.TP3)){
				animator.SetTrigger(FadeTp);
				tpIndex = 2;
			}
			
			if (Input.GetButtonDown(InputNames.TP4)){
				animator.SetTrigger(FadeTp);
				tpIndex = 3;
			}
			
			if (Input.GetButtonDown(InputNames.TP5)){
				animator.SetTrigger(FadeTp);
				tpIndex = 4;
			}
			
			if (Input.GetButtonDown(InputNames.TP6)){
				animator.SetTrigger(FadeTp);
				tpIndex = 5;
			}
			
			if (Input.GetButtonDown(InputNames.Immortality)){
				playerIsInvulnerable = !playerIsInvulnerable;
				playerController.SetInvulnerability(playerIsInvulnerable);
			}
			
			if (Input.GetButtonDown(InputNames.UnlimiteadSafe))
            {
                this.heatGauge.LockColdState();
			}
			
			if (Input.GetButtonDown(InputNames.UnlimiteadHot)){

                this.heatGauge.LockHotState();
            }
			
			if (Input.GetButtonDown(InputNames.UnlimiteadInferno)){

                this.heatGauge.LockInfernoState();
            }

            if (Input.GetButtonDown(InputNames.UnlockGauge))
            {
                this.heatGauge.UnlockStates();
            }

            if (Input.GetButtonDown(InputNames.SpawnZombie))
            {
                ZombieEnemy enemy = this.zombiePooler.GetEntity();
                Vector3 spawnPos = camera.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 10.0f;
                enemy.transform.position = spawnPos;
            }
            
            if (Input.GetButtonDown(InputNames.SpawnBigZombie))
            {
                BigZombieEnemy enemy = this.bigZombiePooler.GetEntity();
                Vector3 spawnPos = camera.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 10.0f;
                enemy.transform.position = spawnPos;
            }
            
            if (Input.GetButtonDown(InputNames.SpawnExplosiveZombie))
            {
                ExplosiveZombieEnemy enemy = this.explosiveZombiePooler.GetEntity();
                Vector3 spawnPos = camera.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 10.0f;
                enemy.transform.position = spawnPos;
            }

            if (Input.GetButtonDown(InputNames.SpawnSwarnHeart))
            {
                HearthSwarmBehavior hearthSwarmBehavior = this.heartSwarmPooler.GetEntity();
                Vector3 spawnPos = camera.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 10.0f;
                spawnPos.x = Mathf.FloorToInt(spawnPos.x);
                spawnPos.y = Mathf.FloorToInt(spawnPos.y);
                hearthSwarmBehavior.transform.position = spawnPos;
                hearthSwarmBehavior.SetupHeartSwarm();
            }

            if (Input.GetButtonDown(InputNames.SpawnBarrel)){
				Instantiate(barrelPrefab, camera.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 10, Quaternion.identity);
			}
			
			if (Input.GetButtonDown(InputNames.SpawnRocket)){
				Instantiate(rocketPrefab, camera.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 10, Quaternion.identity);
			}

			if (Input.GetButtonDown(InputNames.Door)){
				this.door.Activate();
			}
        }

		public void Teleport() {
			this.playerController.Teleport(tpPoints[tpIndex].position);
		}
		
        #endregion
    }
}
