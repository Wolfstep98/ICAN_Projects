using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[UnityEngine.ExecuteInEditMode]
public class GenericBufferExport<_ValueType> : LoadableBehaviour where _ValueType : struct 
{
	public _ValueType[] Datas;
	public string BufferName = "_Buffer";
    [UnityEngine.Tooltip("When > 0 tell every drawProceduralRender how many vertex to draw per element in Datas")]
    public int VertexPerElement = 0;

	private string bufferSizeName;
	private UnityEngine.ComputeBuffer computeBuffer;
	private static System.Collections.Generic.List<UnityEngine.Renderer> renderers = new System.Collections.Generic.List<Renderer>();
	private static System.Collections.Generic.List<DrawProceduralRenderer> drawProceduralRenderers = new System.Collections.Generic.List<DrawProceduralRenderer>();

	private static UnityEngine.MaterialPropertyBlock materialPropertyBlock;

    public void Apply()
    {
        UnityEngine.Debug.Assert(this.Loaded);
        this.ApplyToRenderers();
        this.ApplyToDrawProceduralRenderer();
    }
	
    protected void OnEnable () 
	{
		this.LoadIFN();		
	}
	
	protected void OnDisable () 
	{
		this.UnloadIFN();
	}

	protected override void Load()
	{
        base.Load();
        this.bufferSizeName = this.BufferName + "Size";
		this.CreateOrUpdateComputeBuffer();
        this.ApplyToDrawProceduralRenderer();
        this.ApplyToRenderers();
    }

	protected override void Unload()
	{
		if (this.computeBuffer != null)
		{
			this.computeBuffer.Release();
		}

		this.computeBuffer = null;
        base.Unload();
	}

	private void ApplyToRenderers()
	{
		if (materialPropertyBlock == null)
		{
			materialPropertyBlock = new UnityEngine.MaterialPropertyBlock();
		}

        int datasLength = this.Datas != null ? this.Datas.Length : 0;
        this.transform.GetComponents<UnityEngine.Renderer>(renderers);
		int renderersCount = renderers.Count;
		for (int i = 0; i < renderersCount; ++i)
		{
			renderers[i].GetPropertyBlock(materialPropertyBlock); 
			materialPropertyBlock.SetBuffer(this.BufferName, this.computeBuffer);
			materialPropertyBlock.SetInt(this.bufferSizeName, datasLength);
			renderers[i].SetPropertyBlock(materialPropertyBlock);
            materialPropertyBlock.Clear();
        }

        renderers.Clear();
    }

	private void ApplyToDrawProceduralRenderer()
	{
        int datasLength = this.Datas != null ? this.Datas.Length : 0;
        int vertexCount = this.VertexPerElement * datasLength;

        this.transform.GetComponents<DrawProceduralRenderer>(drawProceduralRenderers);
		int drawProceduralRendererCount = drawProceduralRenderers.Count;
		for (int i = 0; i < drawProceduralRendererCount; ++i)
		{
			drawProceduralRenderers[i].MaterialPropertyBlock.SetBuffer(this.BufferName, this.computeBuffer);
			drawProceduralRenderers[i].MaterialPropertyBlock.SetInt(this.bufferSizeName, datasLength);
            if (vertexCount > 0)
            {
                drawProceduralRenderers[i].VertexCount = vertexCount;
            }
		}

		drawProceduralRenderers.Clear();
	}

	private void CreateOrUpdateComputeBuffer()
	{
		int datasLength = this.Datas != null ? this.Datas.Length : 0;
		if (this.computeBuffer == null || this.computeBuffer.count < datasLength)
		{
			if (this.computeBuffer != null)
			{
				this.computeBuffer.Release();
				this.computeBuffer = null;
			}

			int sizeofOfStruct = System.Runtime.InteropServices.Marshal.SizeOf(typeof(_ValueType));
        	this.computeBuffer = new UnityEngine.ComputeBuffer(System.Math.Max(1, datasLength), sizeofOfStruct);
		}

        if (this.Datas != null)
        {
            this.computeBuffer.SetData(this.Datas);
        }
	}

	void OnValidate()
	{
		if (this.Loaded)
		{
			this.CreateOrUpdateComputeBuffer();
            this.Apply();
		}
	}
}
