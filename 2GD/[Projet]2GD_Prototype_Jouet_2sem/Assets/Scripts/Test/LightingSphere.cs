using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingSphere : MonoBehaviour {

    public bool isFading;
    public bool isCharging;

    public float fadingTime = 5f;
    public float fadeTimeRemaining = 0f;
    public float currentAngularVel;

    public Vector3 initRot;
    public Vector3 currentRot;

    GameObject lightChild;

    //GameManager2 script;
    CircleBehaviours.LightSphereBehaviour behaviour;

    CircleBehaviour circleScript;

	// Use this for initialization
	void Start () {
        //script = GameObject.Find("GameManager").GetComponent<GameManager2>();
        lightChild = transform.GetChild(0).gameObject;
        behaviour = CircleBehaviours.LightSphereBehaviour.lightFadesAfterXSec ;
        isFading = false;
        currentAngularVel = 0f;
        circleScript = GetComponent<CircleBehaviour>();
    }

    private void Update()
    {
        if(behaviour == CircleBehaviours.LightSphereBehaviour.lightFadesAfterXSec && isFading)
        {
            fadeTimeRemaining -= Time.deltaTime;
            if(fadeTimeRemaining <= 0f)
            {
                isFading = false;
                fadeTimeRemaining = 0f;
                lightChild.SetActive(false);
            }
        }
        if (behaviour == CircleBehaviours.LightSphereBehaviour.lightOnAfterATour && isCharging)
        {
            //currentRot = transform.rotation.eulerAngles;
            currentAngularVel += circleScript.rotateSpeed*Time.deltaTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if(behaviour == CircleBehaviours.LightSphereBehaviour.lightFadesAfterXSec)
            {
                fadeTimeRemaining = fadingTime;
                isFading = true;
                lightChild.SetActive(true);
            }
            else if(behaviour == CircleBehaviours.LightSphereBehaviour.lightOffWhenLeaving)
            {
                lightChild.SetActive(true);
            }
            else if(behaviour == CircleBehaviours.LightSphereBehaviour.lightOnAfterATour)
            {
                initRot = currentRot = transform.rotation.eulerAngles;
                isCharging = true;
                currentAngularVel = 0f;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (behaviour == CircleBehaviours.LightSphereBehaviour.lightOnAfterATour)
        {
            if(currentAngularVel >= 360f && isCharging)
            {
                lightChild.SetActive(true);
            }

        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (behaviour == CircleBehaviours.LightSphereBehaviour.lightOffWhenLeaving)
            {
                lightChild.SetActive(false);
            }
        }
        else if (behaviour == CircleBehaviours.LightSphereBehaviour.lightOnAfterATour)
        {
            if (isCharging && !lightChild.activeSelf)
            {
                currentRot = initRot = Vector3.zero;
                currentAngularVel = 0f;
            }
        }
    }

}
