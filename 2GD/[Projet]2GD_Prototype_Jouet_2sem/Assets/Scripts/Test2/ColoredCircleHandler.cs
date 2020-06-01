using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ColoredCircleHandler : MonoBehaviour {

    [Header("Properties")]
    public bool bonus;
    public float valueAmount = 0.1f;

    public GameObject lien;

    public GameObject[] circles;
    public Color[] currentColor;
    public Color[] previousColor;
    public GameObject[] players;

    Dictionary<GameObject, GameObject> playersCircleColored;
    public Dictionary<KeyValuePair<GameObject, GameObject>, GameObject> lienBetweenCircles;

	// Use this for initialization
	void Start () {
        circles = GameObject.FindGameObjectsWithTag("Circle");
        players = GameObject.FindGameObjectsWithTag("Player");
        currentColor = new Color[circles.Length];
        previousColor = new Color[circles.Length];
        for( int i = 0; i < circles.Length; i++)
        {
            currentColor[i] = previousColor[i] = circles[i].GetComponent<Renderer>().material.color;
        }
        playersCircleColored = new Dictionary<GameObject, GameObject>(circles.Length);
        for (int i = 0; i < circles.Length; i++) 
        {
            playersCircleColored.Add(circles[i], null);
        }
        lienBetweenCircles = new Dictionary<KeyValuePair<GameObject, GameObject>, GameObject>();
        for(int i = 0; i < circles.Length;i++)
        {
            foreach(GameObject circle in circles[i].GetComponent<CircleBehaviours>().circleLinked)
            {
                GameObject lienInstance = Instantiate(lien);
                lienInstance.GetComponent<LineRenderer>().SetPositions(new Vector3[] { circles[i].transform.position, circle.transform.position });
                EdgeCollider2D lienCollider = lienInstance.GetComponent<EdgeCollider2D>();
                lienCollider.points = new Vector2[] { circles[i].transform.position, circle.transform.position };
                lienInstance.SetActive(false);
                lienBetweenCircles.Add(new KeyValuePair<GameObject, GameObject>(circles[i], circle),lienInstance);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < circles.Length; i++)
        {
            currentColor[i] = circles[i].GetComponent<Renderer>().material.color;
            if(currentColor[i] != previousColor[i])
            {
                for(int j = 0; j < players.Length; j++)
                {
                    if(players[j] != null && players[j].GetComponent<PlayerBehaviours>().circleInCollision == circles[i])
                    {
                        if (playersCircleColored[circles[i]] == null)
                        {
                            playersCircleColored[circles[i]] = players[j];
                            players[j].GetComponent<PlayerBehaviours>().maxSpeed *= 1 + ((bonus) ? valueAmount : -valueAmount);
                        }
                        else if(playersCircleColored[circles[i]] == players[0])
                        {
                            players[0].GetComponent<PlayerBehaviours>().maxSpeed *= 1 - ((bonus) ? valueAmount : -valueAmount);
                            playersCircleColored[circles[i]] = players[j];
                            players[j].GetComponent<PlayerBehaviours>().maxSpeed *= 1 + ((bonus) ? valueAmount : -valueAmount);
                        }
                        else if (playersCircleColored[circles[i]] == players[1])
                        {
                            players[1].GetComponent<PlayerBehaviours>().maxSpeed *= 1 - ((bonus) ? valueAmount : -valueAmount);
                            playersCircleColored[circles[i]] = players[j];
                            players[j].GetComponent<PlayerBehaviours>().maxSpeed *= 1 + ((bonus) ? valueAmount : -valueAmount);
                        }
                    }
                }
            }
               
        }
        for (int i = 0; i < currentColor.Length; i++)
        {
            previousColor[i] = currentColor[i];
        }
        Dictionary<KeyValuePair<GameObject,GameObject>,GameObject>.KeyCollection keys = lienBetweenCircles.Keys;
        foreach(KeyValuePair<GameObject,GameObject> key in keys)
        {
            if(key.Key.GetComponent<Renderer>().material.color == key.Value.GetComponent<Renderer>().material.color)
            {

            }
            else
            {
                lienBetweenCircles[key].SetActive(false);
            }
        }
    }

    public void CreateLink(GameObject joueur, GameObject circle, GameObject circleToLink)
    {
        CircleBehaviours circleBehaviour = circle.GetComponent<CircleBehaviours>();
        CircleBehaviours circleToLinkBehaviour = circleToLink.GetComponent<CircleBehaviours>();
        //GameObject[] linkedCircle = circle.GetComponent<CircleBehaviours>().circleLinked;
        //Dictionary<GameObject,float> linkedCircleDist =  circle.GetComponent<CircleBehaviours>().circleLinkedDist;

        if(circleBehaviour.nbrDeLien < circleBehaviour.nbrDeLienMax && circleToLinkBehaviour.nbrDeLien < circleToLinkBehaviour.nbrDeLienMax)
        {
            KeyValuePair<GameObject, GameObject> key = new KeyValuePair<GameObject, GameObject>(circle, circleToLink);
            GameObject lien = lienBetweenCircles[key];
            lien.SetActive(true);
            lien.name = "Lien" + joueur.GetComponent<PlayerBehaviours>().player.ToString();
            lien.GetComponent<Renderer>().material.color = joueur.GetComponent<PlayerBehaviours>().playerColor;
            lien.GetComponent<LienBehaviours>().ChangePlayerLinked(joueur);
            Debug.Log("Lien linked" + lien.name);
            circleBehaviour.nbrDeLien++;
            circleToLinkBehaviour.nbrDeLien++;
        }
        else
        {
            Color colorNeeded =  joueur.GetComponent<PlayerBehaviours>().playerColor;
            foreach (GameObject lien in lienBetweenCircles.Values)
            {
                if (lien.GetComponent<Renderer>().material.GetColor("_Color") == colorNeeded) 
                    lien.SetActive(false);
            }
            foreach(KeyValuePair<GameObject,GameObject> obj in lienBetweenCircles.Keys)
            {
                if (obj.Key.GetComponent<CircleBehaviours>().currentColor == colorNeeded && obj.Value.GetComponent<CircleBehaviours>().currentColor == colorNeeded)
                {
                    obj.Key.GetComponent<CircleBehaviours>().nbrDeLien = 0;
                    obj.Value.GetComponent<CircleBehaviours>().nbrDeLien = 0;
                }
            }
            GameObject[] circles = GameObject.Find("GameManager").GetComponent<GameManager2>().circles;
            Dictionary<KeyValuePair<GameObject, GameObject>, float> distBetweenCircles = new Dictionary<KeyValuePair<GameObject, GameObject>, float>();
            foreach(GameObject circleL in circles)
            {
                if(circleL.GetComponent<CircleBehaviours>().currentColor == colorNeeded)
                {
                    CircleBehaviours circleLBehaviour = circleL.GetComponent<CircleBehaviours>();
                    foreach(GameObject circleLLinked in circleLBehaviour.circleLinked)
                    {
                        if(circleLLinked.GetComponent<CircleBehaviours>().currentColor == colorNeeded)
                        {
                            if(!distBetweenCircles.ContainsKey(new KeyValuePair<GameObject, GameObject>(circleL, circleLLinked))&& !distBetweenCircles.ContainsKey(new KeyValuePair<GameObject, GameObject>(circleLLinked, circleL)))
                                distBetweenCircles.Add(new KeyValuePair<GameObject, GameObject>(circleL, circleLLinked), circleL.GetComponent<CircleBehaviours>().circleLinkedDist[circleLLinked]);
                        }
                    }

                }
            }
            IOrderedEnumerable< KeyValuePair < KeyValuePair<GameObject, GameObject>, float>> distBetweenCirclesSorted = distBetweenCircles.OrderBy(value => value.Value);
            Dictionary<KeyValuePair<GameObject, GameObject>, float>.KeyCollection keys = distBetweenCircles.Keys;
            foreach(KeyValuePair<KeyValuePair<GameObject,GameObject>,float> key in distBetweenCirclesSorted)
            {
                GameObject circle1 = key.Key.Key;
                GameObject circle2 = key.Key.Value;
                Debug.Log(distBetweenCircles[key.Key]);
                if(circle1.GetComponent<CircleBehaviours>().nbrDeLien < circle1.GetComponent<CircleBehaviours>().nbrDeLienMax
                    && circle2.GetComponent<CircleBehaviours>().nbrDeLien < circle2.GetComponent<CircleBehaviours>().nbrDeLienMax)
                {
                    KeyValuePair<GameObject, GameObject> cle = new KeyValuePair<GameObject, GameObject>(circle1, circle2);
                    GameObject lien = lienBetweenCircles[cle];
                    lien.SetActive(true);
                    lien.name = "Lien" + joueur.GetComponent<PlayerBehaviours>().player.ToString();
                    lien.GetComponent<Renderer>().material.color = joueur.GetComponent<PlayerBehaviours>().playerColor;
                    lien.GetComponent<LienBehaviours>().ChangePlayerLinked(joueur);
                    Debug.Log("Lien linked" + lien.name);
                    circle1.GetComponent<CircleBehaviours>().nbrDeLien++;
                    circle2.GetComponent<CircleBehaviours>().nbrDeLien++;
                }
                else
                {
                    KeyValuePair<GameObject, GameObject> cle = new KeyValuePair<GameObject, GameObject>(circle1, circle2);
                    if (lienBetweenCircles[cle].activeSelf)
                    {
                        lienBetweenCircles[cle].SetActive(false);
                    }
                }
            }
        }
    }
}
