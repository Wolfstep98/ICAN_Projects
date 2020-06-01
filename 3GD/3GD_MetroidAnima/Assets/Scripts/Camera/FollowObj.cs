using UnityEngine;

public class FollowObj : MonoBehaviour 
{
    #region Fields & Properties
    [SerializeField]
    private Vector3 offset = Vector3.zero;

    [Header("References")]
    [SerializeField]
    private Transform objToFollow = null;
	#endregion

	#region Methods
	#region Initializers
	// Use this for initialization
	void Awake () 
	{
		
	}
	#endregion
	
	private void Update () 
	{
        Vector3 position = objToFollow.position;
        position += this.offset;
        this.transform.position = position;
	}
	#endregion
}
