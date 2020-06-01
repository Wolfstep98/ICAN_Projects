using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MeshExportBuffer : GenericBufferExport<VertexData> 
{
	public UnityEngine.Mesh mesh;

	protected override void Load()
	{
		uint indexCount = this.mesh.GetIndexCount(0);
		this.Datas = new VertexData[indexCount];
		UnityEngine.Vector3[] vertices = this.mesh.vertices;
		UnityEngine.Vector2[] uvs = this.mesh.uv;
		int[] indices = this.mesh.GetIndices(0);
		for (uint i = 0; i < indexCount; ++i)
		{
			VertexData vertexData;
			vertexData.position = vertices[indices[i]];
			vertexData.uv = uvs[indices[i]];
			this.Datas[i] = vertexData;
		}

		base.Load();
	}
}

[System.Serializable]
public struct VertexData
{
	public UnityEngine.Vector3 position;
	public UnityEngine.Vector2 uv;
}
