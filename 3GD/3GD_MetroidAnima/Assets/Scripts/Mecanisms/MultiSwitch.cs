using UnityEngine;

public class MultiSwitch : MonoBehaviour
{
    #region Fields & Properties
    [Header("Parameters")]
    [SerializeField]
    private int numberOfFieldToActivate = 3;

    [Header("Data")]
    [SerializeField]
    private bool on = false;
    [SerializeField]
    private int currentNumberOfSwitchActivated = 0;

    [Header("References")]
    [SerializeField]
    private Switch[] switches = null;
    [SerializeField]
    private Animator animator = null;
    #endregion

    #region Methods
    private void Awake()
    {
        this.on = false;
        this.currentNumberOfSwitchActivated = 0;
        for(int i = 0; i < this.switches.Length;i++)
        {
            this.switches[i].RegisterSwitchStateChangedEvent(this.OnSwitchStateChanged);
        }
    }

    private void OnSwitchStateChanged(bool state)
    {
        if(state)
        {
            this.currentNumberOfSwitchActivated++;
            if(this.currentNumberOfSwitchActivated >= this.numberOfFieldToActivate)
            {
                this.on = true;
                this.animator.SetBool("Open", true);
            }
        }
        else
        {
            this.currentNumberOfSwitchActivated--;
            if(this.on)
            {
                this.on = false;
                this.animator.SetBool("Open", false);
            }
        }
    }
    #endregion
}
