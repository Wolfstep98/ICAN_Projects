using System;
using System.Collections.Generic;
using UnityEngine;
using Game.Entities.Player;

namespace Game
{
    /// <inheritdoc />
    /// <summary>
    /// GameManager : Roots of all the scripts.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        #region Fields
        [Header("References")]
        [SerializeField]
        private GameBehavior[] gameBehaviors = null;
        [SerializeField]
        private List<GameBehavior> dynamicGameBehaviors = null;

        [SerializeField]
        private PlayerManager playerManager = null;
        #endregion

        #region Methods
        #region Initialization
        private void Awake()
        {
#if UNITY_EDITOR
            #region Check for null reference
            if (this.gameBehaviors == null)
            {
                Debug.LogError("[Null Reference] - gameBehaviors are not properly set !");
            }
            else
            {
                for (var i = 0; i < this.gameBehaviors.Length; i++)
                {
                    if (this.gameBehaviors[i] == null)
                        Debug.LogError("[Null Reference] - gameBehaviors " + i + " are not properly set !");
                }
            }
            if (this.playerManager == null)
                Debug.LogError("[Null Reference] - playerManager are not properly set !");
            #endregion
#endif
            foreach (var gameBehaviour in this.gameBehaviors)
            {
                gameBehaviour.CustomAwake();
            }

            this.playerManager.CustomAwake();

            foreach (var gameBehaviour in this.dynamicGameBehaviors)
            {
                gameBehaviour.CustomAwake();
            }
        }
        #endregion

        #region Properties
        public List<GameBehavior> DynamicGameBehavior { get { return this.dynamicGameBehaviors; } }
        #endregion

        #region Updates
        private void Update()
        {
            foreach (var gameBehaviour in this.gameBehaviors)
            {
                gameBehaviour.CustomUpdate();
            }

            this.playerManager.CustomUpdate();

            foreach (var gameBehaviour in this.dynamicGameBehaviors)
            {
                gameBehaviour.CustomUpdate();
            }
        }

        private void FixedUpdate()
        {
            foreach (var gameBehaviour in this.gameBehaviors)
            {
                gameBehaviour.CustomFixedUpdate();
            }

            this.playerManager.CustomFixedUpdate();

            foreach (var gameBehaviour in this.dynamicGameBehaviors)
            {
                gameBehaviour.CustomFixedUpdate();
            }
        }
        #endregion
        #endregion
    }
}
