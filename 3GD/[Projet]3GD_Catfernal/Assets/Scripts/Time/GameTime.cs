using System;
using UnityEngine;

namespace Game
{
    public class GameTime : MonoBehaviour
    {
        #region Fields & Properties
        [Header("Time")]
        [SerializeField] private float Time = 0.0f;
        [SerializeField] private float DeltaTime = 0.0f;
        [SerializeField] private float TimeScale = 1.0f;

        //Static Fields & Properties
        private static float _time = 0.0f;
        public static float time { get { return _time; } }

        private static float _deltaTime = 0.0f;
        public static float deltaTime { get { return _deltaTime; } }

        private static float _timeScale;
        public static float timeScale { get { return _timeScale; } set { _timeScale = value; } }
        #endregion

        #region Methods
        #region Constructors
        static GameTime()
        {
            _time = 0.0f;
            _deltaTime = 0.0f;
            _timeScale = 1.0f;
        }
        #endregion

        private void Awake()
        {
            this.UpdateTimes();
        }

        private void LateUpdate()
        {
            _deltaTime = UnityEngine.Time.deltaTime * _timeScale;
            _time += _deltaTime;
            this.UpdateTimes();
        }

        private void UpdateTimes()
        {
            this.Time = _time;
            this.DeltaTime = _deltaTime;
            this.TimeScale = _timeScale;

            //if (Math.Abs(this.TimeScale - _timeScale) > 0.00001f)
            //    _timeScale = this.TimeScale;
            //else
            //    this.TimeScale = _timeScale;
        }
        #endregion
    }
}
