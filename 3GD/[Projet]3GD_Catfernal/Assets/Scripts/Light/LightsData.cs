using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

public class LightsData : GameBehavior
{
    #region Fields
    [Header("Data")]
    [SerializeField] private Vector2 size = Vector2.zero;
    [SerializeField] private Transform ligthTransform = null;
    [SerializeField] private CircleCollider2D circleCollider2D = null;
    #endregion

    #region Properties
    public Vector2 Size
    {
        get { return this.size; }
        set
        {
            this.size = value;
            this.circleCollider2D.radius = value.y / 2.0f;
        }
    }

    public Transform LightTransform { get { return this.ligthTransform; } }
    #endregion

    private void OnEnable() {
        if(LightsDataExportBuffer.LightsDataExporter.commands.Contains(this)) return;
        LightsDataExportBuffer.LightsDataExporter.commands.Add(this);
    }

    private void OnDisable() {
        LightsDataExportBuffer.LightsDataExporter.commands.Remove(this);
    }
}
