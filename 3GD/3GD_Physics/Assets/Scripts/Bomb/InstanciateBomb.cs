using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstanciateBomb : MonoBehaviour 
{
    #region Fields & Properties
    [Header("References")]
    [SerializeField]
    private GameObject bombInstance = null;
    [Header("Prefabs")]
    [SerializeField]
    private GameObject bombPrefab = null;
	#endregion

	#region Methods
	#region Initializers
	// Use this for initialization
	void Awake () 
	{
#if UNITY_EDITOR
        if (this.bombPrefab == null)
            Debug.LogError("[Missing Reference] - bombPrefab is missing ! ");
#endif
    }
	#endregion
	
    public void SpawnBomb()
    {
        this.bombInstance = Instantiate<GameObject>(this.bombPrefab, this.transform.position, Quaternion.identity);
    }
	#endregion
}
