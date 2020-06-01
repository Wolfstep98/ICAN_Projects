using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour {

    #region Properties

    [Header("References")]
    [SerializeField] private CharacterInputDetection inputDetection = null;
    [SerializeField] private CustomCharacterController2 characterController = null;

    #endregion

    #region Initialization

    private void Awake()
	{
		this.Init();
	}

	private void Init()
	{
#if UNITY_EDITOR
		if (this.inputDetection == null)
			Debug.LogError("[Missing References] - inputDetection is not properly set !");
        if (this.characterController == null)
            Debug.LogError("[Missing References] - characterController is not properly set !");
#endif

        this.Initiate();
	}

	private void Initiate()
	{
		
	}

	#endregion

	// Update is called once per frame
	void Update () {
        this.inputDetection.CustomUpdate();

        this.characterController.CustomUpdate();
	}

	#region Methods

	//Your Methods

	#endregion
}
