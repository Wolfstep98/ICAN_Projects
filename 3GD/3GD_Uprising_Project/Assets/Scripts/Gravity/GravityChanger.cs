using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GravityChanger : MonoBehaviour
{
    #region Fields
    [SerializeField]
    private float gravity = 0.0f;
    #endregion

    #region Properties
    public float Gravity { get { return this.gravity; } }
    #endregion
}
