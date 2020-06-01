using UnityEngine;

namespace Game.Entities.Crosshair
{
    /// <summary>
    /// Class to control the game cursor.
    /// </summary>
    public class Crosshair : GameBehavior
    {
        #region Fields
        [SerializeField] private new Camera camera = null;
        #endregion

        #region Methods

#if UNITY_EDITOR
        #region Check for null reference
        private void Awake()
        {
            if (this.camera == null)
                Debug.LogError("[Null Reference] - camera are not properly set !");
        }
        #endregion
#endif

        #region Initialization
        public override void CustomAwake()
        {
            //Initialize Cursor
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
        }
        #endregion

        #region Updates
        public override void CustomUpdate()
        {
            this.UpdatePosition();
        }
        #endregion

        #region Movement
        private void UpdatePosition()
        {
            var mousePos = this.camera.ScreenToWorldPoint(Input.mousePosition);
            this.transform.position = new Vector3(mousePos.x, mousePos.y, 0);
        }
        #endregion
        #endregion
    }
}
