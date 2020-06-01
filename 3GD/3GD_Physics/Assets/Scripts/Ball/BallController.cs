using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    #region Fields & Properties
    #region Enums
    public enum PhysicState
    {
        Normal = 0,
        Bounce = 1,
        Metal = 2
    }
    #endregion

    [Header("Data")]
    [SerializeField]
    private Vector2 screenCenter = Vector2.zero;

    [Header("Parameters")]
    [SerializeField]
    private bool inputUpdated = false;
    [SerializeField]
    private float mouseMaxDistance = 10.0f;
    [SerializeField]
    private float mouseCursorDistance = 5.0f;
    [SerializeField]
    private float speed = 0.0f;
    [SerializeField]
    private float maxSpeed = 10.0f;
    [SerializeField]
    private Vector2 mouseCursorDirection = Vector2.zero;
    [SerializeField]
    private Vector3 direction = Vector3.zero;
    [SerializeField]
    private PhysicState state = PhysicState.Normal;

    [Header("States")]
    [Header("Normal")]
    [SerializeField]
    private float normalMaxSpeed = 10.0f;
    [SerializeField]
    private float normalMass = 1.0f;
    [SerializeField]
    private Material normalMaterial = null;
    [Header("Bounce")]
    [SerializeField]
    private Material bounceMaterial = null;
    [Header("Metal")]
    [SerializeField]
    private float metalMaxSpeed = 15.0f;
    [SerializeField]
    private float metalMass = 3.0f;
    [SerializeField]
    private Material metalMaterial = null;

    [Header("Physics Material")]
    [SerializeField]
    private PhysicMaterial normal = null;
    [SerializeField]
    private PhysicMaterial bounce = null;
    [SerializeField]
    private PhysicMaterial metal = null;

    [Header("References")]
    [SerializeField]
    new private Rigidbody rigidbody = null;
    [SerializeField]
    new private Collider collider = null;
    [SerializeField]
    private MeshRenderer meshRenderer = null; 

    [Header("Debug")]
    [SerializeField]
    private bool showDirection = true;
	#endregion

	#region Methods
	#region Initializers
	private void Awake () 
	{
        this.Initialize();
	}

    private void Initialize()
    {
#if UNITY_EDITOR
        if (this.rigidbody == null)
            Debug.LogError("[Missing Reference] - rigidbody is missing !");
        if (this.collider == null)
            Debug.LogError("[Missing Reference] - collider is missing !");
        if (this.meshRenderer == null)
            Debug.LogError("[Missing Reference] - meshRenderer is missing !");
# endif

        this.InitializeData();
    }

    private void InitializeData()
    {
        this.screenCenter = new Vector2(Screen.width / 2.0f, Screen.height / 2.0f);
        this.mouseMaxDistance = Screen.height / 2.0f;
    }
	#endregion
	
    public void CustomUpdate()
    {

    }

    public void CustomFixedUpdate()
    {
        if(this.inputUpdated)
        {
            this.ApplyMovement();
            this.inputUpdated = false;
        }
    }

    private void ApplyMovement()
    {
        this.direction = new Vector3(this.mouseCursorDirection.x, 0.0f, this.mouseCursorDirection.y).normalized;
        this.speed = this.maxSpeed * (this.mouseCursorDistance / this.mouseMaxDistance);
        this.direction *= this.speed;
        this.rigidbody.AddForce(this.direction, ForceMode.Acceleration);
    }

    public void UpdateMovement(Vector3 mousePos)
    {
        this.mouseCursorDirection = new Vector2(mousePos.x - this.screenCenter.x, mousePos.y - this.screenCenter.y).normalized;
        this.mouseCursorDistance = Vector2.Distance(mousePos, this.screenCenter);
        this.mouseCursorDistance = ((this.mouseCursorDistance < this.mouseMaxDistance) ? this.mouseCursorDistance : this.mouseMaxDistance);
        this.mouseCursorDirection = this.mouseCursorDirection * this.mouseCursorDistance;
        this.inputUpdated = true;
    }

    public void UpdateState(int value)
    {
        PhysicState state = (PhysicState)value;
        if (this.state != state)
        {
            this.state = state;
            switch (this.state)
            {
                case PhysicState.Normal:
                    this.collider.material = this.normal;
                    this.rigidbody.mass = this.normalMass;
                    this.maxSpeed = this.normalMaxSpeed;
                    this.meshRenderer.material = this.normalMaterial;
                    break;
                case PhysicState.Bounce:
                    this.collider.material = this.bounce;
                    this.rigidbody.mass = this.normalMass;
                    this.maxSpeed = this.normalMaxSpeed;
                    this.meshRenderer.material = this.bounceMaterial;
                    break;
                case PhysicState.Metal:
                    this.collider.material = this.metal;
                    this.rigidbody.mass = this.metalMass;
                    this.maxSpeed = this.metalMaxSpeed;
                    this.meshRenderer.material = this.metalMaterial;
                    break;
                default:
                    Debug.Log("Error - State value must be between 0 and 2.");
                    break;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if(this.showDirection)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawRay(this.transform.position, this.direction);
        }
    }
    #endregion
}
