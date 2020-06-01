using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserInfernoBombBehaviour : MonoBehaviour
{
    #region Properties
    [Header("References")]
    [SerializeField] private Rigidbody2D rigidbody = null;
    [SerializeField] private Collider2D collider = null;
    [SerializeField] private ParticleSystem particles = null;

    [Header("Variables")]
    [SerializeField ,Range(0f, 10f)] private float speed = 1;

    #endregion

    #region Methods
    public void Shoot(Vector2 direction)
    {
        this.rigidbody.velocity = direction.normalized * this.speed;
    }

   
    #endregion
}
