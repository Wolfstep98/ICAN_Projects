using System;
using UnityEngine;

public class TornadoShader : MonoBehaviour
{
    [SerializeField] private bool takeSunDirection = false;

    private static readonly int shaderFresnelPowerID = Shader.PropertyToID("_FresnelPower");
    private static readonly int shaderFresnelGlowID = Shader.PropertyToID("_FresnelGlow");
    private static readonly int shaderFresnelColorID = Shader.PropertyToID("_FresnelColor");

    private static readonly int shaderEmissionID = Shader.PropertyToID("_Emission");
    private static readonly int shaderTextureTilingID = Shader.PropertyToID("_Tiling");
    private static readonly int shaderTextureSpeedID = Shader.PropertyToID("_TextureSpeed");
    private static readonly int shaderAlphaID = Shader.PropertyToID("_Alpha");

    private static readonly int shaderNumberOfWavesID = Shader.PropertyToID("_NumberOfWaves");
    private static readonly int shaderWaveSpeedID = Shader.PropertyToID("_WaveSpeed");

    private static readonly int shaderPositionID = Shader.PropertyToID("_Position");
    private static readonly int shaderSunDirectionID = Shader.PropertyToID("_SunDirection");

    [Header("Tornado")]
    [SerializeField] private Vector3 selectedScale = Vector3.one;
    [SerializeField] private Vector3 unselectedScale = Vector3.zero;

    [SerializeField] private Vector3 lowSelectedScale = Vector3.one;
    [SerializeField] private Vector3 lowUnselectedScale = Vector3.one;

    [Header("Fresnel")]

    [SerializeField] private float selectedPower = 0.8f;
    [SerializeField] private float unselectedPower = 0.0f;

    [SerializeField] private float selectedGlow = 0.8f;
    [SerializeField] private float unselectedGlow = 0.0f;

    [SerializeField, ColorUsage(false, true)] private Color selectedColor = Color.green;
    [SerializeField, ColorUsage(false, true)] private Color unselectedColor = Color.white;

    [Header("Alpha")]

    [SerializeField] private float[] selectedAlpha = { 1.0f, 0.373f, 0.24f };
    [SerializeField] private float[] unselectedAlpha = { 1.0f, 0.337f, 0.168f };

    [Header("Texture")]

    [SerializeField] private Vector2[] selectedTiling = { new Vector2(-1.0f, -1.0f), new Vector2(-2.92f, -4.69f), new Vector2(-2.05f,-2.02f)};
    [SerializeField] private Vector2[] unselectedTiling = { new Vector2(-1.0f, -1.0f), new Vector2(-1.0f, -1.0f), new Vector2(-1.0f, -1.0f) };

    [SerializeField] private Vector2[] selectedTextureSpeed = { new Vector2(0.5f, 0.5f), new Vector2(-2.92f, -4.69f), new Vector2(0.9f, 1.5f)};
    [SerializeField] private Vector2[] unselectedTextureSpeed = { new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f) };

    [Header("Waves")]

    [SerializeField] private float[] selectedWaveSpeed = { -0.5f, -2.0f, - 1.9f };
    [SerializeField] private float[] unselectedWaveSpeed = { -0.5f, -0.5f, -0.5f };

    [SerializeField] private float[] selectedWaveNumber = { 1.0f, 1.75f, 1.0f };
    [SerializeField] private float[] unselectedWaveNumber = { 1.0f, 1.0f, 1.0f };

    [Header("References")]
    [SerializeField] private Transform sun = null;
    [SerializeField] private Renderer[] tornadoRenderers = null;

    private void Awake()
    {
        this.UpdateSelected(false);
    }

    private void Update()
    {
        foreach(Renderer renderer in tornadoRenderers)
        {
            renderer.material.SetVector(shaderPositionID, this.transform.position);
            if(takeSunDirection && renderer.name == "Tornado - Low")
            {
                renderer.material.SetVector(shaderSunDirectionID, -this.sun.forward);
            }
        }
    }

    public void UpdateSelected(bool selected)
    {
        if (selected)
        {
            this.transform.localScale = this.selectedScale;

            int i = 0;
            foreach (Renderer renderer in tornadoRenderers)
            {
                //Scale for low
                if(i == 2)
                {
                    renderer.transform.localScale = this.lowSelectedScale;
                }

                // Fresnel
                renderer.material.SetColor(shaderFresnelColorID, this.selectedColor);
                renderer.material.SetFloat(shaderFresnelPowerID, this.selectedPower);
                renderer.material.SetFloat(shaderFresnelGlowID, this.selectedGlow);

                //Alpha
                renderer.material.SetFloat(shaderAlphaID, this.selectedAlpha[i]);

                //Texture
                renderer.material.SetVector(shaderTextureTilingID, this.selectedTiling[i]);
                renderer.material.SetVector(shaderTextureSpeedID, this.selectedTextureSpeed[i]);

                //Waves
                renderer.material.SetFloat(shaderWaveSpeedID, this.selectedWaveSpeed[i]);
                renderer.material.SetFloat(shaderNumberOfWavesID, this.selectedWaveNumber[i]);

                i++;
            }
        }
        else
        {
            this.transform.localScale = this.unselectedScale;

            int i = 0;
            foreach (Renderer renderer in tornadoRenderers)
            {                
                //Scale for low
                if (i == 2)
                {
                    renderer.transform.localScale = this.lowUnselectedScale;
                }

                // Fresnel
                renderer.material.SetColor(shaderFresnelColorID, this.unselectedColor);
                renderer.material.SetFloat(shaderFresnelPowerID, this.unselectedPower);
                renderer.material.SetFloat(shaderFresnelGlowID, this.unselectedGlow);

                //Alpha
                renderer.material.SetFloat(shaderAlphaID, this.unselectedAlpha[i]);

                //Texture
                renderer.material.SetVector(shaderTextureTilingID, this.unselectedTiling[i]);
                renderer.material.SetVector(shaderTextureSpeedID, this.unselectedTextureSpeed[i]);

                //Waves
                renderer.material.SetFloat(shaderWaveSpeedID, this.unselectedWaveSpeed[i]);
                renderer.material.SetFloat(shaderNumberOfWavesID, this.unselectedWaveNumber[i]);

                i++;
            }
        }
    }
}
