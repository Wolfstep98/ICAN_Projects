using System;
using UnityEngine;

public class CustomPlayerControllerV2 : MonoBehaviour
{
    #region Fields & Properties
    [Header("Parameters")]
    [SerializeField]
    private float speed = 10.0f;
    [SerializeField]
    private Vector3 direction = Vector3.zero;

    [Header("References")]
    [SerializeField]
    new private Rigidbody rigidbody = null;

    [Header("Debug")]
    [SerializeField]
    private bool showDirection = true;
	#endregion
	
	#region Methods
	#region Intialization
	private void Awake()
	{
		this.Initialize();
	}
	
	private void Initialize()
	{
		
	}
	#endregion
	
    public void CustomUpdate()
    {

    }

    public void CustomFixedUpdate()
    {
        Vector3 nextPos = this.transform.position + this.direction * this.speed * Time.deltaTime;
        this.rigidbody.MovePosition(nextPos);
    }

    public void UpdateDirection(Vector2 direction)
    {
        direction.Normalize();
        Vector3 dir = new Vector3(direction.x, 0.0f, direction.y);
        this.direction = dir;
    }

    #region Debug
    private void OnDrawGizmosSelected()
    {
        if(this.showDirection)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawRay(this.transform.position, this.direction);
        }
    }
    #endregion
    #endregion
}
