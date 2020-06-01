using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoiteExpulsion : MonoBehaviour {

    public GameObject cube;
    public GameObject sphere;
    public GameObject capsule;
    public GameObject cylindre;

    public GameObject[] objStocke;

    public const float puissanceMin = 1f;
    public float puissance = 10f;
    public const float puissanceMax = 10000f;

    // Use this for initialization
    void Start () {
        cube.SetActive(false);
        sphere.SetActive(false);
        capsule.SetActive(false);
        cylindre.SetActive(false);
        objStocke = new GameObject[20];
        for(int i = 0; i < objStocke.Length;i++)
        {
            int nbrRandom = Random.Range(0, 4);
            if(nbrRandom == 0)
            {
                objStocke[i] = Instantiate(cube);
                objStocke[i].SetActive(false);
            }
            else if(nbrRandom == 1)
            {
                objStocke[i] = Instantiate(sphere);
                objStocke[i].SetActive(false);
            }
            else if(nbrRandom == 2)
            {
                objStocke[i] = Instantiate(capsule);
                objStocke[i].SetActive(false);
            }
            else if (nbrRandom == 3)
            {
                objStocke[i] = Instantiate(cylindre);
                objStocke[i].SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (puissance < puissanceMin)
            puissance = puissanceMin;
        if (puissance > puissanceMax)
            puissance = puissanceMax;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < objStocke.Length; i++)
            {
                GameObject newObj = Instantiate(objStocke[i], transform.position, Quaternion.Euler(0, 0, 0));
                newObj.SetActive(true);
                objStocke[i] = null;
            }
        }
        if(Input.anyKeyDown)
        {
            if(Input.inputString.Length == 1 
                && Input.inputString[0] > '0'
                && Input.inputString[0] <= '9')
            {
                int nbrASpawn = Input.inputString[0] - '0';
                for(int i = 0; i < nbrASpawn; i++)
                {
                    GameObject newObj = Instantiate(objStocke[i], transform.position + new Vector3(Random.Range(-1f,1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)), Quaternion.Euler(0, 0, 0));
                    newObj.SetActive(true);
                    newObj.GetComponent<Rigidbody>().AddExplosionForce(puissance, transform.position, 10f);
                }
            }
        }
    }
}
