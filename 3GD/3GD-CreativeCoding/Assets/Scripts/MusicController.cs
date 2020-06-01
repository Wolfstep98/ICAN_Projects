using UnityEngine;

namespace Game
{

    public class MusicController : MonoBehaviour
    {
        #region Fields
        [Header("References")]
        [SerializeField] private AudioSource audioSource = null;
        #endregion

        #region Properties
        public AudioSource AudioSource { get { return this.audioSource; } }
        #endregion

        #region Methods
        private void Awake()
        {
            this.audioSource.Play();
        }
        #endregion
    }
}