using System;
using UnityEngine;

[Serializable]
public class PoolerOveruseException : ApplicationException
{
    public PoolerOveruseException() { }
    public PoolerOveruseException(string message) : base(message) { }
    public PoolerOveruseException(string message, Exception inner) : base(message, inner) { }
    protected PoolerOveruseException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}

public class SimplePool : MonoBehaviour 
{
    #region Fields & Properties
    [Header("Parameters")]
    [SerializeField]
    private int numberOfEntities = 10;
    [SerializeField]
    private GameObject entity = null;
    [Header("Data")]
    [SerializeField]
    private bool poolerReady = false;
    [SerializeField]
    private int currentIndex = 0;
    [SerializeField]
    private GameObject[] pooledEntities = null;
	#endregion

	#region Methods
    public void Create()
    {
        if(this.entity != null)
        {
            if (!this.poolerReady)
            {
                this.pooledEntities = new GameObject[this.numberOfEntities];
                for (int i = 0; i < this.numberOfEntities; i++)
                {
                    GameObject obj = Instantiate<GameObject>(this.entity, Vector3.zero, Quaternion.identity);
                    obj.name = this.entity.name + "_" + i;
                    obj.SetActive(false);
                    this.pooledEntities[i] = obj;
                }
                this.poolerReady = true;
                this.currentIndex = 0;
            }
            else
            {
                Debug.LogError("[Error] - pool has already been created !");
            }
        }
        else
        {
            Debug.LogError("[Missing Reference] - entity is missing !");
        }
    }

    public void WakeUp()
    {
        if (this.poolerReady)
        {
            //Index
            if (this.currentIndex < this.numberOfEntities)
            {
                this.pooledEntities[this.currentIndex].SetActive(true);
                this.currentIndex++;
            }
            else
            {
                throw new PoolerOveruseException("All the objects in the pooler is already pooled !");
            }

            //Boucle
            //int objActiveInHierarchy = 0;
            //for(int i = 0; i < this.pooledObjects.Length;i++)
            //{
            //    if(!this.pooledObjects[i].activeInHierarchy)
            //    {
            //        this.pooledObjects[i].SetActive(true);
            //    }
            //    else
            //    {
            //        objActiveInHierarchy++;
            //    }
            //}
            //if(objActiveInHierarchy == this.numberOfEntities)
            //{
            //    throw new PoolerOveruseException("All the objects in the pooler is pooled !");
            //}
        }
    }

    public void Clean()
    {
        if (this.poolerReady)
        {
            if (this.pooledEntities != null)
            {
                for (int i = 0; i < this.pooledEntities.Length; i++)
                {
                    Destroy(this.pooledEntities[i]);
                    this.pooledEntities[i] = null;
                }
                this.pooledEntities = null;
                this.poolerReady = false;
            }
        }
    }
	#endregion
}
