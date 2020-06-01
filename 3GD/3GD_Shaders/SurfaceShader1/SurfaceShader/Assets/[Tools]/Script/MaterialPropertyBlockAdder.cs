using UnityEngine;
using System.Collections;

[UnityEngine.ExecuteInEditMode]
public class MaterialPropertyBlockAdder : MonoBehaviour
{
    [UnityEngine.SerializeField]
    public PropertyColor[] PropertyColors = { };

    [UnityEngine.SerializeField]
    public PropertyVector[] PropertyVectors = { };

    [UnityEngine.SerializeField]
    public PropertyMatrix[] PropertyMatrices = { };

    [UnityEngine.SerializeField]
    public PropertyFloat[] PropertyFloats = { };

    [System.NonSerialized]
    UnityEngine.MaterialPropertyBlock materialPropertyBlock;
    
    public void OnEnable ()
    {
        this.Apply();
    }

    public void OnDisable()
    {
        this.Unapply();
    }

    public bool Apply()
    {
        bool continueNextFrame = false;
        UnityEngine.Renderer renderer = this.transform.GetComponent<UnityEngine.Renderer>();
        if (renderer != null)
        {
            if (this.materialPropertyBlock == null)
            {
                this.materialPropertyBlock = new UnityEngine.MaterialPropertyBlock();
            }

            renderer.GetPropertyBlock(this.materialPropertyBlock);
            {
                int propertyCount = this.PropertyColors != null ? this.PropertyColors.Length : 0;
                for (int i = 0; i < propertyCount; ++i)
                {
                    bool needAnimation = this.PropertyColors[i].AddToMaterialPropertyBlock(this.materialPropertyBlock);
                    continueNextFrame |= needAnimation;
                }
            }

            {
                int propertyCount = this.PropertyVectors != null ? this.PropertyVectors.Length : 0;
                for (int i = 0; i < propertyCount; ++i)
                {
                    bool needAnimation = this.PropertyVectors[i].AddToMaterialPropertyBlock(this.materialPropertyBlock);
                    continueNextFrame |= needAnimation;
                }
            }

            {
                int propertyCount = this.PropertyFloats != null ? this.PropertyFloats.Length : 0;
                for (int i = 0; i < propertyCount; ++i)
                {
                    bool needAnimation = this.PropertyFloats[i].AddToMaterialPropertyBlock(this.materialPropertyBlock);
                    continueNextFrame |= needAnimation;
                }
            }

            {
                int propertyCount = this.PropertyMatrices != null ? this.PropertyMatrices.Length : 0;
                for (int i = 0; i < propertyCount; ++i)
                {
                    bool needAnimation = this.PropertyMatrices[i].AddToMaterialPropertyBlock(this.materialPropertyBlock);
                    continueNextFrame |= needAnimation;
                }
            }

            renderer.SetPropertyBlock(this.materialPropertyBlock);
        }

        return continueNextFrame;
    }

    public void Unapply()
    {
        UnityEngine.Renderer renderer = this.transform.GetComponent<UnityEngine.Renderer>();
        if (renderer != null && this.materialPropertyBlock != null)
        {
            this.materialPropertyBlock.Clear();
            renderer.SetPropertyBlock(this.materialPropertyBlock);
            this.materialPropertyBlock = null;
        }
    }

    [System.Serializable]
    public struct PropertyColor
    {
        public string PropertyName;
        public UnityEngine.Color PropertyValue;

        public bool AddToMaterialPropertyBlock(UnityEngine.MaterialPropertyBlock materialPropertyBlock)
        {
            materialPropertyBlock.SetColor(this.PropertyName, this.PropertyValue);
            return false;
        }
    }

    [System.Serializable]
    public struct PropertyVector
    {
        public string PropertyName;
        public UnityEngine.Vector4 PropertyValue;
        public bool AddToMaterialPropertyBlock(UnityEngine.MaterialPropertyBlock materialPropertyBlock)
        {
            materialPropertyBlock.SetVector(this.PropertyName, this.PropertyValue);
            return false;
        }
    }

    [System.Serializable]
    public struct PropertyMatrix
    {
        public string PropertyName;
        public UnityEngine.Matrix4x4 PropertyValue;
        public bool AddToMaterialPropertyBlock(UnityEngine.MaterialPropertyBlock materialPropertyBlock)
        {
            materialPropertyBlock.SetMatrix(this.PropertyName, this.PropertyValue);
            return false;
        }
    }

    [System.Serializable]
    public struct PropertyFloat
    {
        public string PropertyName;
        public float PropertyValue;

        public bool AddToMaterialPropertyBlock(UnityEngine.MaterialPropertyBlock materialPropertyBlock)
        {
            materialPropertyBlock.SetFloat(this.PropertyName, this.PropertyValue);
            return false;
        }
    }
}
