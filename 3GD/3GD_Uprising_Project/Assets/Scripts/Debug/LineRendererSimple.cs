using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererSimple : MonoBehaviour {

    [SerializeField] LineRenderer line;
    [SerializeField] Transform flailBase;
    [SerializeField] Transform lineTarget1;
    [SerializeField] Transform lineTarget2;
    [SerializeField] Transform lineTarget3;
    [SerializeField] Transform lineTarget4;
    [SerializeField] Transform lineTarget5;
    [SerializeField] Transform lineTarget6;
    [SerializeField] Transform lineTarget7;
    [SerializeField] Transform lineTarget8;




    void Update ()
    {
        line.SetPosition(0, flailBase.position);
        line.SetPosition(1, lineTarget1.position);
        line.SetPosition(2, lineTarget2.position);
        line.SetPosition(3, lineTarget3.position);
        line.SetPosition(4, lineTarget4.position);
        line.SetPosition(5, lineTarget5.position);
        line.SetPosition(6, lineTarget6.position);
        line.SetPosition(7, lineTarget7.position);
        line.SetPosition(8, lineTarget8.position);
        line.SetPosition(9, transform.position);
    }
}
