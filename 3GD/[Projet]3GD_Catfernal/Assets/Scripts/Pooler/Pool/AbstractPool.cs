using System;
using System.Collections.Generic;
using UnityEngine;
using Game;

namespace DesignPattern.ObjectPooling
{
    /// <summary>
    /// Abstract pool template. Inherits from this class to create a pooler for a specific object.
    /// </summary>
    /// <typeparam name="T">T must inherits from MonoBehaviour and implements IEntity interface.</typeparam>
    [Serializable]
    public abstract class AbstractPool<T> : GameBehavior where T : GameBehavior, IEntity
    {
        #region Fields
        [Header("Parameters")]
        [SerializeField] private bool isGrowing = false;
        [SerializeField] private int growingNumber = 3;
        [SerializeField] private int entityNumber = 0;
        [SerializeField] private GameObject entityPrefab = null;
        [SerializeField] private Transform container = null;

        [Header("Data")]
        [SerializeField] private bool isPoolerReady = false;
        [SerializeField] protected T[] entities = null;
        private List<GameBehavior> gameBehaviors = null;

        [Header("References")]
        [SerializeField]
        private GameManager gameManager = null;
        #endregion

        #region Initialization
        public override void CustomAwake()
        {
            this.Initialize();
        }

        protected virtual void Initialize()
        {
            //Check for errors with missing references and missing components
#if UNITY_EDITOR
            if (this.entityPrefab == null)
                Debug.LogError("[Missing Reference] - entity is missing !");
            if (this.entityPrefab.GetComponent<T>() == null)
                Debug.LogError("[Missing Component] - T component is missing !");
            if (this.gameManager == null)
                Debug.LogError("[Missing Component] - gameManager is missing !");
#endif

            //Instantiate the entities
            this.entities = new T[this.entityNumber];
            this.gameBehaviors = this.gameManager.DynamicGameBehavior;
            this.InstantiateEntities(0);
            this.isPoolerReady = true;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Is the pooler ready to be used.
        /// </summary>
        public bool IsPoolerReady { get { return this.isPoolerReady; } }

        /// <summary>
        /// Array containing all the entities.
        /// </summary>
        public T[] Entities { get { return this.entities; } }
        #endregion

        #region Methods
        /// <summary>
        /// Get an available entity in the pool. Return null if none.
        /// </summary>
        /// <returns>Return the 1st available entity in the pool. Return null if none.</returns>
        public T GetEntity()
        {
            for (int i = 0; i < this.entities.Length; i++)
            {
                if (!this.entities[i].IsEnable)
                {
                    this.entities[i].Enable();
                    return this.entities[i];
                }
            }
            if(this.isGrowing)
            {
                int lenght = this.entities.Length;
                T[] temp = new T[this.entities.Length + growingNumber];
                Array.Copy(this.entities, 0, temp, 0, lenght);
                this.entities = temp;
                this.entityNumber = this.entities.Length;
                InstantiateEntities((uint)lenght);
                return this.GetEntity();
            }

            return null;
        }

        /// <summary>
        /// Reset the pooler, disable all the gameObject in the Hierarchy.
        /// </summary>
        public void ResetPooler()
        {
            for (int i = 0; i < this.entities.Length; i++)
            {
                if (this.entities[i].IsEnable)
                {
                    this.entities[i].Disable();
                }
            }
        }

        /// <summary>
        /// Instantiate entity in entities.
        /// </summary>
        /// <param name="startingIndex">Starting index in entities.</param>
        protected virtual void InstantiateEntities(uint startingIndex = 0)
        {
            for(uint i = startingIndex; i < this.entities.Length;i++)
            {
                this.entities[i] = this.InstantiateEntity();
                gameBehaviors.Add(this.entities[i]);
            }
        }

        protected virtual T InstantiateEntity()
        {
            GameObject obj = Instantiate<GameObject>(this.entityPrefab, Vector3.zero, Quaternion.identity, this.container ?? null);
            T entity = obj.GetComponent<T>();
            entity.Initialize();
            return entity;
        }
        #endregion
    }
}