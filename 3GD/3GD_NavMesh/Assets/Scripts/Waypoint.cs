using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour 
{
    #region Fields & Properties
    [Header("References")]
    [SerializeField]
    private Waypoint nextWaypoint = null;
    public Waypoint NextWaypoint { get { return this.nextWaypoint; } }
	#endregion

	#region Methods
	#region Initializers
	// Use this for initialization
	void Awake () 
	{
		
	}
	#endregion
	
	// Update is called once per frame
	void Update () 
	{
		
	}

    private void OnDrawGizmos()
    {
        if (this.nextWaypoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(this.transform.position, this.nextWaypoint.transform.position);
        }
    }
    #endregion
}
