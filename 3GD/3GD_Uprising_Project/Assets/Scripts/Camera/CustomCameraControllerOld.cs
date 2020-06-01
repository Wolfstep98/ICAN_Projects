using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CustomCameraControllerOld : MonoBehaviour
{
    #region Fields & Properties
    [Header("Parameters")]
    [SerializeField]
    private float minCamDistance = 5.0f;
    [SerializeField]
    private float maxCamDistance = 10.0f;
    [SerializeField]
    private float minYDistance = 1.0f;
    [SerializeField]
    private float maxYDistance = 2.0f;

    [SerializeField]
    private Vector3 offset = Vector3.zero;
    [Header("References")]
    [SerializeField]
    private Transform target = null;
	#endregion
	
	#region Methods
	#region Intialization
	private void Awake()
	{
		this.Initialize();
	}
	
	private void Initialize()
	{
#if UNITY_EDITOR
        if (this.target == null)
            Debug.LogError("[Missing Reference] - target is missing !");
#endif
    }
    #endregion

    public void CustomUpdate()
    {
        //Look at target
        this.transform.LookAt(this.target);

        //Set Y
        float yDistance = this.transform.position.y - this.target.transform.position.y;

        if(yDistance < 0.0f)
        {
            if(yDistance < this.minYDistance)
            {
                yDistance = this.target.transform.position.y + this.minYDistance;
            }
            else if(yDistance > this.maxYDistance)
            {
                yDistance = this.target.transform.position.y + this.maxYDistance;
            }
            else
            {
                yDistance += this.target.transform.position.y;
            }
        }
        else
        {
            if (yDistance < this.minYDistance)
            {
                yDistance = this.target.transform.position.y + this.minYDistance;
            }
            else if (yDistance > this.maxYDistance)
            {
                yDistance = this.target.transform.position.y + this.maxYDistance;
            }
            else
            {
                yDistance += this.target.transform.position.y;
            }
        }

        this.transform.position = new Vector3(this.target.transform.position.x + this.offset.x, yDistance, this.target.transform.position.z + this.offset.z);
    }
    #endregion
}
