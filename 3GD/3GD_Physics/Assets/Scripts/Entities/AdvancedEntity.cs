using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedEntity : MonoBehaviour 
{
    #region Fields & Properties


    [Header("References")]
    [SerializeField]
    private AdvancedPool pool = null;
	#endregion

	#region Methods
    public void Init(AdvancedPool pool)
    {
        this.pool = pool;
    }

    public void WakeUp()
    {
        this.gameObject.SetActive(true);
    }

    public void GoToSleep()
    {
        this.gameObject.SetActive(false);
    }
	#endregion
}
