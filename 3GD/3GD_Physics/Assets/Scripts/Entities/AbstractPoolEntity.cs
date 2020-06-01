using System;
using UnityEngine;

[Serializable]
public abstract class AbstractPoolEntity : MonoBehaviour 
{
    #region Fields & Properties
    [SerializeField]
    protected AbstractPool pool = null;
	#endregion

	#region Methods
    public virtual void Init(AbstractPool pool)
    {
        this.pool = pool;
    }

    public virtual void WakeUp()
    {
        this.gameObject.SetActive(true);
    }

    public virtual void GoToSleep()
    {
        this.gameObject.SetActive(false);
    }
	#endregion
}
