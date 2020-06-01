using UnityEngine;

namespace Game
{

    public class PlayerInput : MonoBehaviour
    {
        #region Enum
        public enum DataType
        {
            DirNoInterval,
            DirInterval
        }
        #endregion

        #region Fields
        [Header("References")]
        [SerializeField] private MusicController musicController = null;
        [SerializeField] private PlayerController[] playerControllers = null;

        [SerializeField] private DataType dataType = DataType.DirInterval;

        [SerializeField] private int inputData = 4;
        [SerializeField, Tooltip("Must be power of 2 !")] private int sampleRate = 64;
        private int interval = 0;
        private int currentInterval = 0;
        [SerializeField] private int offset = 0;
        #endregion

        #region Methods
        public void CustomAwake()
        {
            this.interval = this.sampleRate / 4;
            this.currentInterval = this.interval - offset;
        }

        public void CustomUpdate()
        {
            float[] samples = new float[this.sampleRate];
            this.musicController.AudioSource.GetSpectrumData(samples, 0, FFTWindow.Rectangular);
            for (int i = 0; i < this.playerControllers.Length; i++)
            {
                PlayerController controller = this.playerControllers[i];
                Vector4 directions = this.ConstructDirectionsData(samples, i);
                controller.SetDirection(directions);
                controller.Move();
            }
        }

        private Vector4 ConstructDirectionsData(float[] samples, int index)
        {
            Vector4 data = new Vector4();

            switch (this.dataType)
            {
                case DataType.DirInterval:
                    // 4 directions : Right, Down, Left, Up with an interval between all 4 values
                    for (int i = index, dataIndex = 0; i < samples.Length && dataIndex < this.inputData; i += this.currentInterval, dataIndex++)
                    {
                        data[dataIndex] = samples[i];
                    }
                    break;
                case DataType.DirNoInterval:
                    // 4 directions : Right, Down, Left, Up with no interval between all 4 values
                    for (int i = 4 * index, dataIndex = 0; i < samples.Length && dataIndex < this.inputData; i++, dataIndex++)
                    {
                        data[dataIndex] = samples[i];
                    }
                    break;
            }    
            return data;
        }
        #endregion
    }
}