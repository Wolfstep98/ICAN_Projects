using System;
using UnityEngine;

namespace DesignPattern
{

    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        #region Fields
        private static T instance;
        #endregion

        #region Properties
        public static T Instance
        {
            get
            {
                //Check if the instance is instantiate
                if (instance == null)
                {
                    //Search for the instance in the scene
                    instance = FindObjectOfType<T>();

                    //If the instance has not been found, create a new one
                    if (instance == null)
                    {
                        GameObject singleton = new GameObject(typeof(T).Name);
                        instance = singleton.AddComponent<T>();
                    }
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
                //DontDestroyOnLoad(instance);
            }
            else if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }
        #endregion
    }
}