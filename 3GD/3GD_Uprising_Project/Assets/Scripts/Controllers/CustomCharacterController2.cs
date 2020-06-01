using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCharacterController2 : MonoBehaviour {

    #region Properties

    [Header("References")]
    [SerializeField] private Transform armPivot = null;

    [Header("Data")]
    [SerializeField] private float armRotateSpeed = 10.0f;
    [SerializeField] private Transform hand = null;

    private Vector3 armRotation = Vector3.zero;

	#endregion

	#region Initialization

	private void Awake()
	{
		this.Init();
	}

	private void Init()
	{
#if UNITY_EDITOR
		if (this.armPivot == null)
			Debug.LogError("[Missing References] - armPivot is not properly set !");
        if (this.hand == null)
            Debug.LogError("[Missing References] - hand is not properly set !");
#endif

        this.Initiate();
	}

	private void Initiate()
	{
		
	}

	#endregion

	// Update is called once per frame
	public void CustomUpdate () {
        this.CorrectArmMove();
        this.MoveArm();
	}

	#region Methods

	public void SetArmRotation(Vector2 direction) {
        this.armRotation = new Vector3(-direction.y, direction.x, 0);
    }

    public void SetMoveDirection(Vector2 direction) {

    }

    private void CorrectArmMove() {
        Debug.Log(this.hand.position - this.armPivot.position);
    }

    private void MoveArm() {
        this.armPivot.Rotate(this.armRotation * this.armRotateSpeed,Space.World);
    }

    #endregion

    #region Gizmos

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
    }

    #endregion
}
