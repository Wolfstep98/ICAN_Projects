using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedPool : MonoBehaviour 
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
    public bool PoolerReady { get { return this.poolerReady; } }
    [SerializeField]
    private int currentIndex = 0;
    public int CurrentIndex { get { return this.currentIndex; } }
    [SerializeField]
    private AdvancedEntity[] entities = null;
    public AdvancedEntity[] Entities { get { return this.entities; } }
    #endregion

    #region Methods
    public void Create()
    {
        if (this.entity != null)
        {
            if (!this.poolerReady)
            {
                this.entities = new AdvancedEntity[this.numberOfEntities];
                for (int i = 0; i < this.numberOfEntities; i++)
                {
                    GameObject obj = Instantiate<GameObject>(this.entity, Vector3.zero, Quaternion.identity);
                    obj.name = this.entity.name + "_" + i;
                    obj.SetActive(false);
                    this.entities[i] = obj.GetComponent<AdvancedEntity>();
                    this.entities[i].Init(this);
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
                this.entities[this.currentIndex].WakeUp();
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

    public void Sleep(AdvancedEntity entity)
    {
        if (this.poolerReady)
        {
            entity.GoToSleep();
            this.currentIndex--;
        }
    }

    public void Clean()
    {
        if (this.poolerReady)
        {
            if (this.entities != null)
            {
                for (int i = 0; i < this.entities.Length; i++)
                {
                    Destroy(this.entities[i].gameObject);
                    this.entities[i] = null;
                }
                this.entities = null;
                this.poolerReady = false;
            }
        }
    }
    #endregion
}
