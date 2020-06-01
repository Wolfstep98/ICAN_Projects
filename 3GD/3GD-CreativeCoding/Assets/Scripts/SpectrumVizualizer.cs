using UnityEngine;

[RequireComponent(typeof(AudioListener))]
public class SpectrumVizualizer : MonoBehaviour
{
    [SerializeField] private int sampleRate = 256;
    [SerializeField] FFTWindow window = FFTWindow.Rectangular;

    void Update()
    {
        float[] spectrum = new float[this.sampleRate];

        AudioListener.GetSpectrumData(spectrum, 0, this.window);

        Debug.DrawLine(new Vector3(0, 0, 0), new Vector3(0, 1, 0), Color.black);
        for (int i = 1; i < spectrum.Length - 1; i++)
        {
            Debug.DrawLine(new Vector3(i - 1, spectrum[i], -1), new Vector3(i, spectrum[i + 1], -1), Color.yellow);
            Debug.DrawLine(new Vector3(i - 1, spectrum[i] + 10, 0), new Vector3(i, spectrum[i + 1] + 10, 0), Color.red);
            Debug.DrawLine(new Vector3(i - 1, Mathf.Log(spectrum[i - 1]) + 10, 2), new Vector3(i, Mathf.Log(spectrum[i]) + 10, 2), Color.cyan);
            Debug.DrawLine(new Vector3(Mathf.Log(i - 1), spectrum[i - 1] - 10, 1), new Vector3(Mathf.Log(i), spectrum[i] - 10, 1), Color.green);
            Debug.DrawLine(new Vector3(Mathf.Log(i - 1), Mathf.Log(spectrum[i - 1]), 3), new Vector3(Mathf.Log(i), Mathf.Log(spectrum[i]), 3), Color.blue);
        }
    }
}
