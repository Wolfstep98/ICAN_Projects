using UnityEngine;

namespace Game
{

    public class PlayerManager : MonoBehaviour
    {
        #region Fields
        [Header("References")]
        [SerializeField] private PlayerInput input = null;
        [SerializeField] private PlayerController[] controllers = null;
        #endregion

        #region Methods
        #region Init
        private void Awake()
        {
            // Input
            this.input.CustomAwake();

            // Controller
            foreach(PlayerController controller in controllers)
            {
                controller.CustomAwake();
            }
        }
        #endregion

        private void Update()
        {
            // Input
            this.input.CustomUpdate();

            // Controller
            foreach (PlayerController controller in controllers)
            {
                controller.CustomUpdate();
            }
        }
        #endregion
    }
}