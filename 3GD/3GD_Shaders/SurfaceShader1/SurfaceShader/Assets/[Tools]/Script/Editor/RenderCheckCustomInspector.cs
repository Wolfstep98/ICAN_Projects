[UnityEditor.CanEditMultipleObjects]
[UnityEditor.CustomEditor(typeof(RenderCheck), true)]
[UnityEditor.CustomPreview(typeof(RenderCheck))]
public class RenderCheckInspector : UnityEditor.Editor
{
	bool needRefresh;
	System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<RenderCheck, string> > needReload = new System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<RenderCheck, string> >();
	public override bool RequiresConstantRepaint()
	{
		return true;
	}

	public override void OnInspectorGUI()
	{
		this.serializedObject.UpdateIfRequiredOrScript();
		this.DrawDefaultInspector();
		this.serializedObject.ApplyModifiedProperties();

		if (UnityEngine.GUILayout.Button("Capture"))
		{
			this.DoGUI(UnityEngine.Rect.zero, this.target as RenderCheck, true, false);
		}

		if (this.needRefresh)
		{
			UnityEditor.AssetDatabase.Refresh();
			this.needRefresh = false;
		}

		int needReloadCount = this.needReload.Count;
		if (needReloadCount > 0)
		{
			for (int i = 0; i < needReloadCount; ++i)
			{
				this.needReload[i].Key.Match = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Texture2D>(this.needReload[i].Value);
			}

			this.needReload.Clear();
		}
	}

	public override bool HasPreviewGUI()
	{
		return true;
	}

	public void OnDestroy()
	{
	}

	public override void OnInteractivePreviewGUI(UnityEngine.Rect r, UnityEngine.GUIStyle background)
	{
		RenderCheck renderCheck = this.target as RenderCheck;
		
		if (UnityEngine.Event.current.type == UnityEngine.EventType.Repaint && 
		renderCheck && 
		renderCheck.Material != null &&
		renderCheck.Size.x > 0 &&
		renderCheck.Size.y > 0 &&
		renderCheck.Size.x <= 4096 &&
		renderCheck.Size.y <= 4096)
		{
			DoGUI(r, renderCheck, false, true);
		}
	}

	private void DoGUI(UnityEngine.Rect r, RenderCheck renderCheck, bool capture, bool doGui)
	{
		UnityEngine.RenderTexture rt = UnityEngine.RenderTexture.GetTemporary(renderCheck.Size.x, renderCheck.Size.y, 0, UnityEngine.RenderTextureFormat.Default, UnityEngine.RenderTextureReadWrite.sRGB);
		UnityEngine.RenderTexture previousRT = UnityEngine.RenderTexture.active;
		UnityEngine.RenderTexture.active = rt;
		UnityEngine.GL.PushMatrix();
		UnityEngine.GL.LoadPixelMatrix(0, rt.width, rt.height, 0);

		UnityEngine.Rect src = new UnityEngine.Rect(0, 0, 1, 1);
		UnityEngine.Rect dest = new UnityEngine.Rect(0, 0, rt.width, rt.height);
		
		UnityEngine.Texture2D textureToUse = UnityEngine.Texture2D.whiteTexture;
		UnityEngine.Graphics.DrawTexture(dest, textureToUse, src, 0, 0, 0, 0, renderCheck.Material);

		if (capture)
		{
			this.SaveRenderTextureToPNG(rt, UnityEngine.TextureFormat.ARGB32, renderCheck);
			UnityEditor.EditorUtility.SetDirty(renderCheck);
		}

		UnityEngine.GL.PopMatrix();
		UnityEngine.RenderTexture.active = previousRT;

		if (doGui)
		{
			UnityEngine.Rect rectRT = r;
			UnityEngine.Rect rectMatch = r;
			UnityEngine.Rect rectDiff = r;
			rectRT.width = rectRT.height;
			rectMatch.width = rectRT.height;
			rectDiff.width = rectRT.height;
			rectMatch.x = rectRT.xMax + 4;
			rectDiff.x = rectMatch.xMax + 4;
			UnityEngine.GUI.Label(rectRT, rt);
			UnityEngine.GUI.Label(rectRT, "rt");
			if (renderCheck.Match != null)
			{
				UnityEngine.GUI.Label(rectMatch, renderCheck.Match);
				UnityEngine.GUI.Label(rectMatch, "ref");
				if (renderCheck.DiffMaterial != null)
				{
					renderCheck.DiffMaterial.SetTexture("_MatchTex", renderCheck.Match);
					UnityEditor.EditorGUI.DrawPreviewTexture(rectDiff, rt, renderCheck.DiffMaterial);
					renderCheck.DiffMaterial.SetTexture("_MatchTex", null);
					UnityEngine.GUI.Label(rectDiff, "diff");
				}
			}
		}

		UnityEngine.RenderTexture.ReleaseTemporary(rt);
		rt = null;
	}

