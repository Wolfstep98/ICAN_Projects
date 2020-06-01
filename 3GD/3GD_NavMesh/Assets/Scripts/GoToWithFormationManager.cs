using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GoToWithFormationManager : MonoBehaviour 
{
    #region Fields & Properties
    [Header("Parameters")]
    [SerializeField]
    private MinionFormation formation = MinionFormation.Square;
    [Header("References")]
    [SerializeField]
    private NavMeshAgent[] agents = null;
    [SerializeField]
    private Transform target = null;
	#endregion

	#region Methods
	#region Initializers
	private void Awake () 
	{
#if UNITY_EDITOR
        if (this.agents == null)
            Debug.LogError("[Missing Reference] - agents is missing !");
        if (this.target == null)
            Debug.LogError("[Missing Reference] - target is missing !");
#endif
    }
	#endregion
	
    public void GoToPosFormation()
    {
        switch(this.formation)
        {
            case MinionFormation.Square:
                break;
            case MinionFormation.Circle:
                break;
            case MinionFormation.Triangle:
                break;
            case MinionFormation.Line:
                break;
            default:
                break;
        }
    }

    private Vector3[] GetLineFormationPosition()
    {
        return null;
    }

    private Vector3[] GetSquareFormationPosition()
    {
        return null;
    }

    private Vector3[] GetCircleFormationPosition()
    {
        return null;
    }

    private Vector3[] GetTriangleFormationPosition()
    {
        return null;
    }
    #endregion
}
