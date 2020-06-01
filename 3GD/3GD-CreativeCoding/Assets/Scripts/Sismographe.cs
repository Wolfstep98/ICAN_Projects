using UnityEngine;


namespace Game
{
    public class Sismographe : MonoBehaviour
    {
        #region Fields
        [Header("References")]
        [SerializeField] private EntityPooler entityPooler = null;
        [SerializeField] private MusicController musicController = null;
        [SerializeField] private Transform cameraTransform = null;

        [Header("Behaviour")]
        [SerializeField] private AnimationCurve curve;

        [Header("Data")]
        private float[] initialXPos = null;
        private float timer = 0.0f;
        [SerializeField] private float timerMultiplicator = 1.0f;
        [SerializeField] private float entityYSpeed = 5.0f;
        [SerializeField] private float entityXMultiplicator = 2.0f;
        [SerializeField] private int entityNumber = 10;
        [SerializeField] private float entityGap = 10.0f;
        [SerializeField, Tooltip("Must be power of 2 !")] private int sampleRate = 64;
        [SerializeField] FFTWindow window = FFTWindow.Rectangular;

        private Entity[] entities;
        #endregion

        #region Methods
        #region Initializers
        private void Awake()
        {
            this.Initialize();
        }

        private void Initialize()
        {
            this.timer = 0.0f;

            this.initialXPos = new float[this.entityNumber];
            this.entities = new Entity[this.entityNumber];
            //Instantiate all entities and put them in line
            for(int i = 0; i < this.entityNumber; i++)
            {
                Entity entity = this.entityPooler.GetEntity();
                entity.transform.position = new Vector3(i * this.entityGap, 0.0f, 0.0f);
                this.initialXPos[i] = entity.transform.position.x;
                this.entities[i] = entity;
            }
        }
        #endregion


        private void Update()
        {
            float[] data = new float[this.sampleRate];
            this.musicController.AudioSource.GetSpectrumData(data, 0, this.window);

            for(int i = 0; i < data.Length && i < this.entities.Length; i++)
            {
                float xValue = this.GetCurveData();
                xValue *= data[i];
                xValue *= this.entityXMultiplicator;
                this.entities[i].transform.Translate(new Vector3(0.0f, this.entityYSpeed * Time.deltaTime, 0.0f));
                this.entities[i].transform.position = new Vector3(this.initialXPos[i] + xValue, this.entities[i].transform.position.y, this.entities[i].transform.position.z);
            }

            cameraTransform.transform.Translate(new Vector3(0.0f, this.entityYSpeed * Time.deltaTime, 0.0f));

            this.timer += Time.deltaTime * this.timerMultiplicator;
        }

        private float GetCurveData()
        {
            return this.curve.Evaluate(this.timer % 1.0f);
        }
        #endregion
    }
}