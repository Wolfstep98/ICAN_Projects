using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBehaviour1 : MonoBehaviour 
{
    public enum Axis
    {
        x,y,z
    }

    public float distance = 1.0f;
    public float totalTime = 1.0f;
    public float timer = 0.0f;
    public Axis axis = Axis.x;
    public Vector3 firstPoint;
    public AnimationCurve anim;

    private void Awake () 
	{
        firstPoint = transform.position;
        timer = 0.0f;
    }

    private void Update()
    {
        timer += Time.deltaTime / totalTime;
        if (timer > 1.0f)
            timer -= 1.0f;

        Vector3 position = firstPoint;
        switch (axis)
        {
            case Axis.x:
                position.x += anim.Evaluate(timer) * distance;
                break;
            case Axis.y:
                position.y += anim.Evaluate(timer) * distance;
                break;
            case Axis.z:
                position.z += anim.Evaluate(timer) * distance;
                break;
        }

        transform.position = position;
    }
}
