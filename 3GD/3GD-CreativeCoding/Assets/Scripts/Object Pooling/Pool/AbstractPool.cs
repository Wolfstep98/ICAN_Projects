using System;
using UnityEngine;

/// <summary>
/// Abstract pool template. Inherits from this class to create a pooler for a specific object.
/// </summary>
/// <typeparam name="T">T must inherits from MonoBehaviour and implements IEntity interface.</typeparam>
[Serializable]
public abstract class AbstractPool<T> : MonoBehaviour where T : MonoBehaviour, IEntity
{
    #region Fields
    [Header("Parameters")]
    [SerializeField]
    private int entityNumber = 0;
    [SerializeField]
    private GameObject entityPrefab = null;
    [SerializeField]
    private Transform container = null;

    [Header("Data")]
    [SerializeField]
    private bool isPoolerReady = false;
    [SerializeField]
    protected T[] entities = null;
    #endregion

    #region Initialization
    protected virtual void Awake()
    {
        this.Initialize();
    }

    protected virtual void Initialize()
    {
        //Check for errors with missing references and missing components
#if UNITY_EDITOR
        if (this.entityPrefab == null)
            Debug.LogError("[Missing Reference] - entity is missing !");
        if(this.entityPrefab.GetComponent<T>() == null)
            Debug.LogError("[Missing Component] - T component is missing !");
	#endif

        //Instantiate the entities
        this.entities = new T[this.entityNumber];
        for(int i = 0; i < this.entityNumber;i++)
        {
            GameObject obj = Instantiate<GameObject>(this.entityPrefab, Vector3.zero, Quaternion.identity, (this.container == null) ? null : this.container);
            obj.name = this.entityPrefab.name + i;
            this.entities[i] = obj.GetComponent<T>();
            this.entities[i].Initialize();
        }
        this.isPoolerReady = true;
    }
    #endregion

    #region Properties
    /// <summary>
    /// Is the pooler ready to be used.
    /// </summary>
    public bool IsPoolerReady { get { return this.isPoolerReady; } }

    /// <summary>
    /// Array containing all the entities.
    /// </summary>
    public T[] Entities { get { return this.entities; } }
    #endregion

    #region Methods
    /// <summary>
    /// Get an available entity in the pool. Return null if none.
    /// </summary>
    /// <returns>Return the 1st available entity in the pool. Return null if none.</returns>
    public T GetEntity()
    {
        for(int i = 0; i < this.entities.Length;i++)
        {
            if(!this.entities[i].IsEnable)
            {
                this.entities[i].Enable();
                return this.entities[i];
            }
        }
        return null;
    }

    /// <summary>
    /// Reset the pooler, disable all the gameObject in the Hierarchy.
    /// </summary>
    public void Reset()
    {
        for(int i = 0; i < this.entities.Length;i++)
        {
            if(this.entities[i].IsEnable)
            {
                this.entities[i].Disable();
            }
        }
    }
    #endregion
}
