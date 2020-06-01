using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{

    public class AngleController : MonoBehaviour
    {
        #region Fields & Properties
        [Header("References")]
        [SerializeField] private MusicController musicController = null;
        [SerializeField] private EntityPooler entityPooler = null;

        [Header("Data")]
        [SerializeField] private int entityNumber = 10;
        [SerializeField, Tooltip("Must be power of 2 !")] private int sampleRate = 64;
        [SerializeField] private Vector2 defaultVector = Vector2.up;
        [SerializeField] private Vector2 angles = Vector2.zero;
        private Entity[] entities;

        #endregion

        #region Methods
        private void Awake()
        {
            this.Initialize();
        }

        private void Initialize()
        {
            this.entities = new Entity[this.entityNumber];
            for(int i = 0; i < this.entityNumber; i++)
            {
                Entity entity = this.entityPooler.GetEntity();
                entity.transform.position = Vector3.zero;
                this.entities[i] = entity;
            }
        }

        private void Update()
        {
            float[] data = new float[sampleRate];
            this.musicController.AudioSource.GetSpectrumData(data, 0, FFTWindow.Rectangular);
            for(int i = 0; i < this.entityNumber; i++)
            {
                Quaternion quaternion = Quaternion.Euler(0.0f, 0.0f, Mathf.Lerp(this.angles.x, this.angles.y, Mathf.Clamp01(data[i] * 1.5f)));
                Vector2 dir = quaternion * this.defaultVector;
                this.entities[i].transform.Translate(dir.normalized * Time.deltaTime);
            }
        }
        #endregion
    }
}