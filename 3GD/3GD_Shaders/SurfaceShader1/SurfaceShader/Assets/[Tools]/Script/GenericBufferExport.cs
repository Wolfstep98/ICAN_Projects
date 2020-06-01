using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[UnityEngine.ExecuteInEditMode]
public class GenericBufferExport<_ValueType> : LoadableBehaviour where _ValueType : struct 
{
	public _ValueType[] Datas;
	public string BufferName = "_Buffer";
    [UnityEngine.Tooltip("When > 0 tells every drawProceduralRender how many vertex to draw per element in Datas")]
    public int VertexPerElement = 0;

	public UnityEngine.GameObject dest;
	public bool exportToGlobal = false;
	public bool exportToLocal = false;

	private string bufferSizeName;
	private UnityEngine.ComputeBuffer computeBuffer;
	private static System.Collections.Generic.List<UnityEngine.Renderer> renderers = new System.Collections.Generic.List<Renderer>();
	private static System.Collections.Generic.List<DrawProceduralRenderer> drawProceduralRenderers = new System.Collections.Generic.List<DrawProceduralRenderer>();

	private static UnityEngine.MaterialPropertyBlock materialPropertyBlock;

    public void Apply()
    {
        UnityEngine.Debug.Assert(this.Loaded);
		this.CreateOrUpdateComputeBuffer();
		UnityEngine.Debug.Assert(this.computeBuffer != null);
		UnityEngine.Debug.Assert(this.computeBuffer.count >= this.Datas.Length);
		this.computeBuffer.SetData(this.Datas);
		if (this.exportToLocal)
		{
			this.ApplyToRenderers();
        	this.ApplyToDrawProceduralRenderer();	
		}
        
		if (this.exportToGlobal)
		{
			this.ApplyToGlobal();
		}
    }
	
    protected void OnEnable () 
	{
		this.LoadIFN();		
	}
	
	protected void OnDisable () 
	{
		this.UnloadIFN();
	}

	protected void ApplyData()
	{
		this.computeBuffer.SetData(this.Datas);
	}

	protected override void Load()
	{
        base.Load();
        this.bufferSizeName = this.BufferName + "Size";
        this.Apply();
    }

	protected override void Unload()
	{
		if (this.computeBuffer != null)
		{
			this.computeBuffer.Release();
		}

		this.computeBuffer = null;
		if (this.exportToGlobal)
		{
			this.ApplyToGlobal();
		}

        base.Unload();
	}

	protected virtual void FindRenderers(System.Collections.Generic.List<UnityEngine.Renderer> renderersToFill)
	{
		UnityEngine.Transform transformToUse = this.dest != null ? this.dest.transform : this.transform;
		transformToUse.GetComponents<UnityEngine.Renderer>(renderersToFill);
	}

	protected virtual void FindProceduralRenderer(System.Collections.Generic.List<DrawProceduralRenderer> renderersToFill)
	{
		UnityEngine.Transform transformToUse = this.dest != null ? this.dest.transform : this.transform;
		transformToUse.GetComponents<DrawProceduralRenderer>(renderersToFill);
	}

	protected void ApplyToRenderers()
	{
		UnityEngine.Debug.Assert(this.exportToLocal);
		if (materialPropertyBlock == null)
		{
			materialPropertyBlock = new UnityEngine.MaterialPropertyBlock();
		}

        int datasLength = this.Datas != null ? this.Datas.Length : 0;
        this.FindRenderers(renderers);
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

	protected void ApplyToDrawProceduralRenderer()
	{
		UnityEngine.Debug.Assert(this.exportToLocal);
        int datasLength = this.Datas != null ? this.Datas.Length : 0;
        int vertexCount = this.VertexPerElement * datasLength;

        this.FindProceduralRenderer(drawProceduralRenderers);
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

	protected void ApplyToGlobal()
	{
		UnityEngine.Debug.Assert(this.exportToGlobal);
		if (this.computeBuffer != null)
		{
			int datasLength = this.Datas != null ? this.Datas.Length : 0;
			UnityEngine.Shader.SetGlobalBuffer(this.BufferName, this.computeBuffer);
			UnityEngine.Shader.SetGlobalInt(this.bufferSizeName, datasLength);
		}
		else
		{
			UnityEngine.Shader.SetGlobalBuffer(this.BufferName, null);
			UnityEngine.Shader.SetGlobalInt(this.bufferSizeName, 0);
		}
	}

	protected bool CreateOrUpdateComputeBuffer()
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
			return true;
		}

		return false;
	}

	void OnValidate()
	{
		if (this.Loaded)
		{
            this.Apply();
		}
	}
}
