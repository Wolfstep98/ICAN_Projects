using UnityEngine;

[RequireComponent(typeof(Collider))]
public class NextLevel : MonoBehaviour
{
    #region Fields
    [Header("Paramters")]
    [SerializeField]
    private int levelIndex = 0;

    [Header("References")]
    [SerializeField]
    private Switch previousSwitch = null;
    [SerializeField]
    private MinionsManager minionsManager = null;
    #endregion

    #region Methods
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == GameObjectTags.Player)
        {
            this.previousSwitch.DisableSwitch();
            this.minionsManager.SetupMinions(this.levelIndex);
        }
    }
    #endregion
}
