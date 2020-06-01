using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoaderPooler : MonoBehaviour 
{
    #region Fields & Properties
    [SerializeField]
    private Loader loader = null;
	#endregion

	#region Methods
	#region Initializers
	void Awake () 
	{
        this.Create();
	}
	#endregion

    public void Create()
    {
        this.loader.StartLoader();
        //Load machin
        this.loader.StopLoader();
    }
	#endregion
}
