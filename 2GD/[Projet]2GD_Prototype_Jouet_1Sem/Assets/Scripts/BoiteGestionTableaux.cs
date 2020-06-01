using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoiteGestionTableaux : MonoBehaviour {

    FPBoiteController script;

    private void Start()
    {
        script = GetComponentInParent<FPBoiteController>();
    }

    private void OnTriggerEnter(Collider objet)
    {
        if(objet.gameObject.layer == LayerMask.NameToLayer("Interaction Object") && GetComponentInParent<FPBoiteController>().boite.Aspire)
        {
            //Debug.Log(PrefabUtility.GetPrefabParent(objet));
            //if (PrefabUtility.GetPrefabParent(objet)!= null)
            //{
            //    Object prefab = PrefabUtility.GetPrefabParent(objet.gameObject);
            //    prefab = objet.gameObject;
            //    GetComponentInParent<FPBoiteController>().boite.stockageInterne.Add(prefab);
            //    Debug.Log(prefab.name);
            //    Object.Destroy(objet.gameObject);
            //}
            Debug.Log(objet.gameObject);
            GetComponentInParent<FPBoiteController>().boite.stockageInterne.Add(objet.gameObject);
            objet.gameObject.SetActive(false);
            if(objet.GetComponent<SnapHandler>())
            {
                objet.GetComponent<SnapHandler>().ResetUpPoint();
            }
            if (script.boite.viseeATM == Boite.ModeVisee.Rayon)
            {
                script.audioSourceBoite.clip = script.boxObjEnter;
                script.audioSourceBoite.Play();
                script.audioSourceBoite.loop = false;
            }
        }
    }
}