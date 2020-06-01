using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapHandler : MonoBehaviour {

    public Transform childUpTransf;
    public Transform childDownTransf;

    public Rigidbody rigid;

    public float upPos;
    public float downPos;

    [Header("Snap Settings")]
    public bool upPointAllSides;
    public bool upPointTwoSides;


    // Use this for initialization
    void Awake () {
        upPos = downPos = float.NaN;
        rigid = GetComponent<Rigidbody>();

        for (int i = 0; i < transform.childCount;i++)
        {
            if(transform.GetChild(i).name == "UpPoint")
            {
                childUpTransf = transform.GetChild(i);
                upPos = childUpTransf.transform.localPosition.y;
            }
            /*if (transform.GetChild(i).name == "DownPoint")
            {
                childDownTransf = transform.GetChild(i);
                downPos = childDownTransf.transform.localPosition.y;
            }*/
        }
        
    }
	
	// Update is called once per frame
	void Update () {
        if (rigid.velocity == Vector3.zero && rigid.angularVelocity == Vector3.zero)
        {
            if (upPointAllSides)
            {
                if (Vector3.Dot(gameObject.transform.up, Vector3.up) >= 0.9)
                {
                    childUpTransf.localPosition = Vector3.up * upPos;
                    childUpTransf.localEulerAngles = new Vector3(0f, 0f, 0f);
                }
                else if (Vector3.Dot(gameObject.transform.right, Vector3.up) >= 0.9)
                {
                    childUpTransf.localPosition = Vector3.right * upPos;
                    childUpTransf.localEulerAngles = new Vector3(0f, 0f, -90f);
                }
                else if (Vector3.Dot(gameObject.transform.forward, Vector3.up) >= 0.9)
                {
                    childUpTransf.localPosition = Vector3.forward * upPos;
                    childUpTransf.localEulerAngles = new Vector3(90f, 0f, 0f);
                }
                else if (Vector3.Dot(gameObject.transform.up, Vector3.down) >= 0.9)
                {
                    childUpTransf.localPosition = Vector3.up * -upPos;
                    childUpTransf.localEulerAngles = new Vector3(180f, 0f, 0f);
                }
                else if (Vector3.Dot(gameObject.transform.right, Vector3.down) >= 0.9)
                {
                    childUpTransf.localPosition = Vector3.right * -upPos;
                    childUpTransf.localEulerAngles = new Vector3(0f, 0f, 90f);
                }
                else if (Vector3.Dot(gameObject.transform.forward, Vector3.down) >= 0.9)
                {
                    childUpTransf.localPosition = Vector3.forward * -upPos;
                    childUpTransf.localEulerAngles = new Vector3(-90f, 0f, 0f);
                }
        }
            else if(upPointTwoSides)
            {
                if (Vector3.Dot(gameObject.transform.up, Vector3.up) >= 0.9)
                {
                    childUpTransf.localPosition = Vector3.up * upPos;
                    childUpTransf.localEulerAngles = new Vector3(0f, 0f, 0f);
                }
                else if (Vector3.Dot(gameObject.transform.up, Vector3.down) >= 0.9)
                {
                    childUpTransf.localPosition = Vector3.up * -upPos;
                    childUpTransf.localEulerAngles = new Vector3(180f, 0f, 0f);
                }
            }   
        }
    }

    public void ResetUpPoint()
    {
        childUpTransf.localPosition = Vector3.up * upPos;
        childUpTransf.localEulerAngles = new Vector3(0f, 0f, 0f);
    }
}
