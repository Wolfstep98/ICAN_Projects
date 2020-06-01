using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bonus : MonoBehaviour
{

    #region Variables

    //Bonus time
    public float bonusDuration;

    //Enums
    public enum BonusType
    {
        IEM,
        Shield,
        Virus
    }
    public BonusType bonusType;

    //References
    public GameManager gameManager;
    public LienUpdater lienUpdater;


    #endregion

    public void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        lienUpdater = GameObject.FindGameObjectWithTag("GameManager").GetComponent<LienUpdater>();
    }

    public void Init(float _bonusDuration, BonusType bonus)
    {
        bonusDuration = _bonusDuration;
        bonusType = bonus;
        SetSprite();
    }

    private void SetSprite()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        switch(bonusType)
        {
            case BonusType.IEM:
                spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/MineIEM");
                break;
            case BonusType.Shield:
                spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/Bouclier");
                break;
            case BonusType.Virus:
                spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/Virus");
                break;
            default:
                break;
        }
    }

    public void ActivateIEM()
    {
        foreach(GameObject lien in lienUpdater.liens)
        {
            Lien link = lien.GetComponent<Lien>();
            if(lien.activeSelf)
                link.DeactivateLink(bonusDuration);
        }
        Debug.Log("IEM activated");
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

}
