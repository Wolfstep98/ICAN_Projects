using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ConstantForce : MonoBehaviour
{
    #region Fields & Properties
    [Header("Parameters")]
    [SerializeField]
    private bool isEnable = true;
    [SerializeField]
    private float force = 0.0f;
    [SerializeField]
    private Vector3 forceDirection = Vector3.zero;

    [Header("References")]
    [SerializeField]
    new private Rigidbody rigidbody = null;

    [Header("Debug")]
    [SerializeField]
    private bool showDebugForce = true;

    #endregion

    #region Properties
    public bool IsEnable { get { return this.isEnable; } set { this.isEnable = value; } }
    public float Force { get { return this.force; } set { this.force = value; } }
    #endregion

    #region Methods
    #region Editor
    private void OnValidate()
    {
        if(this.rigidbody == null)
            this.rigidbody = GetComponent<Rigidbody>();
    }
    #endregion

    #region Intialization
    private void Awake()
	{
		this.Initialize();
	}
	
	private void Initialize()
	{
#if UNITY_EDITOR
        if (this.rigidbody == null)
            Debug.LogError("[Missing Reference] - rigidbody is missing !");
#endif
    }

    private void FixedUpdate()
    {
        if (this.isEnable)
        {
            this.rigidbody.AddForce(this.forceDirection * this.force, ForceMode.Force);
        }
    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        if (this.showDebugForce)
        {
            if (this.isEnable)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawRay(this.transform.position, this.forceDirection * this.force);
            }
        }
    }

    #endregion
}
