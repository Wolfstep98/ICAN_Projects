using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldToLocalExportBuffer : GenericBufferExport<WorldToLocalData> 
{
	public UnityEngine.GameObject commands;

	public void Update()
	{
		if (!this.Loaded)
		{
			return;
		}

		int commandsCount = (this.commands != null) ? commands.transform.childCount : 0;
		if (commandsCount != this.Datas.Length)
		{
			this.InitDatas();
			this.CreateOrUpdateComputeBuffer();
			this.Apply();
		}
		else
		{
			InitDatas();
			ApplyData();
		}
	}

	protected override void Load()
	{
		this.InitDatas();
		base.Load();
	}

	private void InitDatas()
	{
		int commandsCount = (this.commands != null) ? commands.transform.childCount : 0;
		
        if (this.Datas == null || this.Datas.Length != commandsCount)
        {
            this.Datas = new WorldToLocalData[commandsCount];
        }

		for (int i = 0; i < commandsCount; ++i)
		{
			WorldToLocalData vertexData;
			vertexData.WorldToLocal = commands.transform.GetChild(i).worldToLocalMatrix;
			this.Datas[i] = vertexData;
		}
	}
}

[System.Serializable]
public struct WorldToLocalData
{
	public UnityEngine.Matrix4x4 WorldToLocal;
}
