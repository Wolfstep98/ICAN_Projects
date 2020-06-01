using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIAlwaysFaceCamera : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    new private Camera camera = null;

    private void Update()
    {
        this.transform.rotation = this.camera.transform.rotation;
    }

    private void OnValidate()
    {
        if(this.camera == null)
            this.camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }
}
