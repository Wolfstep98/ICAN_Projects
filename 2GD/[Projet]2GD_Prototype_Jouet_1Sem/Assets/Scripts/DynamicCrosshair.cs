using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicCrosshair : MonoBehaviour {

    public Sprite crosshairModeRayon;
    public Sprite crosshairMode360;

    public Image imageInstance;
    public Color imageColorInstance;

    private void Start()
    {
        imageInstance = GetComponent<Image>();
        imageColorInstance = imageInstance.color;
    }

    public void ChangeCrosshair(Boite obj)
    {
        imageInstance.sprite = (obj.viseeATM == Boite.ModeVisee.Rayon) ? crosshairModeRayon : crosshairMode360;
    }

    public void ChangeCrosshairOpacity(float value)
    {
        imageColorInstance.a = value;
        imageInstance.color = imageColorInstance;
    }


}
