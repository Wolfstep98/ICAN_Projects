using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsDataExportBuffer : GenericBufferExport<LightsDataExportBuffer.LightData> {
	
	public static LightsDataExportBuffer LightsDataExporter; 
	public List<LightsData> commands;

	private void Awake() {
		LightsDataExporter = this;
	}

	public void Update()
	{
		if (!this.Loaded)
		{
			return;
		}

		var commandsCount = commands?.Count ?? 0;
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
		var commandsCount = commands?.Count ?? 0;
		
        if (this.Datas == null || this.Datas.Length != commandsCount)
        {
            this.Datas = new LightData[commandsCount];
        }

		for (var i = 0; i < commandsCount; ++i)
		{
			LightData lightData;
			lightData.Size = commands[i].gameObject.activeSelf ? commands[i].Size : Vector2.zero;
			lightData.WorldToLocal = commands[i].LightTransform.worldToLocalMatrix;
			this.Datas[i] = lightData;
		}
	}
	
	[System.Serializable]
	public struct LightData {
		public Vector2 Size;
		public Matrix4x4 WorldToLocal;
	}
}
