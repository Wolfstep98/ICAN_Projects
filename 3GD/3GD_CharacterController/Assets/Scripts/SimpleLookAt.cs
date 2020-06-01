using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SimpleLookAt : MonoBehaviour
{
    #region Fields & Properties
    [Header("Properties")]
    [SerializeField]
    private bool lookAtTarget = true;

    [Header("Reference")]
    [SerializeField]
    private GameObject target = null;

    [SerializeField]
    new private Camera camera = null;
    #endregion

    #region Methods
    #region Initialization
    private void Awake()
    {
        this.Initialize();
    }

    private void Initialize()
    {
#if UNITY_EDITOR
        if (this.target == null)
            Debug.LogError("[Missing Reference] - target not set !");
        if (this.camera == null)
            Debug.LogError("[Missing Reference] - camera not set !");
#endif
    }
    #endregion

    private void Update()
    {
        if(this.lookAtTarget)
            this.LookAtTarget();
    }

    /// <summary>
    /// Make the camera look at the target.
    /// </summary>
    public void LookAtTarget()
    {
        this.transform.LookAt(this.target.transform, Vector3.up);
    }
    #endregion

}
