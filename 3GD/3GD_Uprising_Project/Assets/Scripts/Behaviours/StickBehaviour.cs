using System;
using System.Collections.Generic;
using UnityEngine;
using GameEvents;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class StickBehaviour : MonoBehaviour, IMassUpdated
{
    #region Fields & Properties
    [Header("Parameters")]
    [SerializeField]
    private bool canGrow = true;

    [SerializeField]
    private int maxJoints = 10;

    [Header("Mass")]
    [SerializeField]
    private bool useRigidbodyMass = true;

    [SerializeField]
    private float originalMass = 1.0f;

    [SerializeField]
    private float totalMass = 0.0f;
    public float TotalMass { get { return this.totalMass; } }

    [Header("Data")]
    [SerializeField]
    private List<FixedJoint> joints = null;

    [SerializeField]
    private List<GameObject> linkedObj = null;

    [Header("References")]
    [SerializeField]
    new private Rigidbody rigidbody = null;

    //---Events---
    public event EventHandler<StickEventArgs> OnGameObjectStick;

    public event EventHandler<MassUpdatedEventArgs> OnMassUpdated;

    #endregion

    #region Methods
    #region Initializers
    void Awake () 
	{
        this.Initialize();
	}

    private void Initialize()
    {
        this.joints = new List<FixedJoint>(this.maxJoints);

#if UNITY_EDITOR
        if (this.rigidbody == null)
            Debug.LogError("[Missing Reference] - rigidbody is not set !");
#endif

        this.InitializeMass();
    }
    #endregion

    #region Joints
    public void RemoveJoints()
    {
        for(int i = 0; i < this.joints.Count;i++)
        {
            this.RemoveJoint(i);
        }
    }

    public void RemoveJoint(int index)
    {
        if (index >= 0 && index < this.joints.Count)
        {
            FixedJoint joint = this.joints[index];

            this.joints[index].connectedBody = null;
            this.linkedObj.RemoveAt(index);

            joint.connectedBody.AddForce((this.joints[index].transform.position - this.transform.position) * 50.0f, ForceMode.Impulse);

            Destroy(this.joints[index]);

            this.joints.RemoveAt(index);

            this.UpdateTotalMass();
        }
        else
        {
            throw new IndexOutOfRangeException();
        }
    }
    #endregion

    #region Mass
    private void InitializeMass()
    {
        this.originalMass = (this.useRigidbodyMass) ? this.rigidbody.mass : this.originalMass;
        this.totalMass = this.originalMass;
    }

    private void UpdateTotalMass()
    {
        float previousMass = this.totalMass;
        float mass = 0.0f;
        mass += this.originalMass;
        foreach(GameObject obj in linkedObj)
        {
            if(obj.GetComponent<StickBehaviour>() == null && obj.GetComponent<BallController>() == null)
            {
                mass += obj.GetComponent<Rigidbody>().mass;
            }
        }
        this.totalMass = mass;

        //Raise update mass event
        MassUpdatedEventArgs args = new MassUpdatedEventArgs(previousMass, this.totalMass);
        this.RaiseOnMassUpdated(args);
    }

    private void RaiseOnMassUpdated(MassUpdatedEventArgs e)
    {
        EventHandler<MassUpdatedEventArgs> handler = this.OnMassUpdated;
        if(handler != null)
        {
            handler(this, e);
        }
    }
    #endregion

    #region Collisions
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != 0)
        {
            if (!this.linkedObj.Contains(collision.gameObject))
            {
                if(this.joints.Count < this.joints.Capacity)
                {
                    FixedJoint joint = this.gameObject.AddComponent<FixedJoint>();
                    joint.connectedBody = collision.rigidbody;
                    joint.connectedAnchor = collision.gameObject.transform.InverseTransformPoint(collision.contacts[0].point);
                    joint.anchor = this.gameObject.transform.InverseTransformPoint(collision.contacts[0].point);
                    this.joints.Add(joint);
                    this.linkedObj.Add(collision.gameObject);

                    //Stick event
                    StickEventArgs args = new StickEventArgs(collision.gameObject);
                    this.RaiseOnGameObjectStick(args);

                    this.UpdateTotalMass();
                }
                else if(this.joints.Count == this.joints.Capacity)
                {
                    int index = this.FindAvailableJoint();
                    if (index == -1)
                    {
                        if (this.canGrow)
                        {
                            FixedJoint joint = this.gameObject.AddComponent<FixedJoint>();
                            joint.connectedBody = collision.rigidbody;
                            joint.connectedAnchor = collision.gameObject.transform.InverseTransformPoint(collision.contacts[0].point);
                            joint.anchor = this.gameObject.transform.InverseTransformPoint(collision.contacts[0].point);
                            this.joints.Add(joint);
                            this.linkedObj.Add(collision.gameObject);

                            //Stick event
                            StickEventArgs args = new StickEventArgs(collision.gameObject);
                            this.RaiseOnGameObjectStick(args);

                            this.UpdateTotalMass();
                        }
                        else
                        {
                            Debug.Log("Max capacity reached !");
                        }
                    }
                    else
                    {
                        FixedJoint joint = this.joints[index];
                        joint.connectedBody = collision.rigidbody;
                        joint.connectedAnchor = collision.gameObject.transform.InverseTransformPoint(collision.contacts[0].point);
                        joint.anchor = this.gameObject.transform.InverseTransformPoint(collision.contacts[0].point);
                        this.joints[index] = joint;
                        this.linkedObj[index] = collision.gameObject;

                        //Stick event
                        StickEventArgs args = new StickEventArgs(collision.gameObject);
                        this.RaiseOnGameObjectStick(args);

                        this.UpdateTotalMass();
                    }
                }
            }
        }
    }

    private int FindAvailableJoint()
    {
        int index = -1;
        for(int i = 0; i < this.joints.Count;i++)
        {
            if(this.joints[i].connectedBody == null)
            {
                return i;
            }
        }
        return index;
    }

    protected virtual void RaiseOnGameObjectStick(StickEventArgs e)
    {
        EventHandler<StickEventArgs> handler = this.OnGameObjectStick;
        if(handler != null)
        {
            handler(this, e);
        }
    }
    #endregion

    #endregion
}
