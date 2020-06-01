using System;
using UnityEngine;

namespace Game.UI
{
    [Serializable]
    public class Menu : IDisplayable, IMenu
    {
        #region Fields
        [Header("Data")]
        [SerializeField]
        private MenuType menu = MenuType.None;
        [SerializeField]
        private bool enableVisibilityToggle = true;
        [SerializeField]
        private int menuIndex = -1;
        [SerializeField]
        private int lastMenuIndex = -1;
        [SerializeField]
        private GameObject container = null;
        [SerializeField]
        private GameObject[] items = null;
        #endregion

        #region Constructor
        protected Menu() { }
        #endregion

        #region Properties
        public int MenuIndex { get { return this.menuIndex; } }
        public int LastMenuIndex { get { return this.lastMenuIndex; } set { this.lastMenuIndex = value; } }
        public MenuType MenuType { get { return this.menu; } }
        public GameObject[] Items { get { return this.items; } }
        #endregion

        #region Methods
        public virtual void OnMenuSelected()
        {

        }

        public virtual void SetVisibility(bool visible, bool canvasVisibility = true)
        {
            if (this.enableVisibilityToggle)
                this.container.SetActive(visible);
        }

        public virtual void GoToItem(int index)
        {

        }
        #endregion
    }
}