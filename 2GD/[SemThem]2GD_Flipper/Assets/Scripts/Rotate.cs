using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Pour faire une rotation propre, prendre l'angle entre le premier vecteur au début du mouvement 
 * et le comparer avec le même mais à un instant T. Lorsqu'il dépasse les 180°, on stop la platforme.
 */



public class Rotate : MonoBehaviour {

    public bool isRotating;

    public float rotationValue;
    public float rotationValueInit;
    public float rotationAngle;
    public float initialRotationAngle;

    public Vector3 rotateObjet;
    public Vector3 objRotation;

    private void Start()
    {
        objRotation = transform.rotation.eulerAngles;
        rotateObjet = transform.rotation.eulerAngles;
        isRotating = false;
        rotationValueInit = 0f;
        rotationValue = 0;
        rotationAngle = 0;
        initialRotationAngle = 0;
    }

    private void Update()
    {
        rotateObjet = transform.rotation.eulerAngles;
        if (transform.rotation.eulerAngles != objRotation && !isRotating)
        {
            isRotating = true;
        }
        else if (isRotating)
        {
            if (transform.rotation.eulerAngles.z > objRotation.z )
                rotationValue += transform.rotation.eulerAngles.z - objRotation.z;
            if (transform.rotation.eulerAngles.z < objRotation.z)
                rotationValue += objRotation.z - transform.rotation.eulerAngles.z;
            if (rotationValueInit == 0)
                rotationValueInit = rotationValue;
            rotationAngle = Vector3.Angle(transform.up, Vector3.up);

        }
        if(isRotating && (rotationAngle > 175 || rotationAngle < 5) && rotationValue > 30)
        //if(isRotating && (Vector3.Angle(transform.up,Vector3.up) < 3 || Vector3.Angle(transform.up, Vector3.down) < 3) && Mathf.Abs(rotationValue) > 175)
        //if (isRotating && (Vector3.Dot(Vector3.up, transform.up) > 0.99 || Vector3.Dot(Vector3.up, transform.up) < -0.99) && Mathf.Abs(rotationValue) > 179)
        {
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            isRotating = false;
            rotationValue = 0f;
            if (GetComponent<Transform>().rotation.eulerAngles.z >= 150 && GetComponent<Transform>().rotation.eulerAngles.z <= 210)
            {
                transform.SetPositionAndRotation(transform.position, Quaternion.Euler(new Vector3(0, 0, 180f)));
            }
            else if ((GetComponent<Transform>().rotation.eulerAngles.z >= 350 && GetComponent<Transform>().rotation.eulerAngles.z <= 360) || (GetComponent<Transform>().rotation.eulerAngles.z >= 0 && GetComponent<Transform>().rotation.eulerAngles.z <= 10))
            {
                transform.SetPositionAndRotation(transform.position, Quaternion.Euler(Vector3.zero));
            }
            rotationValueInit = 0;
        }
        objRotation = transform.rotation.eulerAngles;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //collision.gameObject.GetComponent<Rigidbody>().AddForce((collision.contacts[0].point - collision.gameObject.transform.position).normalized, ForceMode.Force);
        }
    }
}
