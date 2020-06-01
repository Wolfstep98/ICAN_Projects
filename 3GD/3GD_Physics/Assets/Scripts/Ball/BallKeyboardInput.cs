using UnityEngine;
using UnityEngine.Events;

public class BallKeyboardInput : MonoBehaviour 
{
    #region Fields & Properties
    [Header("Events")]
    [SerializeField]
    private UnityEvent spaceEvent;
	#endregion

	#region Methods
	private void Update () 
	{
		if(Input.GetButtonDown(InputNames.Bomb))
        {
            this.spaceEvent.Invoke();
        }
	}
	#endregion
}
