using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class ValueSwitcher : UnityEngine.MonoBehaviour, IDebugGOMouseMoverEventReceiver
{
    public string propertyName = string.Empty;
    public float fadeIn = 1;
    public float fadeOut = 1;

    public void OnSpace()
    {
        MaterialPropertyBlockAdder materialPropertyBlockAdder = this.transform.GetComponent<MaterialPropertyBlockAdder>();
        for(int i = 0; i < materialPropertyBlockAdder.PropertyVectors.Length; ++i)
        {
            if (materialPropertyBlockAdder.PropertyVectors[i].PropertyName == this.propertyName)
            {
                float unityTime = UnityEngine.Time.time;
                UnityEngine.Vector4 previousValue = materialPropertyBlockAdder.PropertyVectors[i].PropertyValue;
                float lerpFactor = System.Math.Max(0, System.Math.Min(1, (unityTime - previousValue.y) / (previousValue.w - previousValue.y)));
                float currentValue = previousValue.z * lerpFactor + previousValue.x * (1 - lerpFactor);
                float duration = (1 - previousValue.z) > 0.5 ? fadeIn : fadeOut;
                materialPropertyBlockAdder.PropertyVectors[i].PropertyValue = new UnityEngine.Vector4(currentValue, unityTime, 1 - previousValue.z, unityTime + duration * lerpFactor);
            }
        }

        materialPropertyBlockAdder.Apply();
    }
}
