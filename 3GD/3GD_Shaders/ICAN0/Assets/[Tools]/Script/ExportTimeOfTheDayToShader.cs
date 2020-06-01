
[UnityEngine.ExecuteInEditMode]
public class ExportTimeOfTheDayToShader : UnityEngine.MonoBehaviour
{
    [UnityEngine.SerializeField]
    string dayTime = "_DayTime";

    [UnityEngine.SerializeField]
    string unityEngineTime = "_UnityEngineTime";

    void Update ()
    {
        if (this.dayTime != string.Empty)
        {
            // you can export global data that are accessible by all the shaders (except computeShader). It is a handy to provide global state read by many shaders.
            System.DateTime now = System.DateTime.Now;
            float secondsSinceStartOfTheDay = now.Hour * 3600 + now.Minute * 60 + now.Second + now.Millisecond * 0.001f;
            UnityEngine.Shader.SetGlobalVector(this.dayTime, new UnityEngine.Vector4(secondsSinceStartOfTheDay, now.Hour, now.Minute, now.Second));
        }

        if (this.unityEngineTime != string.Empty)
        {
            float t = UnityEngine.Time.time;
            UnityEngine.Shader.SetGlobalVector(this.unityEngineTime, new UnityEngine.Vector4(t / 20f, t, t * 2f, t * 3f));
        }
    }
}
