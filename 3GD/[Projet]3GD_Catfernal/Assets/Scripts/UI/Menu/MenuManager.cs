using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class MenuManager : MonoBehaviour
    {
        #region Fields
        [Header("Data")]
        [SerializeField]
        private int firstMenuIndex = 0;
        [SerializeField]
        private int currentMenuIndex = 0;
        [Header("Parameters")]
        [SerializeField]
        private MenuContainer[] menus = null;
        #endregion

        #region Methods
        #region Initialization
        private void Awake()
        {
            this.Initialize();
        }

        private void Initialize()
        {
#if UNITY_EDITOR
            if (this.menus == null)
                Debug.LogError("[Missing Reference] - menus are missing !");
#endif

            this.currentMenuIndex = this.firstMenuIndex;
            for (int i = 0; i < this.menus.Length; i++)
            {
                this.SetMenuVisibility(i, (this.currentMenuIndex == i));
            }
        }
        #endregion

        /// <summary>
        /// Got to the menu with index <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index of the menu.</param>
        public void GoToMenu(int index)
        {
            this.SetMenuVisibility(this.currentMenuIndex, false);

            this.menus[index].LastMenuIndex = this.currentMenuIndex;
            this.currentMenuIndex = index;

            this.SetMenuVisibility(index, true);
        }

        /// <summary>
        /// Go back to the previous menu selected.
        /// </summary>
        public void GoToPreviousMenu()
        {
            if (this.menus[this.currentMenuIndex].LastMenuIndex == -1)
                return;

            int index = this.menus[this.currentMenuIndex].LastMenuIndex;

            this.GoToMenu(index);
        }

        /// <summary>
        /// Set the visibility of the menu.
        /// </summary>
        /// <param name="index">The index of the menu.</param>
        /// <param name="visible">The new visibility of the menu.</param>
        private void SetMenuVisibility(int index, bool visible)
        {
            this.menus[index].Container.SetActive(visible);
        }
        #endregion
    }
}