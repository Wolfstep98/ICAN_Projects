using UnityEngine;
using System.Collections;

[UnityEngine.ExecuteInEditMode]
public class DoubleBufferEvolve : MonoBehaviour
{
    [UnityEngine.SerializeField]
    private UnityEngine.RenderTextureFormat renderTextureFormat = UnityEngine.RenderTextureFormat.ARGB32;

    [UnityEngine.SerializeField]
    private int resetStateEvery = 0;

    [UnityEngine.SerializeField]
    private int size = 512;

    [UnityEngine.SerializeField]
    private UnityEngine.Material firstPassMaterial = null;

    [UnityEngine.SerializeField]
    private UnityEngine.Material evolveMaterial = null;

    [UnityEngine.SerializeField]
    private string inputMaterialName = "_MainTex";

    [UnityEngine.SerializeField]
    private UnityEngine.Material displayMaterial = null;

    [UnityEngine.SerializeField]
    private new UnityEngine.Camera camera = null;

    private UnityEngine.RenderTexture renderTexture0;
    private UnityEngine.RenderTexture renderTexture1;
    private bool renderTextureIndex = false;
    private int frameIndex;

    void Update()
    {
        UnityEngine.Vector2 mousePos = new Vector2(-100, -100);
        if (this.camera != null)
        {
            Ray mouseRay = this.camera.ScreenPointToRay(UnityEngine.Input.mousePosition);
            RaycastHit hit;
            if (UnityEngine.Physics.Raycast(mouseRay, out hit) && hit.collider.transform == this.transform)
            {
                UnityEngine.Vector3 localPos = hit.collider.transform.worldToLocalMatrix.MultiplyPoint(hit.point);
                mousePos = new UnityEngine.Vector2(localPos.x + 0.5f, localPos.y + 0.5f);
            }
        }

        if (!Input.GetMouseButton(0))
        {
            mousePos = new UnityEngine.Vector2(-100, -100);
        }

        if (this.renderTexture0 != null)
        {
            ++this.frameIndex;
            this.renderTextureIndex = !this.renderTextureIndex;
            UnityEngine.RenderTexture inputRenderTexture = this.renderTextureIndex ? this.renderTexture0 : this.renderTexture1;
            UnityEngine.RenderTexture outputRenderTexture = this.renderTextureIndex ? this.renderTexture1 : this.renderTexture0;

            if (this.evolveMaterial)
            {
                this.evolveMaterial.SetVector("_MousePos", mousePos);
                UnityEngine.Graphics.Blit(inputRenderTexture, outputRenderTexture, this.evolveMaterial, 0);
            }
            
            if (this.resetStateEvery > 0 && (this.frameIndex % this.resetStateEvery == 0))
            {
                if (this.firstPassMaterial)
                {
                    UnityEngine.Graphics.Blit(inputRenderTexture, outputRenderTexture, this.firstPassMaterial, 0);
                }
            }
            
            if (this.displayMaterial)
            {
                this.displayMaterial.SetTexture(this.inputMaterialName, outputRenderTexture);
            }
        }
    }

	// Use this for initialization
	void OnEnable ()
    {
        this.renderTexture0 = new RenderTexture(this.size, this.size, 0, this.renderTextureFormat, RenderTextureReadWrite.Linear);
        this.renderTexture0.Create();
        this.renderTexture1 = new RenderTexture(this.size, this.size, 0, this.renderTextureFormat, RenderTextureReadWrite.Linear);
        this.renderTexture1.Create();
        this.renderTextureIndex = false;
        UnityEngine.RenderTexture inputRenderTexture = this.renderTextureIndex ? this.renderTexture0 : this.renderTexture1;
        UnityEngine.RenderTexture outputRenderTexture = this.renderTextureIndex ? this.renderTexture1 : this.renderTexture0;

        if (this.firstPassMaterial)
        {
            UnityEngine.Graphics.Blit(inputRenderTexture, outputRenderTexture, this.firstPassMaterial, 0);
        }

        if (this.displayMaterial)
        {
            this.displayMaterial.SetTexture(this.inputMaterialName, outputRenderTexture);
        }

        this.frameIndex = 0;
    }
	
	// Update is called once per frame
	void OnDisable ()
    {
	    if(this.renderTexture0 !=null)
        {
            this.renderTexture0.Release();
            this.renderTexture1.Release();
            this.renderTexture0 = null;
            this.renderTexture1 = null;
        }
	}
}
