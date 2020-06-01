using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetUpSelectionAnimation : MonoBehaviour {

    public int nextSceneIndex = 1;

	// Use this for initialization
	void OnEnable ()
    {
        Animator anim = GetComponent<Animator>();
        anim.SetInteger("NextScene", nextSceneIndex);
	}
	
}
