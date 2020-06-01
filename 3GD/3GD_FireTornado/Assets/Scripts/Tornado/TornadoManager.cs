using UnityEngine;

public class TornadoManager : MonoBehaviour
{
    #region Fields
    [Header("References")]
    [SerializeField] private TornadoInput input = null;
    [SerializeField] private TornadoController controller = null;
    #endregion

    #region Methods
    private void Update()
    {
        this.input.CustomUpdate();

        this.controller.CustomUpdate();
    }
    #endregion
}
