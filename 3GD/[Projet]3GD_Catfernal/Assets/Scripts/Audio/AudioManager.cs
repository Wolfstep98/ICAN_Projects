
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    #region Properties



    #endregion

    #region Initialization

    private void Awake(){
        this.Init();
    }

    private void Init(){
#if UNITY_EDITOR
        //if (this.var == null)
        //    Debug.LogError("[Missing References] - var not set properly !");
#endif
        this.Initiate();
    }

    private void Initiate(){

    }

    #endregion



    #region Methods



    #endregion
}
