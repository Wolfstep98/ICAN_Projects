using UnityEngine;

public class BallInput : MonoBehaviour 
{
    #region Fields
    [Header("References")]
    [SerializeField]
    private BallController controller = null;
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
        if (this.controller == null)
            Debug.LogError("[Missing Reference] - controller is missing !");
#endif
    }
    #endregion

    public void CustomUpdate()
    {
        this.ProcessInputs();
    }

    private void ProcessInputs()
    {
        if(Input.GetButton(InputNames.Move))
        {
            this.controller.UpdateMovement(Input.mousePosition);
        }

        if(Input.GetButtonDown(InputNames.ReleaseMinions))
        {
            this.controller.ReleaseMinions();
        }
    }
	#endregion
}
