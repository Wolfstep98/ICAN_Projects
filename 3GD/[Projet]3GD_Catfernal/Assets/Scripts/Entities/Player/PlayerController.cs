using System;
using System.Collections.Generic;
using Game.Constants;
using Game.Entities.Player.Health;
using Game.Level;
using Game.Pause;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Game.Entities.Player
{
    public class PlayerController : GameBehavior
    {
        #region Fields

        [Header("References")]
        [SerializeField] private new Rigidbody2D rigidbody2D = null;
        [SerializeField] private new Camera camera = null;
        [SerializeField] private AvatarAnimationManager animManager;
        [SerializeField] private HealthDisplay healthDisplay;

        [Header("Parameters")] 
        [SerializeField] private int maxHealth = 3;
        [SerializeField] private float iFrameAfterHitDuration = 0.5f;
        private float iFrameAfterHitTime = 0;
        private bool isInvulnerable = false;
        private bool isHit = false; 
        private int health;        
        
        public bool IsDead { get; private set; }
        
        [SerializeField] private float moveSpeed = 5.0f;

        private Vector2 direction = Vector2.zero;
        private Vector2 velocity = Vector2.zero;
        private Vector2 backfireForce = Vector2.zero;

        private bool backfireForceActive = false;
        [SerializeField] private float backfireKnockbackTime = 1.0f;
        private float backfireKnockbackXValue = 0.0f;
        private float backfireKnockbackYValue = 0.0f;

        //[SerializeField] private float backfireForceReducing = 5.0f;
        //[SerializeField] private float backfireResetThreshold = 0.01f;
        #endregion

#if UNITY_EDITOR
        #region Check for null reference
        private void Awake()
        {
            if (this.rigidbody2D == null)
                Debug.LogError("[Null Reference] - rigidbody2D are not properly set !");
        }
        #endregion
#endif

        #region Methods
        public override void CustomAwake() 
        {
            this.health = maxHealth;
            this.IsDead = false;
            this.direction = Vector2.zero;
            this.velocity = Vector2.zero;
        }

        public override void CustomUpdate() {
            if (this.backfireForceActive)
            {
                // Reduce backfire force.
                //this.backfireForce.x += ((this.backfireForce.x > 0) ?
                //                                                    -(backfireForceReducing + ((this.direction.x < 0) ? -this.direction.x : 0))
                //                                                    : (backfireForceReducing + ((this.direction.x > 0) ? this.direction.x : 0)))
                //                                                        * Time.deltaTime;
                //this.backfireForce.y += ((this.backfireForce.y > 0) ?
                //                                                    -(backfireForceReducing + ((this.direction.y < 0) ? -this.direction.y : 0))
                //                                                    : (backfireForceReducing + ((this.direction.y > 0) ? this.direction.y : 0)))
                //                                                    * Time.deltaTime;

                this.backfireForce.x -= this.backfireKnockbackXValue * Time.deltaTime;
                this.backfireForce.y -= this.backfireKnockbackYValue * Time.deltaTime;

                //if (Math.Abs(this.backfireForce.x) <= this.backfireResetThreshold)
                if((this.backfireKnockbackXValue > 0.0f)? this.backfireForce.x < 0.0f : this.backfireForce.x > 0.0f)
                {
                    this.backfireForce.x = 0.0f;
                }
                //if (Math.Abs(this.backfireForce.y) <= this.backfireResetThreshold)
                if ((this.backfireKnockbackYValue > 0.0f) ? this.backfireForce.y < 0.0f : this.backfireForce.y > 0.0f)
                {
                    this.backfireForce.y = 0.0f;
                }

                if (this.backfireForce == Vector2.zero)
                    this.backfireForceActive = false;

                this.velocity = this.backfireForce;
            }
            else
            {
                this.velocity = this.direction * this.moveSpeed + this.backfireForce;
            }

            if (isHit){
                iFrameAfterHitTime -= GameTime.deltaTime;
                animManager.SetHit(true);
                isInvulnerable = true;
            }

            if (iFrameAfterHitTime <= 0){
                isHit = false;
                animManager.SetHit(false);
                isInvulnerable = false;
                iFrameAfterHitTime = iFrameAfterHitDuration;
            }
        }

        public override void CustomFixedUpdate()
        {
            if (PauseUtilities.gameIsPaused){
                this.rigidbody2D.velocity = Vector2.zero;
                this.animManager.SetMoving(false);
            }
            else{
                //Move the rigidbody by adding force
                //this.rigidbody2D.AddForce(this.velocity, ForceMode2D.Force); //Bad one, no controls over max speed

                //Move the rigidbody by controlling the velocity
                this.rigidbody2D.velocity = this.velocity;
                this.animManager.SetMoving(this.velocity != Vector2.zero);       
            }
        }

        public void SetDirection(Vector2 direction)
        {
            this.direction = direction.normalized;
        }

        public Vector2 GetDirection()
        {
            return this.direction;
        }

        public void AddBackfireForce(Vector2 force)
        {
            this.backfireForceActive = true;
            this.backfireForce = force;
            this.backfireKnockbackXValue = this.backfireForce.x / this.backfireKnockbackTime;
            this.backfireKnockbackYValue = this.backfireForce.y / this.backfireKnockbackTime;
        }

        public void SetRotation(Vector2 mousePos)
        {
            if (!this.backfireForceActive)
            {
                Vector3 mouseWorldPos = this.camera.ScreenToWorldPoint(mousePos);
                mouseWorldPos.z = this.transform.position.z;
                this.transform.right = mouseWorldPos - this.transform.position;
            }
        }

        public void ReduceHealth(int value) 
        {
            if(isInvulnerable) return;

            isHit = true;
            
            health -= value;
            
            healthDisplay.UpdateDisplay(health);
            
            if (health <= 0){
                this.IsDead = true;
                LevelUtilities.Instance.LoadLevel(SceneManager.GetActiveScene().buildIndex);
            }
        }

        public void RestoreHealth(int value) 
        {
            health += value;
            
            if (health > maxHealth)
                health = maxHealth;
            
            healthDisplay.UpdateDisplay(health);
        }

        public void SetInvulnerability(bool value) {
            isInvulnerable = value;
        }
        
        public void Teleport(Vector2 position) 
        {
            this.rigidbody2D.position = position;
        }
        #endregion

        #region Collision

        private void OnCollisionStay(Collision other) {
            var collision = other.collider;
            if (collision.CompareTag(GameObjectTags.Cell) || collision.CompareTag(GameObjectTags.Hearth))
            {
                ReduceHealth(1);
            }
        }

        private void OnParticleCollision(GameObject particleSystem) {
            if (particleSystem.CompareTag(GameObjectTags.Flame))
            {
                ReduceHealth(1);
            }
        }

        #endregion
    }
}
