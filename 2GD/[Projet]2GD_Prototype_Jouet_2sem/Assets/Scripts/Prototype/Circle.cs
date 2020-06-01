using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

[RequireComponent(typeof(CircleCollider2D))]
public class Circle : MonoBehaviour {

    #region Variables
    [Header("Circle properties")]
    //Circle type
    public bool isBonus = false;
    public bool isSpawn = false;
    public bool isDiscolored = false;
    public bool canBeDiscolored = false;

    //Spawn properties
    public Player.PlayerNumber playerSpawn;
    
    //Bonus properties
    public enum BonusType
    {
        None,
        Hack,
        Shield,
        Boost
    }

    //Is bonus active
    public bool bonusActive;

    //Links of the players active
    public Dictionary<Player.PlayerNumber, int> playerLinks;

    //Bonus pourcentage
    public Dictionary<int, float> boostChancePerLinks;
    public Dictionary<int, float> hackChancePerLinks;
    public float shieldCooldown;

    //The bonus type
    public BonusType bonusType;

    //Bonus sprites references
    public SpriteRenderer bonusSpriteRenderer;
    //With new sprites
    public SpriteRenderer bonusExtSpriteRenderer;
    public SpriteRenderer bonusIntSpriteRenderer;

    //Bonus Sprites 
    public Sprite shieldSprite;
    public Sprite IEMSprite;
    public Sprite boostSprite;

    //Reference
    public Dictionary<GameObject,bool> playersWithBonus;

    //Rotation variables
    public bool rotateClockwise;
    public float rotateSpeed = 200f;
    public Vector3 initRot;
    public Vector3 currentRot;

    //Current sprite color
    //public Color color;
    public GameManager.GameColor color;

    //Liens 
    public int NbrDeLienMax = 2;
    [ReadOnly]
    public int nbrDeLien = 0;

    //Discolorization
    public const float timeBetweenColorAndDiscolorization = 1f;
    [ReadOnly]
    public float currentTimeBetweenColorAndDiscolorization;

    [Header("References")]
    private GameObject circleSpriteInstance;
    private SpriteRenderer circleSpriteRend;

    //private Circle instance;
    private GameManager gameManager;
    private LienUpdater lienUpdater;

    //Circle linked
    public GameObject[] circlesLinked;
    public GameObject[] links;

    //Player
    public GameObject currentPlayer;
    [ReadOnly]
    public int nbrOfPlayerOnCircle;

    //Animations
    public Animator animator;
    [ReadOnly]
    public int playerCollisionAnimParam;
    [ReadOnly]
    public int circleColorAnimParam;
    [ReadOnly]
    public int circleDiscoloredAnimParam;
    [ReadOnly]
    public int circleCanBeDiscoloredAnimParam;

    #endregion

    private void Awake()
    {
        //Assign the references
        //instance = this;
        circleSpriteInstance = transform.parent.gameObject;
        circleSpriteRend = circleSpriteInstance.GetComponent<SpriteRenderer>();
        GameObject gameManagerGO = GameObject.FindGameObjectWithTag("GameManager");
        gameManager = gameManagerGO.GetComponent<GameManager>();
        lienUpdater = gameManagerGO.GetComponent<LienUpdater>();

        //Set up animator 
        animator = circleSpriteInstance.GetComponent<Animator>();
        //Set animator parameters
        playerCollisionAnimParam = Animator.StringToHash("PlayerCollision");
        circleColorAnimParam = Animator.StringToHash("Color");
        circleDiscoloredAnimParam = Animator.StringToHash("isDiscolored");
        circleCanBeDiscoloredAnimParam = Animator.StringToHash("canBeDiscolored");

        //Bonus
        if (isBonus)
        {
            bonusSpriteRenderer = circleSpriteInstance.transform.Find("BonusSprite").GetComponent<SpriteRenderer>();
            bonusExtSpriteRenderer = circleSpriteRend;
            bonusIntSpriteRenderer = circleSpriteInstance.transform.Find("CircleSpriteBonusInt").GetComponent<SpriteRenderer>();
        }
    }

