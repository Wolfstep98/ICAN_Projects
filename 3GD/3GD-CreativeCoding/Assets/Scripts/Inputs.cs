using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Inputs : MonoBehaviour
    {
        #region Fields
        [Header("References")]
        [SerializeField] private EntityPooler entityPooler = null;
        [SerializeField] private MusicController musicController = null;
        [SerializeField] private Transform cameraTransform = null;
        [SerializeField] private CameraShake cameraShake = null;
        [SerializeField] private new Camera camera = null;

        [Header("Data")]
        [SerializeField] private Entity.InputType inputType = Entity.InputType.Sismology;
        private float timer = 0.0f;
        [SerializeField] private float timerMultiplicator = 1.0f;
        [SerializeField] private float cameraYSpeed = 1.0f;
        [SerializeField] private int entityNumber = 10;
        [SerializeField] private float entityGap = 10.0f;
        [SerializeField, Tooltip("Must be power of 2 !")] private int sampleRate = 64;
        [SerializeField] FFTWindow window = FFTWindow.Rectangular;

        [Header("MultiDirection")]
        [SerializeField] private float cameraRotation = 0.0f;
        [SerializeField] private float cameraSpeed = 0.0f;
        [SerializeField] private int inputData = 4;
        [SerializeField] private int offset = 0;
        [SerializeField] private PlayerInput.DataType dataType = PlayerInput.DataType.DirInterval;
        private int interval = 0;
        private int currentInterval = 0;
        private float soundLenght = 0.0f;
        private float soundCurrentTime = 0.0f;

        [Header("Color")]
        [SerializeField] private int spawnNumber = 100;
        [SerializeField] private float spawnRate = 1.0f;
        [SerializeField] List<Entity> entitiesSpawned = null;
        private int currentSpawnedEntity = 0;

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

            switch (this.inputType)
            {
                case Entity.InputType.Sismology:
                    this.entities = new Entity[this.entityNumber];
                    //Instantiate all entities and put them in line
                    for (int i = 0; i < this.entityNumber; i++)
                    {
                        Entity entity = this.entityPooler.GetEntity();
                        entity.transform.position = new Vector3(i * this.entityGap, 0.0f, 0.0f);
                        this.entities[i] = entity;
                        this.entities[i].Initialize(this.inputType);
                    }
                    break;
                case Entity.InputType.Color:
                    entitiesSpawned = new List<Entity>();
                    break;
                case Entity.InputType.MultiDirections:
                    this.soundLenght = this.musicController.AudioSource.clip.length;
                    this.camera.clearFlags = CameraClearFlags.Nothing;
                    this.cameraShake.transform.position = new Vector3(0.0f, 0.0f, -30.0f);
                    this.interval = this.sampleRate / 4;
                    this.currentInterval = this.interval - offset;
                    this.entities = new Entity[this.sampleRate];

                    //Find unique seed for a sound
                    int seed = this.UniqueSeedPerSound(this.musicController.AudioSource.clip.name, this.musicController.AudioSource.clip.length);
                    Random.InitState(seed);
                    Debug.Log("Seed : " + seed + " 0 : " + Random.value);
                    //Instantiate all entities and put them in line
                    for (int i = 0; i < this.sampleRate; i++)
                    {
                        Entity entity = this.entityPooler.GetEntity();
                        float randomValue = Random.Range(0.5f,1.5f);
                        float randomPosition = Random.Range(0.0f, 0.0f);
                        float randomSmallValue = Random.Range(-1.0f, 1.0f);
                        float randomBigValue = Random.Range(-10.0f, 10.0f);
                        entity.Speed *= randomValue;
                        entity.RotationMultiplictor *= randomSmallValue;
                        entity.transform.position = (Vector3)Random.insideUnitCircle * randomPosition; //* randomBigValue; //Random.insideUnitSphere * randomBigValue; //
                        entity.transform.rotation =  Quaternion.Euler(new Vector3(0.0f, 0.0f, Mathf.Lerp(-180.0f, 180.0f, Random.value))); //Random.rotation; //Random rotation at spawn
                        //entity.MainDirection = entity.transform.up;
                        this.entities[i] = entity;
                        this.entities[i].Initialize(this.inputType);
                        this.entities[i].SetCurentMusicPercent(0.0f);
                        this.entities[i].SetupMultiDir(new int[4] { Random.Range(0, 4), Random.Range(0, 4), Random.Range(0, 4), Random.Range(0, 4) });
                    }
                    break;
            }
        }

        private int UniqueSeedPerSound(string soundName, float soundLenght)
        {
            int result = 0;
            for(int i = 0; i < soundName.Length; i++)
            {
                int value = soundName[i] - ' ';
                result += value;
            }
            return result;
        }
        #endregion


        private void Update()
        {
            switch (this.inputType)
            {
                case Entity.InputType.Sismology:
                    this.SismologyInput();
                    break;
                case Entity.InputType.Color:
                    this.ColorInput();
                    break;
                case Entity.InputType.MultiDirections:
                    this.MultiDirectionInput();
                    break;
            }
        }

        private void SismologyInput()
        {
            float[] data = new float[this.sampleRate];
            this.musicController.AudioSource.GetSpectrumData(data, 0, this.window);

            float sum = data[0];
            for (int i = 0; i < data.Length && i < this.entities.Length; i++)
            {
                this.entities[i].ParseData(new float[1] { data[i] });
                //sum += data[i];
                this.entities[i].CustomUpdate();
            }
            //sum /= data.Length;
            //sum /= 2;
            this.cameraShake.ShakeCamera(sum);

            this.cameraShake.transform.Translate(new Vector3(0.0f, this.cameraYSpeed * Time.deltaTime, 0.0f));

            this.timer += Time.deltaTime * this.timerMultiplicator;
        }

        private void ColorInput()
        {
            if(this.timer >= this.spawnRate)
            {
                this.timer = 0.0f;

                Vector3 position = new Vector3(this.currentSpawnedEntity * this.entityGap, 0.0f, 0.0f);
                float[] data = new float[this.sampleRate];
                this.musicController.AudioSource.GetSpectrumData(data, 0, this.window);
                this.InstantiateEntity(position, data);
                this.currentSpawnedEntity++;
            }

            for(int i = 0; i < this.entitiesSpawned.Count; i++)
            {
                this.entitiesSpawned[i].CustomUpdate();
            }

            this.timer += Time.deltaTime * this.timerMultiplicator;
        }

        private void MultiDirectionInput()
        {
            float[] samples = new float[this.sampleRate];
            this.musicController.AudioSource.GetSpectrumData(samples, 0, FFTWindow.Rectangular);
            float highest = this.FindHighestSample(samples);
            Entity.SetHighestColor(highest);
            Entity.MultiDirectionColor();
            this.soundCurrentTime += Time.deltaTime;
            float currentMusicPercent = this.soundCurrentTime / this.soundLenght;

            for (int i = 0; i < this.entities.Length; i++)
            {
                Entity entity = this.entities[i];
                Vector4 directions = this.ConstructDirectionsData(samples, i);
                entity.SetDirection(directions);
                entity.SetCurentMusicPercent(currentMusicPercent);
                entity.CustomUpdate();
            }

            this.cameraShake.ShakeCamera(highest);

            this.cameraTransform.Rotate(Vector3.forward, this.cameraRotation * highest * Time.deltaTime);
            this.cameraShake.transform.Translate(Vector3.back * (1.0f - highest) * this.cameraSpeed * Time.deltaTime, Space.World);
        }

        private Vector4 ConstructDirectionsData(float[] samples, int index)
        {
            Vector4 data = new Vector4();

            switch (this.dataType)
            {
                case PlayerInput.DataType.DirInterval:
                    // 4 directions : Right, Down, Left, Up with an interval between all 4 values
                    for (int i = index, dataIndex = 0; i < samples.Length && dataIndex < this.inputData; i += this.currentInterval, dataIndex++)
                    {
                        data[dataIndex] = samples[i];
                    }
                    break;
                case PlayerInput.DataType.DirNoInterval:
                    // 4 directions : Right, Down, Left, Up with no interval between all 4 values
                    for (int i = 4 * index, dataIndex = 0; i < samples.Length && dataIndex < this.inputData; i++, dataIndex++)
                    {
                        data[dataIndex] = samples[i];
                    }
                    break;
            }
            return data;
        }

        private void InstantiateEntity(Vector3 position, float[] data)
        {
            Entity entity = this.entityPooler.GetEntity();
            entity.transform.position = position;
            entity.ParseData(data);
            entity.Initialize(this.inputType);
            this.entitiesSpawned.Add(entity);
        }
        
        private float FindHighestSample(float[] samples)
        {
            float highestNumber = samples[0];
            for(int i = 0; i < samples.Length; i++)
            {
                if (highestNumber < samples[i])
                    highestNumber = samples[i];
            }
            return highestNumber;
        }
        #endregion

    }
}