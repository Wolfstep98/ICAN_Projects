
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CustomCameraController : MonoBehaviour {

    #region Properties
    [Header("References")]
    [SerializeField] private Transform target = null;
    [SerializeField] private Transform leftArm = null;
    [SerializeField] private Transform rightArm = null;

    [Header("Parameters")]
    //[SerializeField] private bool is3rdPerson = false;
    [SerializeField] private Vector3 positionOffset = Vector3.zero;

    //[SerializeField] bool smoothFollow = false;
    //[SerializeField] private Vector3 smoothingPosition = new Vector3(0.5f, 0.5f, 0.5f);

    //[SerializeField] bool autoRecenter = false;
    //[SerializeField] float timeBeforeRecenter = 1.0f;

    //[SerializeField] bool smoothRecenter = false;
    //[SerializeField] private Vector3 smoothingRotation = new Vector3(0.5f, 0.5f, 0.5f);

    [SerializeField] private float clampAngle = 80.0f;
    
    private Vector3 nextRotation;

    #endregion

    #region Initialization

    private void Awake()
    {
        this.Init();
    }

    private void Init()
    {
#if UNITY_EDITOR
        if (this.target == null)
            Debug.LogError("[Missing References] - target not set properly !");
#endif
        this.Initiate();
    }

    private void Initiate()
    {
        this.nextRotation = this.transform.localRotation.eulerAngles;
    }

    #endregion

    public void CustomLateUpdate()
    {
        this.UpdatePosition();
        this.UpdateRotation();
    }

    #region Methods

    private void UpdatePosition()
    {
        //if (this.smoothFollow) {
        //    this.transform.position = this.target.position + Quaternion.Euler(nextRotation) * positionOffset;
        //}
        //else {
            this.transform.position = this.target.position + Quaternion.Euler(nextRotation) * positionOffset;
        //}
    }

    public void SetRotation(float x, float y)
    {
        this.nextRotation = this.nextRotation + new Vector3(y * Time.deltaTime, -x * Time.deltaTime, 0);
        this.nextRotation.x = Mathf.Clamp(this.nextRotation.x, -this.clampAngle, this.clampAngle);
    }

    public void SetRotation(Vector2 rotation)
    {
        this.nextRotation = this.nextRotation + new Vector3(-rotation.y * Time.deltaTime, rotation.x * Time.deltaTime, 0);
        this.nextRotation.x = Mathf.Clamp(this.nextRotation.x, -this.clampAngle, this.clampAngle);
    }

    private void UpdateRotation()
    {
        this.transform.rotation = Quaternion.Euler(this.nextRotation);
    }

    public void RecenterCamera()
    {
        Vector3 vec = this.leftArm.position - this.rightArm.position;
        this.nextRotation = new Vector3(0.0f, this.target.localEulerAngles.y, 0.0f);
    }

    #endregion
}
