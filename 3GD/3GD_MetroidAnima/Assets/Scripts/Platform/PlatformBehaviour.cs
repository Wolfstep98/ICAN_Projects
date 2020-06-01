using UnityEngine;

public class PlatformBehaviour : MonoBehaviour 
{
    #region Fields & Properties

    [Header("Parameters")]
    [SerializeField]
    private bool isMoving = false;
    [SerializeField]
    private bool invert = false;
    [SerializeField]
    private float minDistance = 0.0f;
    [SerializeField]
    private float maxDistance = 0.0f;
    [SerializeField]
    private float precision = 0.02f;
    [SerializeField]
    private float distance = 10.0f;
    [SerializeField]
    private float moveDuration = 5.0f;
    [SerializeField]
    private float startWithDelay = 0.0f;
    [SerializeField]
    [ReadOnly]
    private float timer = 0.0f;
    [SerializeField]
    private Axis axis = Axis.X;
    [SerializeField]
    [ReadOnly]
    private Vector3 startingPoint = Vector3.zero;
    [SerializeField]
    private AnimationCurve behaviour = null;

    [Header("References")]
    [SerializeField]
    private MeshFilter meshFilter = null;
    #endregion

    #region Methods
    #region Initializers
    private void Awake () 
	{
        this.Initialize();
	}

    private void Initialize()
    {
        this.isMoving = (this.startWithDelay == 0.0f) ? true : false;
        this.startingPoint = this.transform.position;
        this.timer = (!this.invert) ? 0.0f : this.moveDuration;
        this.FindMinMaxDistance();
    }
    #endregion

    #region Behaviour
    private void Update()
    {
        if (!this.isMoving)
        {
            if(Time.timeSinceLevelLoad >= this.startWithDelay)
            {
                this.isMoving = true;
            }
        }
        else
        {
            this.timer += ((this.invert) ? -1 : 1) * Time.deltaTime / this.moveDuration;
            if (this.timer > 1.0f)
                this.timer -= 1.0f;
            else if(this.timer < 0.0f)
                this.timer += 1.0f;

            Vector3 nextPosition = this.startingPoint;
            switch (this.axis)
            {
                case Axis.X:
                    nextPosition += new Vector3(this.behaviour.Evaluate(timer) * this.distance, 0.0f, 0.0f);
                    break;
                case Axis.Y:
                    nextPosition += new Vector3(0.0f, this.behaviour.Evaluate(timer) * this.distance, 0.0f);
                    break;
                case Axis.Z:
                    nextPosition += new Vector3(0.0f, 0.0f, this.behaviour.Evaluate(timer) * this.distance);
                    break;
                default:
                    break;
            }

            this.transform.position = nextPosition;
        }
    }

    private void FindMinMaxDistance()
    {
        float min = float.MaxValue;
        float max = float.MinValue;
        for(float i = 0; i < 1; i += this.precision)
        {
            float value = this.behaviour.Evaluate(i);
            if (value > max)
                max = value;
            if (value < min)
                min = value;
        }
        this.minDistance = min;
        this.maxDistance = max;
    }
    #endregion

    #region Debug
    private void OnDrawGizmosSelected()
    {
        Vector3 pointOne = this.startingPoint;
        Vector3 pointTwo = this.startingPoint;
        switch (this.axis)
        {
            case Axis.X:
                if(this.invert)
                {
                    pointOne.x -= this.minDistance * this.distance;
                    pointTwo.x -= this.maxDistance * this.distance;
                }
                else
                {
                    pointOne.x += this.minDistance * this.distance;
                    pointTwo.x += this.maxDistance * this.distance;
                }
                break;
            case Axis.Y:
                if (this.invert)
                {
                    pointOne.y -= this.minDistance * this.distance;
                    pointTwo.y -= this.maxDistance * this.distance;
                }
                else
                {
                    pointOne.y += this.minDistance * this.distance;
                    pointTwo.y += this.maxDistance * this.distance;
                }
                break;
            case Axis.Z:
                if (this.invert)
                {
                    pointOne.z -= this.minDistance * this.distance;
                    pointTwo.z -= this.maxDistance * this.distance;
                }
                else
                {
                    pointOne.z += this.minDistance * this.distance;
                    pointTwo.z += this.maxDistance * this.distance;
                }
                break;
            default:
                break;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(pointOne, pointTwo);
        Gizmos.DrawMesh(this.meshFilter.sharedMesh, 0, pointTwo, Quaternion.identity, this.transform.lossyScale);
    }
    #endregion

    #region Editor
    private void OnValidate()
    {
        if (this.meshFilter == null)
            this.meshFilter = this.GetComponent<MeshFilter>();
        this.startingPoint = this.transform.position;
        this.FindMinMaxDistance();
    }
    #endregion

    #endregion
}
