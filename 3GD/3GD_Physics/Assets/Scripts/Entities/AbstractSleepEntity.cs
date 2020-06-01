using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbstractSleepEntity : MonoBehaviour 
{
    #region Fields & Properties
    [Header("References")]
    [SerializeField]
    private AbstractPool advancedPool;
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