    // Use this for initialization
    private void Start ()
    {
        //Initialisation
        rotateClockwise = false;
        isDiscolored = false;
        canBeDiscolored = false;
        currentTimeBetweenColorAndDiscolorization = 0f;
        initRot = Vector3.zero;
        currentRot = Vector3.zero;
        //color = Color.white;
        color = GameManager.GameColor.White;
        if (isBonus)
        {
            bonusActive = false;
            playersWithBonus = new Dictionary<GameObject, bool>();
            foreach(GameObject player in gameManager.players)
            {
                if (!playersWithBonus.ContainsKey(player))
                    playersWithBonus.Add(player, false);
            }
            playerLinks = new Dictionary<Player.PlayerNumber, int>();
            for(int i = 0; i < gameManager.numberOfPlayers;i++)
            {
                switch(i)
                {
                    case 0:
                        playerLinks.Add(gameManager.players[i].GetComponent<Player>().player, 0);
                        break;
                    case 1:
                        playerLinks.Add(gameManager.players[i].GetComponent<Player>().player, 0);
                        break;
                    case 2:
                        playerLinks.Add(gameManager.players[i].GetComponent<Player>().player, 0);
                        break;
                    case 3:
                        playerLinks.Add(gameManager.players[i].GetComponent<Player>().player, 0);
                        break;
                    default:
                        break;
                }
            }
            boostChancePerLinks = new Dictionary<int, float>()
            {
                {0, 0.0f },
                {1, 0.0f },
                {2, 0.20f},
                {3, 0.35f },
                {4, 0.5f}
            };
            hackChancePerLinks = new Dictionary<int, float>()
            {
                {0, 0.0f },
                {1, 0.0f },
                {2, 0.70f},
                {3, 0.85f },
                {4, 1.0f}
            };
            shieldCooldown = 7f;

            //Assign the references
            shieldSprite = Resources.Load<Sprite>("Sprites/Bonus/shield");
            IEMSprite = Resources.Load<Sprite>("Sprites/Bonus/iem");
            boostSprite = Resources.Load<Sprite>("Sprites/Bonus/boost");
        }
    }

    // Update is called once per frame
    private void Update ()
    {
		transform.Rotate(Vector3.forward, ((rotateClockwise) ? rotateSpeed : -rotateSpeed) * Time.deltaTime);

        //See how many ball are attached to the circle
        nbrOfPlayerOnCircle = transform.childCount;

        //Update bonus
        if (isBonus && bonusActive)
        {
            if (nbrDeLien > 0)
                UpdatePlayersLinks();
        }
        if(isDiscolored)
        {
            if(!canBeDiscolored)
            {
                currentTimeBetweenColorAndDiscolorization += Time.deltaTime;
                if(currentTimeBetweenColorAndDiscolorization >= timeBetweenColorAndDiscolorization)
                {
                    canBeDiscolored = true;
                    animator.SetBool(circleCanBeDiscoloredAnimParam, true);
                    currentTimeBetweenColorAndDiscolorization = 0f;
                }
            }
        }
    }

    public void ResetColor()
    {
        currentPlayer = null;
        color = GameManager.GameColor.White;
        animator.SetInteger(circleColorAnimParam, (int)color);
        //Update the players circle references
        foreach (GameObject player in gameManager.players)
        {
            player.GetComponent<Player>().UpdateCircleColored();
        }
        lienUpdater.UpdateLinks();
    }

    public void ChangeColor(Player player)
    {
        if (isDiscolored)
        {
            canBeDiscolored = false;
            animator.SetBool(circleCanBeDiscoloredAnimParam, false);
        }
        if (color != player.playerColor)
        {
            currentPlayer = player.gameObject;
            color = player.playerColor;
            animator.SetInteger(circleColorAnimParam, (int)color);
            player.AddCircle(gameObject);
            player.UpdateCircleColored();
            lienUpdater.UpdateLinks();
        }
    }

