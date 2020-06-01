using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CustomCharacterController : MonoBehaviour
{
    [SerializeField]
    private Vector3 direction = Vector3.zero;

    [SerializeField]
    private bool free = true;
    [SerializeField]
    private float freeSpeed = 10.0f;
    [SerializeField]
    private bool run = false;
    [SerializeField]
    private float runSpeed = 13.0f;
    [SerializeField]
    private bool aim = false;
    [SerializeField]
    private float aimSpeed = 5.0f;

    [SerializeField]
    private CharacterController characterController = null;
    [SerializeField]
    private CustomCameraController cameraController = null;
    [SerializeField]
    private Camera mainCamera = null;

    public void CustomUpdate()
    {
        if (run)
            characterController.Move(direction * Time.deltaTime);
        else
            characterController.Move(direction * Time.deltaTime);
    }

    public void UpdateDirection(Vector3 direction)
    {
        Vector3 eulerAngles = new Vector3(0.0f, mainCamera.transform.eulerAngles.y, 0.0f);
        direction = Quaternion.Euler(eulerAngles) * direction;
        direction.Normalize();

        if(free)
        {
            this.direction = direction * freeSpeed;
        }
        else if(run)
        {
            this.direction = direction * runSpeed;
        }
        else if(aim)
        {
            this.direction = direction * aimSpeed;
        }
        else
        {
            this.direction = direction * freeSpeed;
        }
    }

    public void UpdateRotation(Vector3 rotation)
    {
        transform.Rotate(Vector3.up, rotation.x * Time.deltaTime);
    }

    public void UpdateCharacter(bool free, bool run, bool aim)
    {
        this.free = free;
        this.run = run;
        this.aim = aim;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, direction);
    }
}
