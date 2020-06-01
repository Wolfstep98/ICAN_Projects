using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour 
{
    [SerializeField]
    private CustomCharacterControllerInput characterInput = null;
    [SerializeField]
    private CustomCharacterController characterController = null;
    [SerializeField]
    private CustomCameraInput cameraInput = null;
    [SerializeField]
    private CustomCameraController cameraController = null;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        characterInput.CustomUpdate();
        cameraInput.CustomUpdate();

        characterController.CustomUpdate();
        cameraController.CustomUpdate();
    }
}
