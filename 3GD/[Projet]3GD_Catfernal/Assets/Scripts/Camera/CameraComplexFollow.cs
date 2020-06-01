using System.Collections;
using System.Collections.Generic;
using Game;
using Game.Constants;
using Game.Entities.Player;
using UnityEngine;

public class CameraComplexFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float speedFocusFire;
    [SerializeField] private float speedFocusPlayer;
    [SerializeField] private float offset;
    private Vector3 moveVector;

    private float lerpValue = 0;
    
    void Update() {
        if (Input.GetButton(InputNames.FlameThrower)){
            lerpValue += GameTime.deltaTime * speedFocusFire;
            if (lerpValue > 1)
                lerpValue = 1;
        }
        else if(lerpValue > 0){
            lerpValue -= GameTime.deltaTime * speedFocusPlayer;
            if (lerpValue < 0)
                lerpValue = 0;
        }

        var positionPlayer = new Vector2(this.target.position.x, this.target.position.y);
        var direction = new Vector2(target.right.x, target.right.y);
        var positon = Vector2.Lerp(positionPlayer, positionPlayer + direction * offset, lerpValue);
        this.moveVector = new Vector3(positon.x, positon.y, -10);
        transform.position = this.moveVector;

    }
}
