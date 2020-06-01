using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoiteAttractionRayon : MonoBehaviour {

    public LineRenderer line;
    public Vector3 visee;

    public const float puissanceRayonMin = 100f;
    public float puissanceRayon = 500f;
    public const float puissanceRayonMax = 10000f;

    public const float tailleRayonMin = 1f;
    public float tailleRayon = 2.5f;
    public const float tailleRayonMax = 5f;

    public const float longueurRayonMin = 0.1f;
    public float longueurRayon = 5f;
    public const float longueurRayonMax = 20f;


    private void Start()
    {
        visee = transform.position + transform.right * 10f;
        line.SetPositions(new Vector3[2] { transform.position, visee});
        line.enabled = false;
    }

    private void Update()
    {
        visee = transform.position + transform.right * 10f;
        line.SetPositions(new Vector3[2] { transform.position, visee });
        if (Input.GetKeyDown(KeyCode.Space))
        {
            line.enabled = true;
        }
        if(Input.GetKey(KeyCode.Space))
        {
            Ray tir = new Ray(transform.position, visee - transform.position);
            RaycastHit rayInfo = new RaycastHit();
            if(Physics.Raycast(tir, out rayInfo, Vector3.Distance(line.GetPosition(0), line.GetPosition(1))))
            {
                GameObject obj = rayInfo.transform.gameObject;
                if (obj.layer == LayerMask.NameToLayer("Interaction Object"))
                {
                    obj.GetComponent<Rigidbody>().AddForce((transform.position - obj.transform.position).normalized * puissanceRayon * Time.deltaTime);
                }
            }
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            line.enabled = false;
        }

        if(Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(Vector3.down, 50f * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(Vector3.down, -50f * Time.deltaTime);
        }
        if (puissanceRayon > puissanceRayonMax)
            puissanceRayon = puissanceRayonMax;
        if (puissanceRayon < puissanceRayonMin)
            puissanceRayon = puissanceRayonMin;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Interaction Object"))
        {
            Object.Destroy(collision.gameObject);
        }
    }
}
