

[UnityEngine.CreateAssetMenu()]
public class RenderCheck : UnityEngine.ScriptableObject 
{
	public UnityEngine.Vector2Int Size = new UnityEngine.Vector2Int(512, 512);
	public UnityEngine.Material Material;
	public UnityEngine.Texture2D Match;
	public UnityEngine.Material DiffMaterial;
}
