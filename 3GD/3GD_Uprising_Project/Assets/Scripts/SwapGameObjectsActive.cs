using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapGameObjectsActive : MonoBehaviour 
{
    #region Fields & Properties
    [Header("Parameters")]
    [SerializeField]
    private bool firstActivated = true;
    [SerializeField]
    private string inputName = "Switch";

    [Header("References")]
    [SerializeField]
    private GameObject[] objOne = null;
    [SerializeField]
    private GameObject[] objTwo = null;
    #endregion

    #region Methods
    #region Initializers
    private void Awake()
    {
        this.Swap();   
    }
    #endregion

    // Update is called once per frame
    void Update () 
	{
		if(Input.GetButtonDown(this.inputName))
        {
            this.firstActivated = !this.firstActivated;
            this.Swap();
        }
	}

    private void Swap()
    {
        for(int i = 0; i < this.objOne.Length;i++)
            this.objOne[i].SetActive(this.firstActivated);
        for (int i = 0; i < this.objTwo.Length; i++)
            this.objTwo[i].SetActive(!this.firstActivated);
    }
	#endregion
}