    public void ChangeColor(BouncingBall ball)
    {
        if (ball.player == null)
        {
            currentPlayer = null;
            color = ball.color;
            animator.SetInteger(circleColorAnimParam, (int)color);
            //Update the players circle references
            foreach (GameObject player in gameManager.players)
            {
                player.GetComponent<Player>().UpdateCircleColored();
            }
            lienUpdater.UpdateLinks();
        }
        else
        {
            currentPlayer = ball.player.gameObject;
            ChangeColor(ball.player);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && collision.contacts.Length != 0)
        {
            currentPlayer = collision.gameObject;
            //Set the circle rotation
            Vector2 normal = collision.contacts[0].normal;
            Vector2 playerVelocity = collision.gameObject.GetComponent<Player>().playerTrajectory;
            Vector2 contact = Vector2.zero;
            normal.Normalize();
            playerVelocity.Normalize();
            float sign = Mathf.Sign((normal.x - contact.x) * (playerVelocity.y - normal.y) - (normal.y - contact.y) * (playerVelocity.x - normal.x));
            rotateClockwise = (sign >= 0) ? false : true;

            //Check if the circle is the same color as the ball
            if (collision.gameObject.GetComponent<Player>().playerColor != color)
            {
                if (!isBonus)
                {
                    Player playerInCollision = collision.transform.GetComponent<Player>();
                    List<GameObject> circles = new List<GameObject>(); //List contenant les cercles qui doivent changer de couleur
                    circles.Add(gameObject);
                    if (playerInCollision.isHacking)
                    {
                        //Percent to activate the hack
                        float activateHack = Random.value;
                        if (activateHack < playerInCollision.hackingChance)
                        {
                            List<GameObject> circlesTemp = new List<GameObject>();
                            foreach (GameObject link in links)
                            {
                                Lien lien = link.GetComponent<Lien>();
                                GameObject circleLinked = GetCircleIfLinked(lien);
                                if (circleLinked != null)
                                {
                                    circlesTemp.Add(circleLinked);
                                }
                            }
                            if (circlesTemp.Count > 0)
                            {
                                int randomIndex = Random.Range(0, circlesTemp.Count);
                                circles.Add(circlesTemp[randomIndex]);
                                Debug.Log("Hack activated / " + playerInCollision.player.ToString());
                            }
                            Debug.Log("No circle linked for hack");
                        }
                    }

                    //Color the circles
                    foreach (GameObject circle in circles)
                    {
                        circle.GetComponent<Circle>().ChangeColor(playerInCollision);
                    }

                    //Update the players circle references
                    foreach (GameObject player in gameManager.players)
                    {
                        if (player != playerInCollision)
                            player.GetComponent<Player>().UpdateCircleColored();
                    }
                }
            }
        }
        else if(collision.transform.tag == "BouncingBall")
        {
            BouncingBall script = collision.gameObject.GetComponent<BouncingBall>();
            if (script.color != color)
            {
                if (!isBonus)
                {
                    ChangeColor(script);
                }
            }
        }
    }

    //Return the other circle linked if the link is On
    public GameObject GetCircleIfLinked(Lien lien)
    {
        GameObject linkedCircle = null;
        if (lien.gameObject.activeSelf && lien.isActive)
        {
            Debug.Log(lien.name + " between" + lien.circlesLinked[0].name + " and " + lien.circlesLinked[1].name);
            if (lien.circlesLinked[0] != gameObject)
            {
                linkedCircle = lien.circlesLinked[0];
            }
            else if (lien.circlesLinked[1] != gameObject)
            {
                linkedCircle = lien.circlesLinked[1];
            }
        }
        return linkedCircle;
    }

    public void UpdatePlayersLinks()
    {
        Dictionary<Player.PlayerNumber, int> temp = new Dictionary<Player.PlayerNumber, int>();
        foreach (GameObject circle in circlesLinked)
        {
            Circle script = circle.GetComponent<Circle>();
            if (script.currentPlayer != null)
            {
                Player.PlayerNumber number = script.currentPlayer.GetComponent<Player>().player;
                switch (number)
                {
                    case Player.PlayerNumber.J1:
                    case Player.PlayerNumber.J2:
                    case Player.PlayerNumber.J3:
                    case Player.PlayerNumber.J4:
                        if (!temp.ContainsKey(number))
                            temp.Add(number, 1);
                        else
                            temp[number]++;
                        break;
                    default:
                        break;
                }
            }
        }
        Dictionary<Player.PlayerNumber, int>.KeyCollection keysTemp = playerLinks.Keys;
        Player.PlayerNumber[] keys = new Player.PlayerNumber[keysTemp.Count];
        keysTemp.CopyTo(keys, 0);
        foreach (Player.PlayerNumber key in keys)
        {
            if(temp.ContainsKey(key))
                playerLinks[key] = temp[key];
            else
                playerLinks[key] = 0;
        }
        GiveBonus();
    }

