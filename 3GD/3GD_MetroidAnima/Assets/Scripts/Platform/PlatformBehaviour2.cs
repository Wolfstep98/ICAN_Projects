using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBehaviour2 : MonoBehaviour 
{
    public enum Axe
    {
        X,Y,Z
    }

    [SerializeField]
    float distance = 5.0f;
    [SerializeField]
    float temps = 1.0f;
    [SerializeField]
    float timer = 0.0f;
    [SerializeField]
    Axe axe = Axe.X;
    [SerializeField]
    Vector3 start;
    [SerializeField]
    AnimationCurve courbe;

    void Awake () 
	{
        start = transform.position;
        timer = 0.0f;
    }

    void Update()
    {
        timer += Time.deltaTime / temps;
        if (timer > 1.0f)
            timer -= 1.0f;

        Vector3 prochainePos = start;
        switch (axe)
        {
            case Axe.X:
                prochainePos.x += distance * courbe.Evaluate(timer);
                break;
            case Axe.Y:
                prochainePos.y += distance * courbe.Evaluate(timer);
                break;
            case Axe.Z:
                prochainePos.z += distance * courbe.Evaluate(timer);
                break;
        }

        transform.position = prochainePos;
    }
}
