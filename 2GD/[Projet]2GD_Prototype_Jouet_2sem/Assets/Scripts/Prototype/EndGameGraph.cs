using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGameGraph : MonoBehaviour {

    //Score
    [ReadOnly]
    public int score;

    //Settings
    public const float HeightMax = 675.0f;

    //The player graph
    public Player player;
    public Player.PlayerNumber playerGraph = Player.PlayerNumber.None;

    //The height associated with the score
    public Dictionary<int, float> HeightByScore = null;

    //References
    public RectTransform rectTransform;
    public Image image;
    public Text scoreText;
    public Image playerSpriteUI;
    public Sprite playerSprite;

    public GameManager gameManager;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        scoreText = GetComponentInChildren<Text>();
        playerSpriteUI = transform.Find("PlayerSprite").GetComponent<Image>();

        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    private void Start()
    {
        if (HeightByScore == null)
        {
            HeightByScore = new Dictionary<int, float>();
            int maxScore = gameManager.circles.Length - gameManager.bonus.Length;
            float coef = HeightMax / maxScore;
            HeightByScore.Add(0, 55f);
            HeightByScore.Add(1, 60f);
            for (int i = 2; i <= maxScore; i++)
            {
                HeightByScore.Add(i, coef * (i + 1));
            }
        }
        SetHeight(score);
    }

    public void Initialise(Player player, int score)
    {
        this.player = player; 
        this.score = score;
        playerSprite = player.playerSprite;
    }

    //Set the height of the graph
    public void SetHeight(int score)
    {
        if (gameManager.winners.Contains(player.gameObject))
        {
            image.color = gameManager.GameColorToColor(player.playerColor);
            scoreText.color = Color.white;
        }
        else
        {
            scoreText.color = gameManager.GameColorToColor(player.playerColor);
        }
        playerSpriteUI.sprite = playerSprite;
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, HeightByScore[score]);
        scoreText.text = score.ToString();
    }
}
