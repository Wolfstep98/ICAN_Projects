using System;
using UnityEngine;


[Serializable]
public class IndexOutOfPoolerException : ApplicationException
{
    public IndexOutOfPoolerException() { }
    public IndexOutOfPoolerException(string message) : base(message) { }
    public IndexOutOfPoolerException(string message, System.Exception inner) : base(message, inner) { }
    protected IndexOutOfPoolerException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}

public class AdvancedSleepEntity : MonoBehaviour 
{
    #region Fields & Properties
    [Header("References")]
    [SerializeField]
    private AdvancedPool advancedPool;
    #endregion

    #region Methods
    public void SleepLastActiveEntity()
    {
        if (this.advancedPool.PoolerReady)
        {
            int index = this.advancedPool.CurrentIndex - 1;
            if (index < 0)
                throw new IndexOutOfPoolerException("You can't sleep more object, they are already all sleeping !");
            this.advancedPool.Sleep(this.advancedPool.Entities[index]);
        }
    }
	#endregion
}
