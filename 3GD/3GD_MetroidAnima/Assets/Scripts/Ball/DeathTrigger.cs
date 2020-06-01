using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DeathTrigger : MonoBehaviour 
{
    #region Fields
    [SerializeField]
    private MinionsManager minionsManager = null;
    #endregion

    #region Methods
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == GameObjectTags.Player)
        {
            this.minionsManager.RespawnPlayer();
        }
    }
    #endregion
}
