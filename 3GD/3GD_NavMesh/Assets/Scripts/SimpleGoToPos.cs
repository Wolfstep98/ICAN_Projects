using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimpleGoToPos : MonoBehaviour 
{
    #region Fields & Properties
    [Header("References")]
    [SerializeField]
    private NavMeshAgent agent = null;
    [SerializeField]
    private Transform destination = null;
	#endregion

	#region Methods
	#region Initializers
	private void Awake () 
	{
#if UNITY_EDITOR
        if (this.agent == null)
            Debug.LogError("[Missing Reference] - agent is missing !");
        if (this.destination == null)
            Debug.LogError("[Missing Reference] - destination is missing !");
#endif
    }
#endregion
	
    public void GoToDestination()
    {
        this.agent.SetDestination(this.destination.position);
    }

#endregion
}
