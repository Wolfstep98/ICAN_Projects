
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSameColor : MonoBehaviour {

    #region Properties

    [SerializeField] private new SpriteRenderer renderer;
    [SerializeField] private SpriteRenderer target;
    
    #endregion

    #region Methods

    private void Update() {
        this.renderer.color = this.target.color;
    }

    #endregion
}
