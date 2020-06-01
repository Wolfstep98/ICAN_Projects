using System;
using UnityEngine;

public class ArmRotation : MonoBehaviour
{
    #region Fields & Properties
    [SerializeField]
    private Transform root = null;

    [SerializeField]
    private Vector3 initialRotation = Vector3.zero;
    [SerializeField]
    private Vector2 initialStickRotation = new Vector2(0.0f, 1.0f);

    [SerializeField]
    private Vector3 rotation = Vector3.zero;
    [SerializeField]
    private Vector3 rotationAxis = Vector3.zero;
    [SerializeField]
    private Vector3 vectorToCross = Vector3.back;
    [SerializeField]
    private Vector3 rotationTarget = Vector3.zero;

    [Header("Angle X")]
    [SerializeField]
    private float angleX = 0.0f;
    [SerializeField]
    private float angleXMultiplicator = 1.0f;
    [SerializeField]
    private float angleXToRotation = 90.0f;
    [SerializeField]
    private float minimumAngleXThreshold = 0.15f;
    [SerializeField]
    private Vector2 angleXMax = new Vector2(0.0f, 90.0f);

    [Header("Angle X")]
    [SerializeField]
    private float angleY = 0.0f;
    [SerializeField]
    private float angleYMultiplicator = 1.0f;
    [SerializeField]
    private float angleYToRotation = 90.0f;
    [SerializeField]
    private float minimumAngleYThreshold = 0.15f;
    [SerializeField]
    private Vector2 angleYMax = new Vector2(-90.0f, 90.0f);

    [Header("Angle")]
    [SerializeField]
    private float angleToRotation = -90.0f;

    [Header("Debug")]
    [SerializeField]
    private bool showSpheres = false;
    [SerializeField]
    private bool showRays = false;
    #endregion

    #region Methods
    public void CustomUpdate()
    {
        //this.root.localRotation = Quaternion.Euler(this.angleX, 0.0f , this.angleY);
    }

    public void UpdateRotation(Vector3 rotation)
    {
        //this.angleX = rotation.x * this.angleXToRotation * this.angleXMultiplicator;
        //this.AngleXVerification();
        //this.angleY = rotation.y * this.angleYToRotation * this.angleYMultiplicator;
        //this.AngleYVerification();

        //New Behaviour

        //this.root.rotation = Quaternion.identity;

        //this.rotation = rotation;

        //float angle = Vector2.Angle(this.initialStickRotation, rotation);
        ////Debug.Log(angle);
        //if (rotation.x < 0.0f)
        //    angle *= -1;
        //this.root.Rotate(Vector3.up, angle, Space.World);

        //float doubleAngle = rotation.magnitude;
        //if (Math.Abs(doubleAngle) > 1.0f)
        //{
        //    doubleAngle = 1.0f;
        //    this.rotation.Normalize();
        //}

        //doubleAngle *= this.angleToRotation;

        //this.root.Rotate(Vector3.right, doubleAngle, Space.Self);

        //Better rotation

        this.root.rotation = Quaternion.Euler(this.initialRotation);

        Vector3 planeRotation = new Vector3(rotation.x, 0.0f, rotation.y);

        if (this.root.rotation == Quaternion.identity)
            this.rotation = planeRotation;
        else
            this.rotation = -planeRotation;

        Vector3 normal = Vector3.Cross(this.rotation, this.vectorToCross);
        this.rotationAxis = -normal;

        float doubleAngle = rotation.magnitude;
        if (Math.Abs(doubleAngle) > 1.0f)
        {
            doubleAngle = 1.0f;
            this.rotation.Normalize();
        }

        doubleAngle *= this.angleToRotation;

        this.rotationTarget = Vector3Calculus.RotateTowardsAxisAtAngle(this.rotation, doubleAngle);

        this.root.Rotate(this.rotationAxis, doubleAngle, Space.World);
    }

    private void AngleXVerification()
    {
        if (Math.Abs(this.angleX) < this.minimumAngleXThreshold)
            this.angleX = 0.0f;

        if (this.angleX < this.angleXMax.x)
            this.angleX = this.angleXMax.x;
        else if (this.angleX > this.angleXMax.y)
            this.angleX = this.angleXMax.y;
    }

    private void AngleYVerification()
    {
        if (Math.Abs(this.angleY) < this.minimumAngleYThreshold)
            this.angleY = 0.0f;

        if (this.angleY < this.angleYMax.x)
            this.angleY = this.angleYMax.x;
        else if (this.angleY > this.angleYMax.y)
            this.angleY = this.angleYMax.y;
    }

    public void ChangeInitialRotation(float value)
    {
        if (value <= 0.0f)
            this.initialRotation = Vector3.zero;
        else
            this.initialRotation = Vector3.forward * 180.0f;
    }

    private void OnDrawGizmosSelected()
    {
        //Vector3 planeInitialRotation = new Vector3(this.initialStickRotation.x, 0.0f, this.initialStickRotation.y);
        Vector3 planeRotation = new Vector3(rotation.x, 0.0f, rotation.y);
        //Debug.DrawRay(this.root.position, planeInitialRotation, Color.black);
        Debug.DrawRay(this.root.position, planeRotation, Color.red);

        if (this.showSpheres)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(this.root.position, 1.0f);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(this.root.position, rotation.magnitude);
        }

        if (this.showRays)
        {
            Debug.DrawRay(this.root.position, this.rotationAxis, Color.green);
            Debug.DrawRay(this.root.position, this.vectorToCross, Color.blue);
            Debug.DrawRay(this.root.position, this.rotation, Color.red);
            Debug.DrawRay(this.root.position, this.rotationTarget, Color.white);
        }
    }
    #endregion
}
