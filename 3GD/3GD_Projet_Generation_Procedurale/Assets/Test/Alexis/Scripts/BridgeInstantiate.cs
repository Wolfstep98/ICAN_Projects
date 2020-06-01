using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeInstantiate : MonoBehaviour
{


    Raycast hit;

    public Vector3 playerposition;

    public ArrayList[] tab;

    public GameObject[] prefabs;
    public float prefabLength = 10;

    public GameObject target;
    public GameObject bridge1;
    public GameObject bridge2;
    public GameObject bridge3;

    public Renderer render;

    public float timer;
    public bool looked = false;



    void Start()
    {

        render = gameObject.GetComponent<Renderer>();

    }

    void Update()
    {


        if (render.isVisible)
        {

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //GameObject instance = Instantiate<GameObject>(Resources.Load, "bridge", typeof(GameObject)), as GameObject);
            Object.Instantiate(bridge1);

            //cube.AddComponent<Rigidbody>();
            cube.transform.position = new Vector3(playerposition.x + 5, 0, playerposition.z + 5); // Islands close point needed
            cube.transform.localScale = new Vector3(1, 1, 5);

            target.gameObject.SetActive(true);
            timer += 1 * Time.deltaTime;
            //StartCoroutine(FadeIn(1f, GetComponent<GameObject>()));

        }

        if (timer >= 1)
        {

            bridge2.gameObject.SetActive(true);

        }

        if (timer >= 2)
        {
            bridge3.gameObject.SetActive(true);
        }



    }


    /*IEnumerator FadeIn(float f){


		render.material.color.a = new Color (render.material.color.r, render.material.color.g, render.material.color.b, 0);

		while (render.material.color.a < 1f) {
			render.material.color.a = new Color (render.material.color.r, render.material.color.g,render.material.color.b,render.material.color.a + (Time.deltaTime / t));
			yield return null;
			}

		}*/

}
