using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LienUpdater : MonoBehaviour {

    #region Variables

    [Header("LienUpdater properties")]

    //Values
    public bool bonus = true;
    public float valueAmount = 0.05f;

    //Prefabs
    public GameObject lienPrefab;

    //References
    public GameManager gameManager;
    public GameObject[] circles;
    public GameObject[] players;
    public GameObject[] liens;
    public Color[] currentColor;
    public Color[] previousColor;

    #endregion


    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        lienPrefab = Resources.Load<GameObject>("Prefabs/Test/Lien2D");   
    }

    // Use this for initialization
    void Start () {
        circles = GameObject.FindGameObjectsWithTag("Circle");
        players = GameObject.FindGameObjectsWithTag("Player");

        //Instanciate the links
        for (int i = 0; i < circles.Length; i++)
        {
            List<GameObject> links = new List<GameObject>();
            foreach (GameObject circle in circles[i].GetComponent<Circle>().circlesLinked)
            {
                GameObject lienInstance = Instantiate(lienPrefab);
                Link link = new Link(lienInstance, circle, circles[i]);
                Lien lienBehaviour = lienInstance.GetComponent<Lien>();
                EdgeCollider2D lienCollider = lienInstance.GetComponent<EdgeCollider2D>();
                gameManager.links.Add(link);
                lienBehaviour.Init(circle, circles[i], link);
                lienInstance.GetComponent<LineRenderer>().SetPositions(new Vector3[] { circles[i].transform.position, circle.transform.position });
                lienCollider.points = new Vector2[] { circles[i].transform.position, circle.transform.position };
                links.Add(lienInstance);
                //lienBetweenCircles.Add(new KeyValuePair<GameObject, GameObject>(circles[i], circle), lienInstance);
            }
            GameObject[] linksArray = links.ToArray();
            circles[i].GetComponent<Circle>().links = linksArray;
        }
        liens = GameObject.FindGameObjectsWithTag("Lien");
        foreach(GameObject lien in liens)
        {
            lien.SetActive(false);
        }
    }

    public void FindCirclesWithSameColor(List<GameObject> circles, List<Link> links)
    {
        foreach (GameObject circle in circles)
        {
            foreach (GameObject circleLinked in circle.GetComponent<Circle>().circlesLinked)
            {
                if (!circle.GetComponent<Circle>().isBonus && !circleLinked.GetComponent<Circle>().isBonus)
                {
                    if (circle.GetComponent<Circle>().color == circleLinked.GetComponent<Circle>().color
                    && circles.Find(circleParam =>
                    {
                        if (circleParam == circle)
                            return false;
                        return circleLinked == circleParam;
                    })
                    && !links.Contains(links.Find(link =>
                    {
                        if ((link.circle1 == circle || link.circle1 == circleLinked)
                            && (link.circle2 == circle || link.circle2 == circleLinked))
                            return true;
                        else
                            return false;
                    })))
                    {
                        links.Add(gameManager.links.Find(link =>
                        {
                            if ((link.circle1 == circle || link.circle1 == circleLinked)
                                && (link.circle2 == circle || link.circle2 == circleLinked))
                                return true;
                            else
                                return false;
                        }));
                    }
                }
                else
                {
                    if(!links.Contains(links.Find(link =>
                    {
                        if ((link.circle1 == circle || link.circle1 == circleLinked)
                            && (link.circle2 == circle || link.circle2 == circleLinked))
                            return true;
                        else
                            return false;
                    })))
                    {
                        links.Add(gameManager.links.Find(link =>
                        {
                            if ((link.circle1 == circle || link.circle1 == circleLinked)
                                && (link.circle2 == circle || link.circle2 == circleLinked))
                                return true;
                            else
                                return false;
                        }));
                    }
                }
            }
        }
    }

    public void ActivateLinks(List<Link> links, Player.PlayerNumber playerNumber)
    {
        foreach(Link link in links)
        {
            Circle circle1Script = link.circle1.GetComponent<Circle>();
            Circle circle2Script = link.circle2.GetComponent<Circle>();
            if(true)//circle1Script.nbrDeLien < circle1Script.NbrDeLienMax
                //&& circle2Script.nbrDeLien < circle2Script.NbrDeLienMax)
            {
                Player player = gameManager.FindPlayer(playerNumber).GetComponent<Player>();
                //Get the good color
                //Color color = new Color();
                GameManager.GameColor color = (circle1Script.color != GameManager.GameColor.White) ? circle1Script.color : circle2Script.color;
                Player.PlayerNumber number;
                if (circle1Script.currentPlayer == null && circle2Script.currentPlayer != null && !circle2Script.isBonus)
                    number = circle2Script.currentPlayer.GetComponent<Player>().player;
                else if (circle2Script.currentPlayer == null && circle1Script.currentPlayer != null && !circle1Script.isBonus)
                    number = circle1Script.currentPlayer.GetComponent<Player>().player;
                else if ((circle1Script.currentPlayer == null && circle2Script.currentPlayer == null) || (circle1Script.isBonus && circle2Script.isBonus))
                    number = player.player;
                else
                    number = player.player; //(circle1Script.isBonus) ? circle2Script.currentPlayer.GetComponent<Player>().player : circle1Script.currentPlayer.GetComponent<Player>().player;
                //Activate the link
                link.lien.SetActive(true);
                link.lien.GetComponent<Lien>().playerLinked = number;
                link.lien.GetComponent<Lien>().color = color;
                link.lien.gameObject.GetComponent<Renderer>().material.color = gameManager.GameColorToColor(color);
                circle1Script.nbrDeLien++;
                circle2Script.nbrDeLien++;
            }
        }
    }

    public void UpdateLinks()
    {
        //Reset all links and nbrDeLien
        foreach (Link link in gameManager.links)
        {
            link.lien.SetActive(false);
            link.circle1.GetComponent<Circle>().nbrDeLien = 0;
            link.circle2.GetComponent<Circle>().nbrDeLien = 0;
        }

        List<GameObject> circlesJ1 = new List<GameObject>();
        List<GameObject> circlesJ2 = new List<GameObject>();
        List<GameObject> circlesJ3 = new List<GameObject>();
        List<GameObject> circlesJ4 = new List<GameObject>();
        List<Link> linksJ1 = new List<Link>();
        List<Link> linksJ2 = new List<Link>();
        List<Link> linksJ3 = new List<Link>();
        List<Link> linksJ4 = new List<Link>();

        //Get the circles with the same color as the players color
        foreach (GameObject player in gameManager.players)
        {
            Player currentPlayer = player.GetComponent<Player>();
            if (currentPlayer == null)
                continue;
            Dictionary<GameObject, bool>.KeyCollection keys = currentPlayer.circlesWithSameColor.Keys;
            foreach (GameObject key in keys)
            {
                if (currentPlayer.circlesWithSameColor[key])
                {
                    if (currentPlayer.player == Player.PlayerNumber.J1)
                    {
                        circlesJ1.Add(key);
                    }
                    else if (currentPlayer.player == Player.PlayerNumber.J2)
                    {
                        circlesJ2.Add(key);
                    }
                    else if(currentPlayer.player == Player.PlayerNumber.J3)
                    {
                        circlesJ3.Add(key);
                    }
                    else if(currentPlayer.player == Player.PlayerNumber.J4)
                    {
                        circlesJ4.Add(key);
                    }
                }
            }
        }


        //Get links for J1
        FindCirclesWithSameColor(circlesJ1, linksJ1);
        //Get links for J2
        FindCirclesWithSameColor(circlesJ2, linksJ2);
        //Get links for J3
        FindCirclesWithSameColor(circlesJ3, linksJ3);
        //Get links for J4
        FindCirclesWithSameColor(circlesJ4, linksJ4);

        linksJ1.Sort(delegate (Link a, Link b)
        {
            if (a.distance == b.distance)
                return 0;
            if (a.distance > b.distance)
                return 1;
            else
                return -1;
        });
        linksJ2.Sort(delegate (Link a, Link b)
        {
            if (a.distance == b.distance)
                return 0;
            if (a.distance > b.distance)
                return 1;
            else
                return -1;
        });
        linksJ3.Sort(delegate (Link a, Link b)
        {
            if (a.distance == b.distance)
                return 0;
            if (a.distance > b.distance)
                return 1;
            else
                return -1;
        });
        linksJ4.Sort(delegate (Link a, Link b)
        {
            if (a.distance == b.distance)
                return 0;
            if (a.distance > b.distance)
                return 1;
            else
                return -1;
        });

        ActivateLinks(linksJ1, Player.PlayerNumber.J1);
        ActivateLinks(linksJ2, Player.PlayerNumber.J2);
        ActivateLinks(linksJ3, Player.PlayerNumber.J3);
        ActivateLinks(linksJ4, Player.PlayerNumber.J4);
    }
}
