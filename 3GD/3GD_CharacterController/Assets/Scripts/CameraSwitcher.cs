using System;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    #region Fields & Properties
    [Header("Behaviour Data")]
    [SerializeField]
    private int currentCameraIndex = 0;
    [SerializeField]
    private int lastCameraIndex = 0;

    [Header("Data")]
    [SerializeField]
    private int numberOfCamera = 0; 
    public int NumberOfCamera { get { return this.numberOfCamera; } }

    [Header("References")]
    [SerializeField]
    private CustomCharacterController customCharacterController = null;
    [SerializeField]
    private Camera[] cameras = new Camera[0];
    #endregion

    #region Methods
    private void Awake()
    {
        if(this.cameras == null || this.cameras.Length == 0)
        {
            this.numberOfCamera = Camera.GetAllCameras(this.cameras);
        }
        else
        {
            this.numberOfCamera = this.cameras.Length;
        }

        this.currentCameraIndex = 0;
        this.UpdateMainCamera(0);
    }

    /// <summary>
    /// Update all the cameras behaviour, set the current main camera active and all the others inactive.
    /// </summary>
    private void UpdateMainCamera(int mainCameraIndex)
    {
        this.lastCameraIndex = this.currentCameraIndex;
        this.currentCameraIndex = mainCameraIndex;
        this.cameras[this.lastCameraIndex].gameObject.SetActive(false);
        this.cameras[this.currentCameraIndex].gameObject.SetActive(true);
    }

    public void SetMainCamera(int index)
    {
        try
        {       
            this.customCharacterController.ChangeMainCamera(this.cameras[index].transform);
            this.UpdateMainCamera(index);
            Debug.Log("Camera Switch ! " + index);
        }
        catch(IndexOutOfRangeException e)
        {
            Debug.LogError(e.Message);
        }
        catch(NullReferenceException e)
        {
            Debug.LogError(e.Message);
        }
    }
    #endregion
}
