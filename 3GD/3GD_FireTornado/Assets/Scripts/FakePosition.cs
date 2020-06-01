using System;
using UnityEngine;

[ExecuteInEditMode]
public class FakePosition : MonoBehaviour
{
    private static readonly int shaderFakePosID = Shader.PropertyToID("_FakePos");

    [Header("References")]
    [SerializeField] private Renderer shader = null;
    [SerializeField] private Transform fake = null;

    private void Update()
    {
        this.shader.sharedMaterial.SetVector(shaderFakePosID, this.fake.transform.position);
    }
}
