using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WalkTrigger : MonoBehaviour
{
    #region Fields & Properties
    [Header("Parameters")]
    [SerializeField]
    private Vector3 direction = Vector3.zero;
    [Header("References")]
    [SerializeField]
    private CustomCameraBehaviourInput camInput = null;
    #endregion

    #region Methods
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            CustomCharacterControllerInput input = other.gameObject.GetComponent<CustomCharacterControllerInput>();
            input.OnWalkZoneEntered(this.direction.normalized);
            this.camInput.OnWalkZoneEnter();
        }
    }

    #region Debug
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawRay(this.transform.position, this.direction);
    }
    #endregion
    #endregion
}
