using UnityEngine;
using UnityEngine.AI;
using System.Collections;

/// <summary>
/// Minion entity, inherits from MonoBehaviour and implements IEntity.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class MinionEntity : MonoBehaviour, IEntity
{
    #region Fields
    [Header("Parameters")]
    [SerializeField]
    private float followThreshold = 1.0f;
    [SerializeField]
    private Vector3 lastStationaryPosition = Vector3.zero;
    [SerializeField]
    private Color neutralColor = Color.gray;
    [SerializeField]
    private Color followColor = Color.cyan;

    [Header("Data")]
    [SerializeField]
    private bool isEnable = false;
    [SerializeField]
    private bool isFollowing = false;

    //--- Events ---
    public delegate void MinionDeath(MinionEntity entity);
    private MinionDeath minionDeath = null;

    [Header("References")]
    [SerializeField]
    private MeshRenderer meshRenderer = null;
    [SerializeField]
    private NavMeshAgent agent = null;
    [SerializeField]
    new private Collider collider = null;
    [SerializeField]
    private Transform target = null;
    [SerializeField]
    private Transform spawn = null;
    #endregion

    #region Properties
    public bool IsEnable { get { return this.isEnable; } }
    public bool IsFollowing { get { return this.isFollowing; } }
    #endregion

    #region Methods
    #region IEntity
    public void Initialize()
    {
        this.Disable();
    }

    public void Disable()
    {
        this.isEnable = false;
        if(this.isFollowing)
        {
            this.StopFollow();
        }
        this.target = null;
        this.meshRenderer.material.color = this.neutralColor;
        this.agent.enabled = false;
        this.gameObject.SetActive(false);
    }

    public void Enable()
    {
        this.isEnable = true;
        this.gameObject.SetActive(true);
        this.StartCoroutine(this.Wait1FrameForAgentActivation());
    }

    private IEnumerator Wait1FrameForAgentActivation()
    {
        yield return null;
        this.agent.enabled = true;
    }

    public void SetupSpawn(Transform spawnPoint)
    {
        this.spawn = spawnPoint;
    }

    public void Respawn()
    {
        this.transform.position = this.spawn.position;
        this.Enable();
    }

    public void Die()
    {
        this.Disable();
        if(this.spawn != null)
            this.Respawn();
        this.minionDeath.Invoke(this);
    }
    #endregion

    #region Update
    private void FixedUpdate()
    {
        if (this.isFollowing)
        {
            if ((this.lastStationaryPosition - this.target.transform.position).magnitude >= this.followThreshold)
            {
                this.SetDestination();
            }
        }
    }
    #endregion

    #region NavMesh

    public void StartFollow()
    {
        if (this.target != null)
        {
            this.isFollowing = true;
            this.collider.isTrigger = true;
            this.agent.SetDestination(this.target.position);
            this.meshRenderer.material.color = this.followColor;
        }
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    public void ResetTarget()
    {
        this.target = null;
    }

    public void SetDestination()
    {
        this.agent.SetDestination(this.target.position);
        this.lastStationaryPosition = this.target.transform.position;
    }

    public void StopFollow()
    {
        if (this.isFollowing)
        {
            this.isFollowing = false;
            this.collider.isTrigger = false;
            this.meshRenderer.material.color = this.neutralColor;
            this.agent.ResetPath();
        }
    }
    #endregion

    #region Events
    public void RegisterEventMinionDeath(MinionDeath minionDeath)
    {
        this.minionDeath += minionDeath;
    }

    public void UnregisterEventMinionDeath(MinionDeath minionDeath)
    {
        this.minionDeath -= minionDeath;
    }
    #endregion
    #endregion
}
