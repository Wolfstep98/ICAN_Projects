using System;
using UnityEngine;
using UnityEngine.Events;
using DesignPattern.ObjectPooling;
using Game.Constants;
using Game.Entities.Player;

namespace Game.Entities.Projectiles
{
    /// <summary>
    /// Abstract class for all Projectile.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class AbstractProjectile : GameBehavior, IProjectile, IEntity
    {
        #region Class
        public class OnShootEvent : UnityEvent<Vector3>
        {
            public OnShootEvent() : base() { }
        }
        #endregion

        #region Fields
        [Header("References")]
        [SerializeField] protected new Rigidbody2D rigidbody = null;
        [SerializeField] protected PlayerController playerController = null;

        protected bool isEnable = false;
        protected int damage = 5;
        protected float speed = 5.0f;
        protected float maxDistance = 10.0f;
        protected Vector2 direction = Vector3.zero;

        private Vector2 startingPosition = Vector2.zero;

        [Header("Events")]
        public OnShootEvent onShootEvent = null;
        #endregion

        #region Init
        public virtual void Initialize()
        {
            this.direction = Vector2.zero;
            this.startingPosition = Vector2.zero;
#if UNITY_EDITOR
            if (this.rigidbody == null)
                Debug.LogError("[Missing Reference] - rigidbody ref is missing !");
#endif

            this.Disable();
        }
        public virtual void Initialize(Vector2 spawn, PlayerController playerController, float speed = -1)
        {
            if(speed >= 0)
                this.speed = speed;
            this.transform.position = spawn;
            this.startingPosition = spawn;
            this.playerController = playerController;
        }
        #endregion

        #region Properties
        public bool IsEnable { get { return this.isEnable; } }
        public float Speed { get { return this.speed; } }
        #endregion

        #region Methods
        public virtual void Shoot(Vector2 direction)
        {
            this.direction = direction;
            this.onShootEvent?.Invoke(direction);

            Debug.Log("Shoot in direction : " + direction.ToString() + " !");
        }

        public override void CustomFixedUpdate()
        {
            if(Vector2.Distance(this.startingPosition, this.rigidbody.position) >= this.maxDistance)
            {
                this.Disable();
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            this.Disable();
        }

        public virtual void Enable()
        {
            this.isEnable = true;
            this.direction = Vector2.zero;
            this.startingPosition = Vector2.zero;
            this.onShootEvent = new OnShootEvent();
            this.gameObject.SetActive(true);
        }

        public virtual void Disable()
        {
            this.isEnable = false;
            this.direction = Vector2.zero;
            this.startingPosition = Vector2.zero;
            this.onShootEvent = null; // Remove all events ? Allows to destroy only run-time ref.
            this.gameObject.SetActive(false);
        }
        #endregion
    }
}