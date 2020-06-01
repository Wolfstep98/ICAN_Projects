using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[UnityEditor.CustomEditor(typeof(Texture2DArrayDescriptor), true)]
[UnityEditor.CustomPreview(typeof(Texture2DArrayDescriptor))]
public class Texture2DArrayDescriptorCustomInspector : UnityEditor.Editor
{
    private bool needAssetDatabaseRefresh;
    private System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<Texture2DArrayDescriptor, string> > needReload = new System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<Texture2DArrayDescriptor, string> >();
    
    public override void OnInspectorGUI()
	{
        this.serializedObject.UpdateIfRequiredOrScript();
		this.DrawDefaultInspector();
		this.serializedObject.ApplyModifiedProperties();

        if (UnityEngine.GUILayout.Button("Capture"))
		{
			this.CreateAsset(this.target as Texture2DArrayDescriptor);
		}

        if (this.needAssetDatabaseRefresh)
		{
			UnityEditor.AssetDatabase.Refresh();
			this.needAssetDatabaseRefresh = false;
		}

		int needReloadCount = this.needReload.Count;
		if (needReloadCount > 0)
		{
			for (int i = 0; i < needReloadCount; ++i)
			{
				this.needReload[i].Key.Texture2DArray = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Texture2DArray>(this.needReload[i].Value);
			}

			this.needReload.Clear();
		}
    }

    public void CreateAsset(Texture2DArrayDescriptor descriptor)
    {
        Texture2D[] textures = descriptor.Sources;

        Texture2DArray array = new Texture2DArray(textures[0].width, textures[0].height, textures.Length, descriptor.TextureFormat, true);
        for (int i = 0; i < textures.Length; i++)
        {
            UnityEngine.Graphics.CopyTexture(textures[i], 0, array, i);
        }

        // to set the isReadable to false;
        array.Apply(false, true);
        string textureArrayPath = null;
        if (descriptor.Texture2DArray == null)
        {
            string descriptorPath = UnityEditor.AssetDatabase.GetAssetPath(descriptor);
            textureArrayPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(descriptorPath), 
            System.IO.Path.GetFileNameWithoutExtension(descriptorPath) + "TA.asset");
        }
        else
        {
            textureArrayPath = UnityEditor.AssetDatabase.GetAssetPath(descriptor.Texture2DArray);
        }

        
        UnityEditor.AssetDatabase.CreateAsset(array, textureArrayPath);
        this.needReload.Add(new System.Collections.Generic.KeyValuePair<Texture2DArrayDescriptor, string>(descriptor, textureArrayPath));
        this.needAssetDatabaseRefresh = true;
    }
}