    public void GiveBonus()
    {
        Dictionary<Player.PlayerNumber, int>.KeyCollection keysTemp = playerLinks.Keys;
        Player.PlayerNumber[] keys = new Player.PlayerNumber[keysTemp.Count];
        keysTemp.CopyTo(keys, 0);
        int nbrOfPlayerWithBonus = 0;
        switch (bonusType)
        {
            case BonusType.Boost:
                foreach (Player.PlayerNumber key in keys)
                {
                    Player player = gameManager.FindPlayer(key).GetComponent<Player>();
                    switch (playerLinks[key])
                    {
                        case 0:
                        case 1:
                            if (playersWithBonus[player.gameObject])
                            {
                                player.DeactivateBoost();
                                playersWithBonus[player.gameObject] = false;
                            }
                            break;
                        case 2:
                            player.ActivateBoost(boostChancePerLinks[2]);
                            playersWithBonus[player.gameObject] = true;
                            nbrOfPlayerWithBonus++;
                            break;
                        case 3:
                            player.UpdateBoost(boostChancePerLinks[3]);
                            playersWithBonus[player.gameObject] = true;
                            nbrOfPlayerWithBonus++;
                            break;
                        case 4:
                            player.UpdateBoost(boostChancePerLinks[4]);
                            playersWithBonus[player.gameObject] = true;
                            nbrOfPlayerWithBonus++;
                            break;
                        default:
                            break;
                    }
                }
                break;
            case BonusType.Hack:
                foreach (Player.PlayerNumber key in keys)
                {
                    Player player = gameManager.FindPlayer(key).GetComponent<Player>();
                    switch (playerLinks[key])
                    {
                        case 0:
                        case 1:
                            if (playersWithBonus[player.gameObject])
                            {
                                player.DeactivateHack();
                                playersWithBonus[player.gameObject] = false;
                            }
                            break;
                        case 2:
                            player.ActivateHack(hackChancePerLinks[2]);
                            playersWithBonus[player.gameObject] = true;
                            nbrOfPlayerWithBonus++;
                            break;
                        case 3:
                            player.UpdateHackChance(hackChancePerLinks[3]);
                            playersWithBonus[player.gameObject] = true;
                            nbrOfPlayerWithBonus++;
                            break;
                        case 4:
                            player.UpdateHackChance(hackChancePerLinks[4]);
                            playersWithBonus[player.gameObject] = true;
                            nbrOfPlayerWithBonus++;
                            break;
                        default:
                            break;
                    }
                }
                break;
            case BonusType.Shield:
                Player.PlayerNumber[] otherPlayer;
                Player.PlayerNumber[] players = FindPlayerWithMaxLinks(out otherPlayer);
                foreach (Player.PlayerNumber playerNumber in players)
                {
                    Player player = gameManager.FindPlayer(playerNumber).GetComponent<Player>();
                    switch (playerLinks[playerNumber])
                    {
                        case 0:
                        case 1:
                            if (playersWithBonus[player.gameObject])
                            {
                                player.DeactivateShield();
                                playersWithBonus[player.gameObject] = false;
                            }
                            break;
                        case 2:
                        case 3:
                        case 4:
                            if (!player.shieldOn && nbrOfPlayerWithBonus == 0)
                            {
                                player.ActivateShield(shieldCooldown);
                                playersWithBonus[player.gameObject] = true;
                                nbrOfPlayerWithBonus++;
                            }
                            else if(!player.shieldOn && nbrOfPlayerWithBonus > 0)
                            {
                                player.DeactivateShield();
                                playersWithBonus[player.gameObject] = false;
                            }
                            else
                            {
                                player.UpdateShieldCooldown(shieldCooldown);
                                nbrOfPlayerWithBonus++;
                            }
                                break;
                        default:
                            break;
                    }
                }
                break;
            default:
                Debug.Log("Error, no bonus type were found");
                break;
        }

        //Setup the bonus sprites color
        Dictionary<GameObject, bool>.KeyCollection playerWithBonus = playersWithBonus.Keys;
        bool colorSetup = false;
        int nbrOfPlayerWithBonusColor = 0;
        foreach (GameObject player in playerWithBonus)
        {
            //Change sprites colors
            Player script = player.GetComponent<Player>();
            Color colorPlayer = new Color();
            colorPlayer = gameManager.GameColorToColor(script.playerColor);
            switch (bonusType)
            {
                case BonusType.Shield:
                    switch (playerLinks[script.player])
                    {
                        case 2:
                        case 3:
                        case 4:
                            bonusExtSpriteRenderer.color = colorPlayer;
                            bonusIntSpriteRenderer.color = colorPlayer;
                            bonusSpriteRenderer.color = colorPlayer;
                            colorSetup = true;
                            nbrOfPlayerWithBonusColor++;
                            break;
                        default:
                            break;
                    }
                    break;
                case BonusType.Boost:
                case BonusType.Hack:
                    switch (playerLinks[script.player])
                    {
                        case 2:
                            bonusIntSpriteRenderer.color = colorPlayer;
                            if (nbrOfPlayerWithBonusColor == 0)
                            {
                                bonusSpriteRenderer.color = colorPlayer;
                                bonusExtSpriteRenderer.color = colorPlayer;
                            }
                            colorSetup = true;
                            nbrOfPlayerWithBonusColor++;
                            break;
                        case 3:
                        case 4:
                            if (nbrOfPlayerWithBonus == 1)
                                bonusIntSpriteRenderer.color = colorPlayer;
                            bonusExtSpriteRenderer.color = colorPlayer;
                            colorSetup = true;
                            bonusSpriteRenderer.color = colorPlayer;
                            nbrOfPlayerWithBonusColor++;
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }
        if (!colorSetup)
        {
            bonusExtSpriteRenderer.color = Color.white;
            bonusIntSpriteRenderer.color = Color.white;
            bonusSpriteRenderer.color = Color.white;
        }
    }

    Player.PlayerNumber[] FindPlayerWithMaxLinks(out Player.PlayerNumber[] otherPlayer)
    {
        Dictionary<Player.PlayerNumber, int>.KeyCollection keys = playerLinks.Keys;
        List<Player.PlayerNumber> players = new List<Player.PlayerNumber>();
        List<Player.PlayerNumber> otherPlayers = new List<Player.PlayerNumber>();
        int maxLinks = 0;
        foreach(Player.PlayerNumber key in keys)
        {
            if(playerLinks[key] >= maxLinks)
            {
                players.Add(key);
            }
            else
            {
                otherPlayers.Add(key);
            }
        }
        otherPlayer = otherPlayers.ToArray();
        return players.ToArray();
    }

    public void SetActive(bool activate)
    {
        bonusActive = activate;
        if(!activate)
        {
            Dictionary<GameObject, bool>.KeyCollection keysTemp = playersWithBonus.Keys;
            GameObject[] keys = new GameObject[keysTemp.Count];
            keysTemp.CopyTo(keys, 0);
            foreach(GameObject key in keys)
            { 
                if (playersWithBonus[key])
                {
                    Player player = key.GetComponent<Player>();
                    switch (player.currentBonus)
                    {
                        case BonusType.Boost:
                            player.DeactivateBoost();
                            break;
                        case BonusType.Hack:
                            player.DeactivateHack();
                            break;
                        case BonusType.Shield:
                            player.DeactivateShield();
                            break;
                        default:
                            Debug.Log("Error, no bonus type were found");
                            break;
                    }
                    playersWithBonus[key] = false;
                }
            }
        }
        bonusSpriteRenderer.sprite = null;
    }

    public void ChangeBonusType(BonusType bonusType)
    {
        this.bonusType = bonusType;
        switch (bonusType)
        {
            case BonusType.Boost:
                bonusSpriteRenderer.sprite = boostSprite;
                break;
            case BonusType.Hack:
                bonusSpriteRenderer.sprite = IEMSprite;
                break;
            case BonusType.Shield:
                bonusSpriteRenderer.sprite = shieldSprite;
                break;
            default:
                break;
        }
    }


    //Event Functions
    public void ActivateBlackout()
    {
        if (!isBonus)
        {
            isDiscolored = true;
            canBeDiscolored = true;
            animator.SetBool(circleDiscoloredAnimParam, true);
            animator.SetBool(circleCanBeDiscoloredAnimParam, true);
            currentTimeBetweenColorAndDiscolorization = 0f;
        }
    }

    public void DeactivateBlackout()
    {
        if (!isBonus)
        {
            isDiscolored = false;
            canBeDiscolored = false;
            animator.SetBool(circleDiscoloredAnimParam, false);
            animator.SetBool(circleCanBeDiscoloredAnimParam, false);
            currentTimeBetweenColorAndDiscolorization = 0f;
        }
    }

    //When this function is called, reset the color of the circle
    public void Reset()
    {
        ResetColor();
    }

}


