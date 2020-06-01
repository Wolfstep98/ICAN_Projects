using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusGenerator : MonoBehaviour {

    #region Variables

    //Values
    public bool randomBonusDuration = true;

    public float spawnRate = 10f;
    public float bonusRadius;

    public float bonusDuration;
    public float minBonusDuration = 2f;
    public float maxBonusDuration = 10f;
    public Bonus.BonusType bonusType;

    //References
    public GameObject bonusPrefab;

    public GameManager gameManager;

    #endregion

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        bonusPrefab = Resources.Load<GameObject>("Prefabs/Final/2D/Bonus");
    }

    private void Start()
    {
        bonusRadius = bonusPrefab.GetComponent<CircleCollider2D>().radius * bonusPrefab.transform.lossyScale.x;
        StartCoroutine(GenerateBonusXSec());
    }

    public IEnumerator GenerateBonusXSec()
    {
        yield return new WaitForSeconds(spawnRate);
        while(true)
        {
            GenerateRandomBonus();
            GenerateBonus();
            yield return new WaitForSeconds(spawnRate);
        }
    }

    public void GenerateRandomBonus()
    {
        bonusDuration = (randomBonusDuration) ? Random.Range(minBonusDuration, maxBonusDuration) : bonusDuration;
        int rdmNumber = Random.Range(0, 3);
        bonusType = (Bonus.BonusType)rdmNumber;
    }

    public void GenerateBonus()
    {
        //Pick a spawn point
        Vector2 spawnPoint = PickARandomPointAroundCircles();
        //Instantiate the bonus
        GameObject bonus = GameObject.Instantiate<GameObject>(bonusPrefab, spawnPoint, Quaternion.identity); //Insert the spawn point
        bonus.GetComponent<Bonus>().Init(bonusDuration, bonusType);
        Debug.Log("Created a bonus for " + bonusDuration + " with the type : " + bonusType.ToString());
    }

    public Vector2 PickARandomPointAroundCircles()
    {
        //Pick a random circle
        int circleRdmIndex = Random.Range(0, gameManager.circles.Length);
        GameObject circle = gameManager.circles[circleRdmIndex];
        Vector2 spawnPoint = PickRandomPointAroundCircle(circle);
        return spawnPoint;
    }

    public Vector2 PickRandomPointAroundCircle(GameObject circle)
    {
        float rdmAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Vector2 circlePos = circle.transform.position;
        float circleRadius = circle.GetComponent<CircleCollider2D>().radius * circle.transform.lossyScale.x;
        Vector2 spawnPoint = new Vector2(
            circlePos.x + Mathf.Cos(rdmAngle) * (circleRadius + bonusRadius),
            circlePos.y + Mathf.Sin(rdmAngle) * (circleRadius + bonusRadius));
        return spawnPoint;
    }
}
