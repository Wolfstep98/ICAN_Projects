
[UnityEngine.ExecuteInEditMode]
public class MousePositionToMaterial : UnityEngine.MonoBehaviour
{
    public string MousePositionPropertyName = "_MousePosition";

    public UnityEngine.Vector4 mousePosition;

    private int mousePositionPropertyId = -1;

    protected void OnEnable()
    {
        this.mousePositionPropertyId = UnityEngine.Shader.PropertyToID(this.MousePositionPropertyName);
    }

	protected void Update ()
    {
        UnityEngine.Vector3 mousePosition = UnityEngine.Input.mousePosition;
        this.mousePosition = mousePosition;
        UnityEngine.Shader.SetGlobalVector(this.mousePositionPropertyId, mousePosition);
	}
}
