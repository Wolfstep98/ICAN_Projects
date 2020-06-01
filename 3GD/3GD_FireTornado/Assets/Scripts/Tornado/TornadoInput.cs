using UnityEngine;

public class TornadoInput : MonoBehaviour
{
    #region Fields
    [Header("References")]
    [SerializeField] private TornadoController controller = null;
    #endregion

    #region Methods
    public void CustomUpdate()
    {
        //Update Tornado Direction
        this.controller.SetDirection(new Vector3(Input.GetAxis(InputNames.Horizontal), 0, Input.GetAxis(InputNames.Vertical)));

        //Get the next Tornado
        if(Input.GetKeyDown(KeyCode.Space))
        {
            this.controller.GetNextTornado();
        }
    }
    #endregion
}
