using UnityEngine;

public class TornadoController : MonoBehaviour
{
    #region Fields
    private int index = 0;
    [SerializeField] private float speed = 5.0f;
    private Vector3 direction = Vector3.zero;

    [Header("References")]
    [SerializeField] private Transform tornado = null;
    [SerializeField] private Transform[] tornados = null;
    #endregion

    #region Methods
    private void Awake()
    {
        this.index = -1;
    }

    private void Start()
    {
        this.GetNextTornado();
    }

    public void CustomUpdate()
    {
        this.tornado.position += this.direction * this.speed * Time.deltaTime;
    }

    /// <summary>
    /// Set the direction of the current tornado.
    /// </summary>
    /// <param name="direction">The direction.</param>
    public void SetDirection(Vector3 direction)
    {
        if (direction == Vector3.zero)
        {
            this.direction = Vector3.zero;
            return;
        }
        this.direction = direction.normalized;
    }

    /// <summary>
    /// Get the next tornado in the array.
    /// </summary>
    public void GetNextTornado()
    {
        this.tornado.GetComponent<TornadoShader>().UpdateSelected(false);

        this.index++;
        this.tornado = this.tornados[this.index % this.tornados.Length];

        this.tornado.GetComponent<TornadoShader>().UpdateSelected(true);
    }
    #endregion
}
