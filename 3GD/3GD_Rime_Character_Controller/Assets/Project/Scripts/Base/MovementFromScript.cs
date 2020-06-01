using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementFromScript : MonoBehaviour {

   

    public Transform GameObjectToMove;

    private Vector3 direction;
    private float rotationAngle;

    // Update is called once per frame
    void Update () {
        MoveGameObject(GameObjectToMove, direction);
	}

    public void MoveGameObject(Transform t, Vector3 v)    {
        GameObjectToMove.rotation = Quaternion.Euler(0, rotationAngle, 0);
        v = t.rotation * v;
        GameObjectToMove.position = new Vector3(t.position.x + v.x * Time.deltaTime,
           t.position.y + v.y * Time.deltaTime,
           t.position.z + v.z * Time.deltaTime);
        
    }

    public void SetDirection(Vector3 dir){
        direction = dir;
    }

    public void SetRotationAngle(float rot){
        rotationAngle = rotationAngle + rot * Time.deltaTime;
    }
}
