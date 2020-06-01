using System;
using UnityEngine;

namespace DesignPattern
{

    [Serializable]
    public class Singleton<T> where T : class, new()
    {
        #region Fields
        private static T instance;
        #endregion

        #region Properties
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new T();
                }
                return instance;
            }
        }
        #endregion

        #region Initialization
        private void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
            }
        }
        #endregion
    }
}