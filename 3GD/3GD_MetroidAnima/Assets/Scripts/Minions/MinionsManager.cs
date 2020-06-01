using UnityEngine;

public class MinionsManager : MonoBehaviour
{
    #region Fields
    [Header("Parameters")]
    [SerializeField]
    private int currentLevelIndex = 0;
    [SerializeField]
    private LevelData[] levelsData = null;

    [Header("Data")]
    [SerializeField, ReadOnly]
    private bool firstInit = true;

    [Header("References")]
    [SerializeField]
    private MinionPooler minionPooler = null;
    [SerializeField]
    private BallController player = null;
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
        if (this.minionPooler == null)
            Debug.LogError("[Missing Reference] - minionPooler is missing !");
        if (this.player == null)
            Debug.LogError("[Missing Reference] - player is missing !");
#endif

        this.firstInit = true;
    }

    private void Start()
    {
        this.SetupMinions(this.currentLevelIndex);
    }
    #endregion

    #region Level
    public void SetupMinions(int levelIndex)
    {
        if (levelIndex > this.levelsData.Length)
            return;

        this.currentLevelIndex = levelIndex;
        if (!firstInit)
        {
            MinionEntity[] minionEntities = this.minionPooler.Entities;
            for(int i = 0; i < minionEntities.Length;i++)
            {
                if(!minionEntities[i].IsFollowing)
                {
                    minionEntities[i].Disable();
                }
                else
                {
                    minionEntities[i].SetupSpawn(null);
                }
            }
        }
        else
        {
            this.firstInit = false;
        }

        for(int i = 0; i < this.levelsData[this.currentLevelIndex].MinionsSpawners.Length;i++)
        {
            MinionEntity minion = this.minionPooler.GetEntity();
            minion.SetupSpawn(this.levelsData[this.currentLevelIndex].MinionsSpawners[i]);
            minion.Respawn();
        }
    }

    public void RespawnPlayer()
    {
        this.player.transform.position = this.levelsData[this.currentLevelIndex].SpawnPoint.position;
        this.player.ReleaseMinions();
        this.SetupMinions(this.currentLevelIndex);
        foreach(Switch _switch in this.levelsData[this.currentLevelIndex].Switches)
        {
            _switch.DisableSwitch();
        }
    }
    #endregion

    #endregion
}
