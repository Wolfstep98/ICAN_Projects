using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MinionDeathTrigger : MonoBehaviour
{
    #region Methods
    #region Triggers
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == GameObjectTags.Minion)
        {
            MinionEntity entity = other.gameObject.GetComponent<MinionEntity>();
            entity.Die();
        }
    }
    #endregion
    #endregion
}
