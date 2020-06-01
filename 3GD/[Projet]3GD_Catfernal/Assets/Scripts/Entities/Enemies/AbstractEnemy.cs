using System;
using System.Xml;
using UnityEngine;
using UnityEngine.Events;
using DesignPattern.ObjectPooling;
using Game.Enums;
using Game.Constants;
using Game.Serialization;
using Game.Behaviors;
using Game.Entities.Player;
using NodeCanvas.StateMachines;
using NodeCanvas.Framework;
using Pathfinding;
using FMOD.Studio;
using FMODUnity;

namespace Game.Entities.Ennemies
{
    /// <summary>
    /// Abstract class for all enemy.
    /// </summary>
    public abstract class AbstractEnemy : Flammable, IEnemy, IEntity, ISerializable
    {
        #region Class
        #region Event Class
        /// <summary>
        /// Event that occurs when the ennemy takes damage.
        /// First parameter : Damage amount.
        /// Second parameter : Life left.
        /// </summary>
        [Serializable]
        public class OnDamageDealt : UnityEvent<int, int>
        {
            public OnDamageDealt() : base() { }
        }

        /// <summary>
        /// Event that occrus when the enemy die.
        /// First parameter : Damage source.
        /// </summary>
        [Serializable]
        public class OnDeathEvent : UnityEvent<DamageSource>
        {
            public OnDeathEvent() : base() { }
        }
        #endregion
        #endregion

        #region Fields
        #region Constants
        protected const int DefaultLife = 1;

        protected const string XmlRoot = "Ennemy";
        protected const string XmlEnemyType = "EnnemyType";
        protected const string XmlLife = "Life";
        protected const string XmlDamageDealtOnPlayerHit = "DamageDealtOnPlayerHit";
        #endregion

        [Header("References")]
        [SerializeField] protected FSMOwner fSMOwner = null;
        [SerializeField] protected Blackboard blackboard = null;
        [SerializeField] protected AILerp lerp = null;
        [SerializeField] protected AIDestinationSetter destinationSetter = null;

        [Header("Data")]
        [SerializeField] protected bool isAlreadyInstantiated = false;
        protected bool isEnable = false;
        protected bool isDestroying = false;
        protected float destroyDelay = 0.0f;
        protected float destroyTimer = 0.0f;
        [SerializeField] protected EnemyType enemyType = EnemyType.None;
        [SerializeField] protected int m_life = 0;
        [SerializeField] protected int damageDealtOnPlayerHit = 1;
        [SerializeField] protected float knockbackForceOnHit = 5.0f;

        [SerializeField] protected int damagePerTick = 5;

        [Header("Xml Save File")]
        [SerializeField] protected string XmlSaveFileName = "AbstractEnemy";

        [Header("Moving")]
        [SerializeField] protected float timeBetweenStep = 0.2f;
        protected float stepTime = 0.0f;

        [Header("Growl")]
        [SerializeField] protected Vector2 timesBetweenGrowls = Vector2.one;
        protected float timeBetweenGrowl = 0.0f;
        protected float growlTimer = 0.0f;

        [SerializeField] protected Vector2 timesBetweenBurningGrowls = Vector2.one;
        protected float timeBetweenBurningGrowl = 0.0f;

        [Header("Sounds")]
        [SerializeField] [EventRef] protected string walkSound;
        [SerializeField] [EventRef] protected string growlSound;
        [SerializeField] [EventRef] protected string growlIsBurningSound;
        [SerializeField] [EventRef] protected string attackSound;

        [Space(5)]

        [Header("Events")]
        public OnDamageDealt onDamageDealt = null;
        public OnDeathEvent onDeath = null;
        #endregion

        #region Init
        public override void CustomAwake()
        {
            if(this.isAlreadyInstantiated)
            {
                this.Initialize();
                this.Enable();
            }
        }

        public virtual void Initialize()
        {
            if(!this.isAlreadyInstantiated)
                this.Disable();
            //this.LoadXmlData();
        }
        #endregion

        #region Properties
        public bool IsEnable { get { return this.isEnable; } }
        public int Life { get { return this.m_life; } }
        public int DamageDealtOnPlayerHit { get { return this.damageDealtOnPlayerHit; } }
        #endregion

        #region Methods
        public override void CustomUpdate()
        {
            if (!this.isEnable) return;

            base.CustomUpdate();

            if(this.isDestroying)
            {
                this.destroyTimer += GameTime.deltaTime;

                if(this.destroyTimer >= this.destroyDelay)
                {
                    this.Disable();
                }
                return;
            }

            //if (this.destinationSetter.target != null || this.destinationSetter.target != this.transform)
            //{
            //    this.stepTime += GameTime.deltaTime;

            //    if(this.stepTime >= this.timeBetweenStep)
            //    {
            //        this.stepTime -= this.timeBetweenStep;
            //        this.MakeStep();
            //    }
            //}

            this.growlTimer += GameTime.deltaTime;
            if (!this.isBurning)
            {
                if(this.growlTimer >= this.timeBetweenGrowl)
                {
                    this.growlTimer -= this.timeBetweenGrowl;
                    this.Growl();
                    this.timeBetweenGrowl = UnityEngine.Random.Range(this.timesBetweenGrowls.x, this.timesBetweenGrowls.y);
                }
            }
            else
            {
                if (this.growlTimer >= this.timeBetweenBurningGrowl)
                {
                    this.growlTimer -= this.timeBetweenBurningGrowl;
                    this.BurningGrowl();
                    this.timeBetweenBurningGrowl = UnityEngine.Random.Range(this.timesBetweenBurningGrowls.x, this.timesBetweenBurningGrowls.y);
                }
            }
        }

