using System;
using UnityEngine;

/// <summary>
/// This scripts allows to change the color of all particles systems to match the color of the light.
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(Light))]
public class LightColorUpdatesAllColors : MonoBehaviour
{
    #region Fields
    [Header("References")]
    [SerializeField] private new Light light = null;
    [SerializeField] private ParticleSystem[] particleSystems = null;

    [SerializeField] private bool updateAlpha = false;
    private Color previousColor = Color.black;
    #endregion

    private void Update()
    {
        if (this.light.color != this.previousColor)
        {
            for (int i = 0; i < this.particleSystems.Length; i++)
            {
                ParticleSystem particleSystem = this.particleSystems[i];
                ParticleSystem.MainModule mainModule = particleSystem.main;
                ParticleSystem.MinMaxGradient minMaxGradient = mainModule.startColor;
                Color tempColor = minMaxGradient.color;
                tempColor.r = this.light.color.r;
                tempColor.g = this.light.color.g;
                tempColor.b = this.light.color.b;
                if (this.updateAlpha) tempColor.a = this.light.color.a;
                minMaxGradient.color = tempColor;
                mainModule.startColor = minMaxGradient;
            }
            Debug.Log("Lights updated !");
            previousColor = this.light.color;
        }
    }

    private void OnValidate()
    {
        if(this.light == null)
        {
            this.light = this.GetComponent<Light>();
        }
    }
}
