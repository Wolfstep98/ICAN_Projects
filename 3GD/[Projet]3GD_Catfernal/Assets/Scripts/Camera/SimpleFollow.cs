
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFollow : MonoBehaviour {

    #region Properties

    [SerializeField] private Transform target;

    #endregion

    #region Initialization

    private void Awake(){
        this.Init();
    }

    private void Init(){
#if UNITY_EDITOR
        if (this.target == null)
            Debug.LogError("[Missing References] - target not set properly !");
#endif
    }

    #endregion

    #region Methods

    private void Update() {
        this.transform.position = this.target.position;
    }

    #endregion
}
