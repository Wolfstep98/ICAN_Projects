using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class FollowTarget : MonoBehaviour 
{
    #region Fields & Properties
    [Header("Parameters")]
    [SerializeField]
    private bool isActive = false;
    [SerializeField]
    private float followThreshold = 0.5f;
    [SerializeField]
    private Vector3 lastStationaryPosition = Vector3.zero;

    [Header("References")]
    [SerializeField]
    private NavMeshAgent agent = null;
    [SerializeField]
    private Transform target = null;
	#endregion

	#region Methods
	#region Initializers
	private void Awake () 
	{
#if UNITY_EDITOR
        if (this.agent == null)
            Debug.LogError("[Missing Reference] - agent is missing !");
        if (this.target == null)
            Debug.LogError("[Missing Reference] - target is missing !");
#endif
        this.isActive = false;
    }
    #endregion

    private void Update()
    {
        if (this.isActive)
        {
            if ((this.lastStationaryPosition - this.target.transform.position).magnitude >= this.followThreshold)
            {
                this.SetDestination();
            }
        }
    }

    public void StartFollow()
    {
        this.isActive = true;
        this.agent.SetDestination(this.target.position);
    }

    public void SetDestination()
    {
        this.agent.SetDestination(this.target.position);
        this.lastStationaryPosition = this.target.transform.position;
    }

    public void StopFollow()
    {
        this.isActive = false;
        this.agent.ResetPath();
    }
	#endregion
}
