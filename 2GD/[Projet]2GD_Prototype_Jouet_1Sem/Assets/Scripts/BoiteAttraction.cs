using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoiteAttraction : MonoBehaviour {

    //Boite boiboite;
    private Collider[] colliderObjAutour;
    public Collider[] colliderUtile;
    public LineRenderer line;
    public LineRenderer[] lineTab;

    public const float puissanceMin = 1f;
    public float puissance = 10f;
    public const float puissanceMax = 1000f;

    //Use this for first initialization
    private void Awake()
    {
       //boiboite = new Boite();
    }

    // Use this for initialization
    void Start () {
        colliderUtile = new Collider[20];
        colliderObjAutour = new Collider[20];
        lineTab = new LineRenderer[20];

        colliderObjAutour = Physics.OverlapSphere(transform.position, 10f);
        int j = 0;
        for (int i = 0; i < colliderObjAutour.Length; i++)
        {
            if(colliderObjAutour[i].gameObject.layer == LayerMask.NameToLayer("Interaction Object"))
            {
                colliderUtile[j] = colliderObjAutour[i];
                lineTab[j] = LineRenderer.Instantiate(line);
                j++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        int i = 0;
        if (Input.GetKey(KeyCode.Space))
        {
            foreach (Collider col in colliderUtile)
            {
                if (col != null)
                {
                    GameObject obj = col.gameObject;
                    Vector3 objToBoite = transform.position - obj.GetComponent<Transform>().position;
                    objToBoite.Normalize();
                    obj.GetComponent<Rigidbody>().AddForce(objToBoite * puissance);
                    lineTab[i].enabled = true;
                    lineTab[i].SetPositions(new Vector3[2] { obj.transform.position, obj.transform.position + objToBoite * 2 });
                }
                else
                {
                    if (lineTab[i] != null)
                    {
                        Object.Destroy(lineTab[i].gameObject);
                    }
                    lineTab[i] = null;
                }
                i++;
            }
        }
        else if(Input.GetKeyUp(KeyCode.Space))
        {
            foreach(LineRenderer line in lineTab)
            {
                if (line != null)
                {
                    if (line.enabled)
                        line.enabled = false;
                }
            }
        }
        if (puissance < puissanceMin)
            puissance = puissanceMin;
        if (puissance > puissanceMax)
            puissance = puissanceMax;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Interaction Object"))
        {
           Object.Destroy(collision.gameObject);
        }
    }
}
