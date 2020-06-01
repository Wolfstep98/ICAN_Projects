using System;
using UnityEngine;

namespace Game.UI
{
    [Serializable]
    public class MenuContainer
    {
        #region Fields
        [Header("Data")]
        [SerializeField]
        private MenuType menu = MenuType.None;

        [SerializeField]
        private int lastMenuIndex = -1;

        [SerializeField]
        private GameObject container = null;

        //[SerializeField]
        //private GameObject[] items = null;
        #endregion

        #region Constructors
        public MenuContainer() : this(MenuType.None)
        { }
        public MenuContainer(MenuType menu) : this(menu, -1)
        { }
        public MenuContainer(MenuType menu, int lastMenuIndex) : this(menu, lastMenuIndex, null)
        { }
        public MenuContainer(MenuType menu, int lastMenuIndex, GameObject container) : this(menu, lastMenuIndex, container, null)
        { }
        public MenuContainer(MenuType menu, int lastMenuIndex, GameObject container, GameObject[] items)
        {
            this.menu = menu;
            this.lastMenuIndex = lastMenuIndex;
            this.container = container;
            //this.items = items;
        }
        #endregion

        #region Properties
        public MenuType Menu { get { return this.menu; } }

        public int LastMenuIndex { get { return this.lastMenuIndex; } set { this.lastMenuIndex = value; } }

        public GameObject Container { get { return this.container; } }

        //public GameObject[] Items { get { return this.items; } }
        #endregion
    }
}