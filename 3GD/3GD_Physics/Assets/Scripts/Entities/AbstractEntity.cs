using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbstractEntity : AbstractPoolEntity 
{
    #region Fields & Properties

    #endregion

    #region Methods
    public override void Init(AbstractPool pool)
    {
        base.Init(pool);
    }

    public override void WakeUp()
    {
        base.WakeUp();
    }

    public override void GoToSleep()
    {
        base.GoToSleep();
    }
    #endregion
}