        protected void Growl()
        {
            RuntimeManager.PlayOneShot(this.growlSound, this.transform.position);
        }

        protected void BurningGrowl()
        {
            RuntimeManager.PlayOneShot(this.growlIsBurningSound, this.transform.position);
        }

        public virtual void TakeDamage(int amount, DamageSource source)
        {
            if (this.m_life <= 0)
                return;

            int lifeLeft = this.m_life - amount;

            if (lifeLeft >= 0)
            {
                this.m_life = lifeLeft;
            }

            if (lifeLeft < 0)
            {
                this.m_life = 0;
            }

            if (this.m_life == 0)
                this.Death();

            if (this.onDamageDealt != null)
                this.onDamageDealt.Invoke(amount, this.m_life);
        }

        protected virtual void Death(DamageSource damageSource = DamageSource.Undefined)
        {
            this.onDeath?.Invoke(damageSource);

            this.gameObject.SetActive(false);
            this.Disable();
        }

        public virtual void Enable()
        {
            this.isEnable = true;
            this.isDestroying = false;
            if(this.m_life == 0)
                this.m_life = DefaultLife;
            //this.onDeath = new OnDeathEvent();
            if (!this.isBurning)
            {
                this.timeBetweenGrowl = UnityEngine.Random.Range(this.timesBetweenGrowls.x, this.timesBetweenGrowls.y);
            }
            else
            {
                this.timeBetweenBurningGrowl = UnityEngine.Random.Range(this.timesBetweenBurningGrowls.x, this.timesBetweenBurningGrowls.y);
            }
            this.gameObject.SetActive(true);
        }

        public virtual void Disable()
        {
            this.isEnable = false;
            this.onDeath = null;
            this.gameObject.SetActive(false);
        }

        public virtual void Disable(float delay)
        {
            this.isDestroying = true;
            this.destroyDelay = delay;
            this.destroyTimer = 0.0f;
        }

        protected override void Burn()
        {
            base.Burn();

            this.growlTimer = 0.0f;
            this.timeBetweenBurningGrowl = UnityEngine.Random.Range(this.timesBetweenBurningGrowls.x, this.timesBetweenBurningGrowls.y);
        }

        protected override void BurnTick()
        {
            this.TakeDamage(this.damagePerTick, DamageSource.Fire);
        }

        #region Collisions
        protected virtual void OnPlayerCollision(PlayerController playerController)
        {
            // Reduce health
            playerController.ReduceHealth(this.damageDealtOnPlayerHit);

            // Knockback
            Vector2 direction = (playerController.transform.position - this.transform.position).normalized;
            direction *= this.knockbackForceOnHit;
            playerController.AddBackfireForce(direction);

            // Feedback sonore
            RuntimeManager.PlayOneShot(this.attackSound, this.transform.position);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(collision.gameObject.tag == GameObjectTags.Player)
            {
                PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    this.OnPlayerCollision(playerController);
                }
            }
        }
        #endregion

        #region Serialization
        public virtual void SaveXmlData()
        {
            throw new NotImplementedException();
        }

        public virtual XmlNode Serialize(XmlDocument document)
        {
            XmlNode rootnode = document.CreateElement(XmlRoot);
            {
                XmlAttribute enemyTypeAttribute = document.CreateAttribute(XmlEnemyType);
                {
                    enemyTypeAttribute.Value = this.enemyType.ToString();
                }
                rootnode.Attributes.Append(enemyTypeAttribute);

                XmlNode lifeNode = document.CreateElement(XmlLife);
                {
                    lifeNode.InnerText = this.m_life.ToString();
                }
                rootnode.AppendChild(lifeNode);

                XmlNode damageDealtOnPlayerHitNode = document.CreateElement(XmlDamageDealtOnPlayerHit);
                {
                    damageDealtOnPlayerHitNode.InnerText = this.damageDealtOnPlayerHit.ToString();
                }
                rootnode.AppendChild(damageDealtOnPlayerHitNode);
            }
            return rootnode;
        }

        protected virtual void LoadXmlData()
        {
            throw new NotImplementedException();
        }

        public virtual void Deserialize(XmlNode rootNode)
        {
            XmlNode lifeNode = rootNode.SelectSingleNode(XmlLife);
            {
                this.m_life = int.Parse(lifeNode.InnerText);
            }

            XmlNode damageDealtOnPlayerHitNode = rootNode.SelectSingleNode(XmlDamageDealtOnPlayerHit);
            {
                this.damageDealtOnPlayerHit = int.Parse(damageDealtOnPlayerHitNode.InnerText);
            }
        }
        #endregion

        #endregion
    }
}