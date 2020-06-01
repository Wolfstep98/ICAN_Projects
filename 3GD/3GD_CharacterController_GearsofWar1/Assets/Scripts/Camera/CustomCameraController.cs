using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomCameraController : MonoBehaviour
{
    [SerializeField]
    private bool free = true;
    [SerializeField]
    private int freeFOV = 60;
    [SerializeField]
    private bool run = false;
    [SerializeField]
    private int runFOV = 65;
    [SerializeField]
    private bool aim = false;
    [SerializeField]
    private int aimFOV = 50;

    [SerializeField]
    private Vector3 cameraPosition = Vector3.zero;

    [SerializeField]
    private Vector3 freeCamPosition = Vector3.zero;
    [SerializeField]
    private Vector3 runCamPosition = Vector3.zero;
    [SerializeField]
    private Vector3 aimCamPosition = Vector3.zero;

    [SerializeField]
    private Transform budy = null;
    [SerializeField]
    private Transform player = null;
    [SerializeField]
    private Camera mainCamera = null;
    [SerializeField]
    private CustomCharacterController customCharacterController = null;

    private void Awake()
	{
        free = true;
        run = false;
        aim = false;
        cameraPosition = player.position + (budy.rotation * freeCamPosition);
    }

    public void CustomUpdate()
    {
        UpdatePosition();
 
        transform.position = cameraPosition;
        transform.rotation = budy.rotation;
    }

    private void UpdatePosition()
    {
        if(free)
        {
            cameraPosition = player.position + (budy.rotation * freeCamPosition);
        }
        else if(run)
        {
            cameraPosition = player.position + (budy.rotation * runCamPosition);
        }
        else if(aim)
        {
            cameraPosition = player.position + (budy.rotation * aimCamPosition);
        }
    }

    public void UpdateRotation(Vector3 rotation)
    {
        Vector3 previousRotation = budy.localRotation.eulerAngles;
        budy.Rotate(Vector3.right, -1 * rotation.y * Time.deltaTime, Space.Self);  
    }

    public void UpdateCamera(bool free, bool run, bool aim)
    {
        this.free = free;
        this.run = run;
        this.aim = aim;

        if(free)
        {
            mainCamera.fieldOfView = freeFOV;
        }
        else if (run)
        {
            mainCamera.fieldOfView = runFOV;
        }
        else if (aim)
        {
            mainCamera.fieldOfView = aimFOV;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawRay(player.position, budy.rotation * freeCamPosition);
    }
}
