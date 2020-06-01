using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour 
{
    #region Fields & Properties
    [Header("Parameters")]
    [SerializeField]
    private bool useRigidbodyMass = true;
    [SerializeField]
    private float initialMass = 1.0f;

    [SerializeField]
    private float totalMass = 0.0f;
    public float TotalMass { get { return this.totalMass; } }

    [Header("References")]
    [SerializeField]
    new private Rigidbody rigidbody = null;

    [SerializeField]
    private List<StickBehaviour> linkedGameObjects = null;
	#endregion

	#region Methods
	#region Initializers
	void Awake () 
	{
        this.Initialize();
	}

    private void Initialize()
    {
#if UNITY_EDITOR
        if (this.rigidbody == null)
            Debug.LogError("[Missing Reference] - rigidbody is not set !");
#endif
        this.InitializeMass();
    }

    private void InitializeMass()
    {
        this.initialMass = (this.useRigidbodyMass) ? this.rigidbody.mass : this.initialMass;
        this.totalMass = this.initialMass;
    }
#endregion

    #region Mass
    public void UpdateCurrentMass()
    {
        float mass = 0;
        mass += this.initialMass;
        for(int i = 0; i < this.linkedGameObjects.Count;i++)
        {
            mass += this.linkedGameObjects[i].TotalMass;
        }
        this.totalMass = mass;
        this.rigidbody.mass = mass;
    }
    #endregion

    #region Stick behaviours

    public void DisableStickBehaviours()
    {  
        for(int i = 0; i < this.linkedGameObjects.Count;i++)
        {
            this.linkedGameObjects[i].OnGameObjectStick -= this.StickBehaviour_OnGameObjectStick;
            this.linkedGameObjects[i].OnMassUpdated -= this.StickBehaviour_OnMassUpdated;
            this.linkedGameObjects[i].RemoveJoints();
            this.linkedGameObjects.RemoveAt(i);
        }

        this.UpdateCurrentMass();
    }

    public void OnGameObjectCollision(GameObject gameObject)
    {
        StickBehaviour behaviour = gameObject.GetComponent<StickBehaviour>();
        this.AddStickBehaviour(behaviour);
    }

    private void AddStickBehaviour(StickBehaviour behaviour)
    {
        if (behaviour != null)
        {
            if (!this.linkedGameObjects.Contains(behaviour))
            {
                this.linkedGameObjects.Add(behaviour);
                behaviour.OnGameObjectStick += this.StickBehaviour_OnGameObjectStick;
                behaviour.OnMassUpdated += this.StickBehaviour_OnMassUpdated;
            }
        }
    }

    //Called when a stick behaviour stick a gameObject
    private void StickBehaviour_OnGameObjectStick(object sender, GameEvents.StickEventArgs e)
    {
        StickBehaviour behaviour = e.StickObject.GetComponent<StickBehaviour>();
        this.AddStickBehaviour(behaviour);
    }

    private void StickBehaviour_OnMassUpdated(object sender, GameEvents.MassUpdatedEventArgs e)
    {
        this.UpdateCurrentMass();
    }

    #endregion
    #endregion
}
