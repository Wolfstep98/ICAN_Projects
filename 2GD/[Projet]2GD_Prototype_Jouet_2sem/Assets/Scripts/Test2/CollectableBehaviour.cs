using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableBehaviour : MonoBehaviour {

    [Header("Properties")]
    [Tooltip("Temps avant l'explosion (en sec)")]
    public float timer = 10f; 

    [Header("Debug")]
    public Color color;

    public MeshRenderer mesh;

    private void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        color = mesh.material.color;
    }

}
