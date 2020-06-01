using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceBetweenTransforms : MonoBehaviour 
{
    #region Fields & Properties
    [Header("Parameters")]
    [SerializeField]
    private float distance = 0.0f;

    [Header("References")]
    [SerializeField]
    private Transform firstObj = null;
    [SerializeField]
    private Transform secondObj = null;
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
        if(this.firstObj != null && this.secondObj != null)
            this.distance = Vector3.Distance(firstObj.position, secondObj.position);
	}

    private void OnDrawGizmosSelected()
    {
        if (this.firstObj != null && this.secondObj != null)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawLine(firstObj.position, secondObj.position);
        }
    }
    #endregion
}
