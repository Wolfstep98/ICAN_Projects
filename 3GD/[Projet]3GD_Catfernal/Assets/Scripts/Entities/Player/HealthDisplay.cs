using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Entities.Player.Health {
    public class HealthDisplay : MonoBehaviour {

        #region Properties

        [SerializeField] private GameObject[] hearth;

        #endregion

        #region Initialization

    #if UNITY_EDITOR
        private void Awake(){
            if (this.hearth == null){
                Debug.LogError("[Missing References] - hearth not set properly !");
            }
            else{
                for (var i = 0; i < this.hearth.Length; i++){
                    if (this.hearth[i] == null)
                        Debug.LogError("[Missing References] - hearth " + i + " not set properly !");
                }
            }
        }
    #endif
        #endregion

        #region Methods

        public void UpdateDisplay(int health) {
            for (var i = 0; i < this.hearth.Length; i++){
                this.hearth[i].SetActive(i <= health - 1);
            }
        }

        #endregion
    }
}
