using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereMoving : MonoBehaviour
{
    public Vector3 lastMousePos;
    public Vector3 mousePosition;
    public Vector3 objPos;

    public bool neBougePas = false;
    private bool isMoving;

    public Camera mainCam;

    private Rigidbody2D rigid;

    public GameManager2 script;

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        isMoving = false;
        script = GameObject.Find("GameManager").GetComponent<GameManager2>();
        //neBougePas = script.sphereNotMoving;
    }

    private void Update()
    {
        //neBougePas = script.sphereNotMoving;
        if (neBougePas)
            rigid.isKinematic = !isMoving;
        else
            rigid.isKinematic = false;
    }

    private void OnMouseDown()
    {
        lastMousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        if(neBougePas)
            isMoving = true;
    }

    private void OnMouseDrag()
    {
        
        mousePosition = mainCam.ScreenToWorldPoint(Input.mousePosition);
        if (lastMousePos != mousePosition)
        {
            Vector3 translationVec = mousePosition - lastMousePos;
            translationVec.z = 0f;
            objPos = translationVec;
            transform.position += objPos;
        }
        lastMousePos = mousePosition;
    }

    private void OnMouseUp()
    {
        if (neBougePas)
            isMoving = false;
    }
}
