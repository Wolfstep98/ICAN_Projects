using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBehaviour : MonoBehaviour 
{
    #region Fields & Properties
    [Header("Parameters")]
    [SerializeField]
    private float explosionTimeMin = 3.0f;
    [SerializeField]
    private float explosionTimeMax = 5.0f;
    [SerializeField]
    private float explosionTime = 0.0f;

    [SerializeField]
    private float explosionForceMin = 100.0f;
    [SerializeField]
    private float explosionForceMax = 1000.0f;
    [SerializeField]
    private float explosionForce = 0.0f;

    [SerializeField]
    private float explosionRadius = 3.0f;
	#endregion

	#region Methods
	#region Initializers
	// Use this for initialization
	private void Awake () 
	{
        this.Initialize();
	}

    private void Initialize()
    {
        this.explosionTime = Random.Range(this.explosionTimeMin, this.explosionTimeMax);
        this.explosionForce = Random.Range(this.explosionForceMin, this.explosionForceMax);
    }
	#endregion
	
	// Update is called once per frame
	private void Update () 
	{
        this.explosionTime -= Time.deltaTime;
        if(this.explosionTime <= 0.0f)
        {
            Collider[] colliders = Physics.OverlapSphere(this.transform.position, this.explosionRadius);
            if(colliders.Length > 0)
            {
                for(int i = 0; i < colliders.Length;i++)
                {
                    if(colliders[i].tag.Contains("Player"))
                    {
                        Rigidbody rb = colliders[i].GetComponent<Rigidbody>();
                        rb.AddExplosionForce(this.explosionForce, this.transform.position, this.explosionRadius, 0.0f, ForceMode.Force);
                    }
                }
            }
            Destroy(this.gameObject);
        }
	}

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(this.transform.position, this.explosionRadius);
    }
    #endregion
}
