using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    #region Fields & Properties
    [Header("Data")]
    [SerializeField]
    private bool onPlatform = false;
    [SerializeField, ReadOnly]
    private Vector3 lastPlatformPosition = Vector3.zero;
    [SerializeField]
    private int minionNumber = 0;
    [SerializeField]
    private MinionEntity[] minionEntities = null;

    [Header("Parameters")]
    [SerializeField]
    private bool inputUpdated = false;
    [SerializeField]
    private float mouseMaxDistance = 10.0f;
    [SerializeField]
    private float mouseCursorDistance = 5.0f;
    [SerializeField]
    private float speed = 0.0f;
    [SerializeField]
    private float maxSpeed = 10.0f;
    [SerializeField]
    private Vector2 mouseCursorDirection = Vector2.zero;
    [SerializeField]
    private Vector3 direction = Vector3.zero;

    [Header("References")]
    [SerializeField]
    new private Rigidbody rigidbody = null;
    [SerializeField]
    private Camera mainCamera = null;
    [SerializeField]
    private Transform platform = null;

    [Header("Debug")]
    [SerializeField]
    private bool showDirection = true;
	#endregion

	#region Methods
	#region Initializers
	private void Awake () 
	{
        this.Initialize();
	}

    private void Initialize()
    {
#if UNITY_EDITOR
        if (this.rigidbody == null)
            Debug.LogError("[Missing Reference] - rigidbody is missing !");
# endif

        this.InitializeData();
    }

    private void InitializeData()
    {
        this.mouseMaxDistance = Screen.height / 1.5f;
        this.minionNumber = 0;
        this.minionEntities = new MinionEntity[MinionPooler.MaxMinionsPerMap];
    }
    #endregion

    #region Update
    public void CustomUpdate()
    {
        //Move with the platform
        if (this.onPlatform)
        {
            Vector3 diff = this.platform.position - this.lastPlatformPosition;
            this.rigidbody.position += diff;
            this.lastPlatformPosition = this.platform.position;
        }
    }

    public void CustomFixedUpdate()
    {   
        if (this.inputUpdated)     
        {        
            this.ApplyMovement();
            this.inputUpdated = false;
        }
    }
    #endregion

    #region Movement
    private void ApplyMovement()
    {
        //Move
        this.direction = new Vector3(this.mouseCursorDirection.x, 0.0f, this.mouseCursorDirection.y).normalized;
        this.speed = this.maxSpeed * (this.mouseCursorDistance / this.mouseMaxDistance);
        this.direction *= this.speed;
        this.rigidbody.AddForceAtPosition(this.direction, this.transform.position + (Vector3.up / 2), ForceMode.Acceleration);
    }

    public void UpdateMovement(Vector3 mousePos)
    {
        Vector3 ballToScreen = this.mainCamera.WorldToScreenPoint(this.rigidbody.position);
        this.mouseCursorDirection = new Vector2(mousePos.x - ballToScreen.x, mousePos.y - ballToScreen.y).normalized;
        this.mouseCursorDistance = Vector2.Distance(mousePos, ballToScreen);
        this.mouseCursorDistance = ((this.mouseCursorDistance < this.mouseMaxDistance) ? this.mouseCursorDistance : this.mouseMaxDistance);
        this.mouseCursorDirection = this.mouseCursorDirection * this.mouseCursorDistance;
        this.inputUpdated = true;
    }
    #endregion

    #region Collisions
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == GameObjectTags.Minion)
        {
            this.AddMinion(collision.gameObject);
        }
        if (collision.gameObject.tag == GameObjectTags.Platform)
        {
            this.onPlatform = true;
            this.platform = collision.transform;
            this.lastPlatformPosition = collision.transform.position;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.tag == GameObjectTags.Platform)
        {
            if (this.platform == collision.transform)
            {
                this.onPlatform = false;
                this.platform = null;
            }
        }
    }
    #endregion

    #region Minions
    private void AddMinion(GameObject obj)
    {
        //Get the minion component
        MinionEntity entity = obj.GetComponent<MinionEntity>();
        if (entity == null)
            throw new MissingComponentException("[Ball Controller] - Missing Minion Entity component !");

        //Check if the minion is already on the list
        IList<MinionEntity> minions = this.minionEntities;
        if (minions.Contains(entity))
            return;

        //Add the minion and set the minion target to player
        this.minionEntities[this.minionNumber] = entity;
        this.minionNumber++;
        this.SetPlayerAsMinionTarget(entity);
        entity.RegisterEventMinionDeath(this.OnMinionDie);
        entity.StartFollow();
    }

    private void SetPlayerAsMinionTarget(MinionEntity entity)
    {
        entity.SetTarget(this.transform);
    }

    public void ReleaseMinions()
    {
        if (this.minionNumber == 0)
            return;

        for(int i = 0; i < this.minionNumber;i++)
        {
            this.minionEntities[i].StopFollow();
            this.minionEntities[i].ResetTarget();
            this.minionEntities[i].UnregisterEventMinionDeath(this.OnMinionDie);
            this.minionEntities[i] = null;
        }
        Debug.Log("[Player Controller] - Minions has been released.");
        this.minionNumber = 0;
    }

    private void OnMinionDie(MinionEntity entity)
    {
        IList<MinionEntity> minions = this.minionEntities;
        int index = minions.IndexOf(entity);
        this.minionEntities[index].UnregisterEventMinionDeath(this.OnMinionDie);
        this.minionEntities[index] = null;
        this.SortArray();
    }

    private void SortArray()
    {
        MinionEntity[] minions = new MinionEntity[MinionPooler.MaxMinionsPerMap];
        int minionsNumber = 0;
        for (int i = 0; i < this.minionEntities.Length; i++)
        {
            if (this.minionEntities[i] != null)
            {
                minions[minionsNumber] = this.minionEntities[i];
                minionsNumber++;
            }
        }
        this.minionNumber = minionsNumber;
        this.minionEntities = minions;
    }
    #endregion

    #region Editor
    private void OnDrawGizmosSelected()
    {
        if (this.showDirection)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawRay(this.transform.position, this.direction);
        }
    }
    #endregion

    #endregion
}
