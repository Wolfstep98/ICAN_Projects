using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class UnityEventInt : UnityEvent<int>
{

}

[RequireComponent(typeof(Collider))]
public class ChangeBehaviourOnCollision : MonoBehaviour
{
    #region Fields & Properties
    [Header("Parameters")]
    [SerializeField]
    private BallController.PhysicState state = BallController.PhysicState.Normal;

    [Header("Events")]
    [SerializeField]
    private UnityEventInt OnCollision;
    #endregion

    //Collisions 3D
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            this.OnCollision.Invoke((int)this.state);
        }
    }
}
