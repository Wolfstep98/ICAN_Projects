
[UnityEngine.ExecuteInEditMode]
public class AddWorldToLocalMatrix : UnityEngine.MonoBehaviour
{
    public string propertyName = "_WorldToLocal";
    public UnityEngine.Vector3 scale = UnityEngine.Vector3.one;

    private UnityEngine.Matrix4x4 previousWorldToLocal = UnityEngine.Matrix4x4.identity;
    private UnityEngine.Vector3 previousScale = UnityEngine.Vector3.one;

    public void OnEnable()
    {
        this.previousScale = UnityEngine.Vector3.zero; // to force refresh.
        this.UpdateMatrix();
    }

    public void OnDisable()
    {
        this.UpdateMatrix(true);
    }

    public void Update()
    {
        this.UpdateMatrix();
    }

    private void UpdateMatrix(bool reset = false)
    {
        UnityEngine.Matrix4x4 thisWorldToLocalMatrix = this.transform.worldToLocalMatrix;
        if (reset || this.previousWorldToLocal != thisWorldToLocalMatrix || this.previousScale != this.scale)
        {
            MaterialPropertyBlockAdder materialPropertyBlockAdder = this.transform.GetComponentInParent<MaterialPropertyBlockAdder>();
            if (materialPropertyBlockAdder != null)
            {
                int propertyCount = materialPropertyBlockAdder.PropertyMatrices != null ? materialPropertyBlockAdder.PropertyMatrices.Length : 0;
                bool found = false;
                for (int i = 0; i < propertyCount; ++i)
                {
                    if (materialPropertyBlockAdder.PropertyMatrices[i].PropertyName == this.propertyName)
                    {
                        found = true;
                        // Pour forcer la disparition on fait croire que le decal est infiniment petit et pas au bon endroit.
                        UnityEngine.Matrix4x4 matrixToUSe = reset ? UnityEngine.Matrix4x4.Translate(UnityEngine.Vector3.one * -1) * UnityEngine.Matrix4x4.Scale(UnityEngine.Vector3.zero) : thisWorldToLocalMatrix;
                        materialPropertyBlockAdder.PropertyMatrices[i].PropertyValue = UnityEngine.Matrix4x4.Scale(new UnityEngine.Vector3(1 / this.scale.x, 1 / scale.y, 1 / scale.z)) * matrixToUSe;
                    }
                }

                if (!found)
                {
                    System.Array.Resize(ref materialPropertyBlockAdder.PropertyMatrices, propertyCount + 1);
                    materialPropertyBlockAdder.PropertyMatrices[propertyCount].PropertyName = this.propertyName;
                    materialPropertyBlockAdder.PropertyMatrices[propertyCount].PropertyValue = thisWorldToLocalMatrix;
                }

                materialPropertyBlockAdder.Apply();
            }

            this.previousWorldToLocal = thisWorldToLocalMatrix;
            this.previousScale = this.scale;
        }
    }
}
