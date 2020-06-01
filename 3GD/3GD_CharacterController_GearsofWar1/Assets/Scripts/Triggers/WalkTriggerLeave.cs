using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WalkTriggerLeave : MonoBehaviour
{
    #region Fields & Properties
    [SerializeField]
    private CustomCameraBehaviourInput camInput = null;
    #endregion

    #region Methods
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            CustomCharacterControllerInput input = other.gameObject.GetComponent<CustomCharacterControllerInput>();
            input.OnWalkZoneLeaved();
            this.camInput.OnWalkZoneLeave();
        }
    }
    #endregion
}
