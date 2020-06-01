using System;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider))]
public class Switch : MonoBehaviour
{
    #region Fields & Properties
    [Header("Parameters")]
    [SerializeField]
    private int numberOfMinionsToActivateSwitch = 6;

    [Header("Data")]
    [SerializeField, ReadOnly]
    private bool isSwitchOn = false;
    [SerializeField, ReadOnly]
    private int currentNumberOfMinions = 0;

    //--- Events ---
    public delegate void OnSwitchStateChanged(bool state);
    private OnSwitchStateChanged onSwitchStateChanged = null;

    [Header("References")]
    [SerializeField]
    private Animator animator = null;
    [SerializeField]
    private TextMeshProUGUI uiValues = null;
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
        if (this.animator == null)
            Debug.LogError("[Missing Reference] - doorAnimator is missing !");
#endif

        this.uiValues.text = this.currentNumberOfMinions + " / " + this.numberOfMinionsToActivateSwitch;
    }
    #endregion

    #region Switch
    private void SetSwitch(bool on)
    {
        this.isSwitchOn = on;
        this.animator.SetBool("Open", on);
        if(this.onSwitchStateChanged != null)
            this.onSwitchStateChanged.Invoke(on);
    }

    public void DisableSwitch()
    {
        this.SetSwitch(false);
        this.currentNumberOfMinions = 0;

        this.uiValues.text = this.currentNumberOfMinions + " / " + this.numberOfMinionsToActivateSwitch;
    }

    public void RegisterSwitchStateChangedEvent(OnSwitchStateChanged onSwitchStateChanged)
    {
        this.onSwitchStateChanged += onSwitchStateChanged;
    }

    public void UnregisterSwitchStateChangedEvent(OnSwitchStateChanged onSwitchStateChanged)
    {
        this.onSwitchStateChanged -= onSwitchStateChanged;
    }
    #endregion

    #region Collisions
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == GameObjectTags.Minion)
        {
            this.currentNumberOfMinions++;

            this.uiValues.text = this.currentNumberOfMinions + " / " + this.numberOfMinionsToActivateSwitch;
            if (!this.isSwitchOn)
            {
                if (this.currentNumberOfMinions >= this.numberOfMinionsToActivateSwitch)
                {
                    this.SetSwitch(true);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == GameObjectTags.Minion)
        {
            if(this.currentNumberOfMinions > 0)
                this.currentNumberOfMinions--;

            this.uiValues.text = this.currentNumberOfMinions + " / " + this.numberOfMinionsToActivateSwitch;
            if (this.isSwitchOn)
            {
                if (this.currentNumberOfMinions < this.numberOfMinionsToActivateSwitch)
                {
                    this.SetSwitch(false);
                }
            }
        }
    }
    #endregion
    #endregion
}
