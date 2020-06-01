using System;
using UnityEngine;

/// <summary>
/// Holds all the LevelData that we need.
/// </summary>
[Serializable]
public class LevelData 
{
    #region Fields
    [Header("References")]
    [SerializeField]
    private Transform spawnPoint = null;
    [SerializeField]
    private Transform[] minionsSpawners = null;
    [SerializeField]
    private Switch[] switches = null;
	#endregion
	
	#region Contructors
	public LevelData ()
	{
		
	}
    #endregion

    #region Properties
    public Transform SpawnPoint { get { return this.spawnPoint; } }
    public Transform[] MinionsSpawners { get { return this.minionsSpawners; } }
    public Switch[] Switches { get { return this.switches; } }
    #endregion
}
