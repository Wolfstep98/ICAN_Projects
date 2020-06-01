using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))]
public class Lien : MonoBehaviour {

    #region Variables

    [Header("Lien properties")]
    //Status
    public bool isActive;
    public bool isDeactivatedByEvent;
    public float deactivationTime = 2f;
    [ReadOnly]
    public float deactivationTimeLeft;

    //Color
    //public Color color;
    public GameManager.GameColor color;

    //References
    public Link link;
    public Player.PlayerNumber playerLinked;
    public GameObject[] circlesLinked;

    #endregion

    public void Init(GameObject circle1, GameObject circle2, Link link)
    {
        playerLinked = Player.PlayerNumber.None;
        circlesLinked = new GameObject[2];
        circlesLinked[0] = circle1;
        circlesLinked[1] = circle2;
        color = GameManager.GameColor.White;
        this.link = link;
        isActive = true;
        isDeactivatedByEvent = false;
        name = "lien : (" + circle1.name + "," + circle2.name + ")";
    }

    private void Update()
    {
        if(isDeactivatedByEvent)
        {
            deactivationTimeLeft -= Time.deltaTime;
            if(deactivationTimeLeft <= 0f)
            {
                ActivateLink();
            }
        }
    }

    public void DeactivateLink(float deactivateTime)
    {
        isDeactivatedByEvent = true;
        gameObject.GetComponent<Renderer>().enabled = false;
        gameObject.GetComponent<Collider2D>().enabled = false;
        this.deactivationTime = deactivateTime;
        deactivationTimeLeft = deactivationTime;
    }

    public void ActivateLink()
    {
        isDeactivatedByEvent = false;
        gameObject.GetComponent<Renderer>().enabled = true;
        gameObject.GetComponent<Collider2D>().enabled = true;
        deactivationTimeLeft = 0f;
    }
}

//Link class, which defines an in-game link between 2 gameobjects
[System.Serializable]
public class Link
{
    //----Attributs----

    //Instance
    public Link instance;

    //Values
    public float distance;

    //References
    public GameObject lien;
    public GameObject circle1;
    public GameObject circle2;

    //Constructor
    public Link(GameObject lien, GameObject circle1, GameObject circle2)
    {
        instance = this;
        this.lien = lien;
        this.circle1 = circle1;
        this.circle2 = circle2;
        distance = CalculateLinkLenghtBetween2GO(circle1, circle2);
    }

    //----Methods----
    private float CalculateLinkLenghtBetween2GO(GameObject a, GameObject b)
    {
        float distanceBetweenCircles = Vector2.Distance(a.transform.position, b.transform.position);
        float aColliderRadius = a.GetComponent<CircleCollider2D>().radius * a.transform.lossyScale.x;
        float bColliderRadius = b.GetComponent<CircleCollider2D>().radius * b.transform.lossyScale.x;
        distanceBetweenCircles -= (aColliderRadius + bColliderRadius);
        return distanceBetweenCircles;
    }
}