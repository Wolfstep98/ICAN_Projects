using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public float Speed;
    public float SpeedRoation;
    private Vector3 direction;

    private MovementFromScript movement;

    // Use this for initialization
    void Start(){
        movement = GetComponent<MovementFromScript>();
    }

    void Update() {
        if (Input.GetButton("Horizontal")){
            movement.SetRotationAngle(Input.GetAxis("Horizontal") * SpeedRoation);
        }
        if (Input.GetButton("Vertical")){
            direction = new Vector3(0, 0, Input.GetAxis("Vertical") * Speed);
            movement.SetDirection(direction);
        }
        else{
            movement.SetDirection(Vector3.zero);
        }
        
    }

    public bool IsXNegatif(){
        if (direction.x >= 0){
            return false;
        }
        return true;
    }

}
