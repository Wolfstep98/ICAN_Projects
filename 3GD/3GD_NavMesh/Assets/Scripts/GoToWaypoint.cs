using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GoToWaypoint : MonoBehaviour 
{
    #region Fields & Properties
    [Header("References")]
    [SerializeField]
    private NavMeshAgent agent = null;
    [SerializeField]
    private Waypoint waypoint = null;
	#endregion

	#region Methods
	#region Initializers
	private void Awake () 
	{
#if UNITY_EDITOR
        if (this.agent == null)
            Debug.LogError("[Missing Reference] - agent is missing !");
        if (this.waypoint == null)
            Debug.LogError("[Missing Reference] - waypoint is missing !");
#endif
    }
    #endregion

    private void Update()
    {
        if (!this.agent.pathPending && Vector3.Distance(this.agent.transform.position, this.waypoint.transform.position) < this.agent.stoppingDistance)
        {
            this.UpdateWaypoint();
            this.GoToCurrentWaypoint();
        }
    }

    public void GoToCurrentWaypoint()
    {
        this.agent.SetDestination(this.waypoint.transform.position);
    }

    private void UpdateWaypoint()
    {
        if (this.waypoint.NextWaypoint != null)
        {
            this.waypoint = this.waypoint.NextWaypoint;
        }
    }
	#endregion
}