	private void SaveRenderTextureToPNG(
		UnityEngine.RenderTexture renderTexture, 
		UnityEngine.TextureFormat textureFormat, 
		RenderCheck renderCheck)
	{
		string fileName = string.Empty;
		if (renderCheck.Match != null)
		{
			fileName = UnityEditor.AssetDatabase.GetAssetPath(renderCheck.Match);
		}
		else
		{
			fileName = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(UnityEditor.AssetDatabase.GetAssetPath(renderCheck));
		}

		fileName = System.IO.Path.GetDirectoryName(fileName) + "/" + System.IO.Path.GetFileNameWithoutExtension(fileName) + ".png";

		if (renderCheck.Match != null && (renderCheck.Match.width != renderTexture.width || renderCheck.Match.height != renderTexture.height))
		{
			UnityEngine.GameObject.Destroy(renderCheck.Match);
		}

		if (renderCheck.Match == null)
		{
			renderCheck.Match = new UnityEngine.Texture2D(renderTexture.width, renderTexture.height, textureFormat, false, true);
		}

		UnityEngine.RenderTexture previousRT = UnityEngine.RenderTexture.active;
		UnityEngine.RenderTexture.active = renderTexture;
		UnityEngine.Texture2D linearTexture = new UnityEngine.Texture2D(renderTexture.width, renderTexture.height, UnityEngine.TextureFormat.RGBAFloat, false);
		linearTexture.ReadPixels(new UnityEngine.Rect(0.0f, 0.0f, (float)renderTexture.width, (float)renderTexture.height), 0, 0, false);
		//linearTexture.Apply();
        UnityEngine.Color[] colors = linearTexture.GetPixels();
        UnityEngine.Color32[] outputColor = renderCheck.Match.GetPixels32();
        int pixelCount = colors.Length;
        for (int i = 0; i < pixelCount; ++i)
        {
            outputColor[i] = colors[i].linear;
        }

        renderCheck.Match.SetPixels32(linearTexture.GetPixels32());
		UnityEngine.GameObject.DestroyImmediate(linearTexture);
		linearTexture = null;
		UnityEngine.RenderTexture.active = previousRT;

		renderCheck.Match.Apply(false, false);

		byte[] pngFile = UnityEngine.ImageConversion.EncodeToPNG(renderCheck.Match);

		try
		{
			using (System.IO.Stream fileStream = new System.IO.FileStream(fileName, System.IO.FileMode.Create))
			{
				System.IO.BinaryWriter streamWriter = new System.IO.BinaryWriter(fileStream);
				streamWriter.Write(pngFile);
				this.needRefresh = true;
				this.needReload.Add(new System.Collections.Generic.KeyValuePair<RenderCheck, string>(renderCheck, fileName));
			}
		}
		catch (System.Exception exception)
		{
			UnityEngine.Debug.LogError(string.Format("Saving texture at path {0} failed with exception {1}", fileName, exception.Message));
		}
	}
}