using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Boite
{
    [Header("Boite")]
    //-------------
    // Attributs 
    //-------------
    #region Attributs

    [SerializeField]
    private bool m_aspire;
    public bool Aspire
    {
        get { return m_aspire; }
        set { m_aspire = value; }
    }

    [SerializeField]
    private bool m_expulse;
    public bool Expulse
    {
        get { return m_expulse; }
        set { m_expulse = value; }
    }

    public enum ModeVisee
    {
        Rayon,
        MultiDirectionnel
    }
    public ModeVisee viseeATM;

    public const float tailleRayonMin = 1f;
    public const float tailleRayonMax = 100f;

    [SerializeField]
    private float m_longueurRayon;
    public float longueurRayon
    {
        get { return m_longueurRayon; }
        set { m_longueurRayon = value; }
    }
    public const float tailleRayonMultiDirMin = 1f;
    public const float tailleRayonMultiDirMax = 100f;

    [SerializeField]
    private float m_longueurRayonMultiDir;
    public float longueurRayonMultiDir
    {
        get { return m_longueurRayonMultiDir; }
        set { m_longueurRayonMultiDir = value; }
    }

    public enum ModeUtilisation
    {
        Attirance,
        Expulsion
    }
    public ModeUtilisation utilisationATM;

    public const float forceMaximum = 2500f;

    public const float forceAttiranceMin = 1f;
    public const float forceAttiranceMax = 10000f;
    public float forceMax
    {
        get { return forceMaximum; }
    }

[SerializeField]
    private float m_forceAttirance;
    public float forceAttirance
    {
        get { return m_forceAttirance; }
        set { m_forceAttirance = value; }
    }

    public const float forceExpulsionMin = 1f;
    public const float forceExpulsionMax = 10000f;

    [SerializeField]
    private float m_forceExpulsion;
    public float forceExpulsion
    {
        get { return m_forceExpulsion; }
        set { m_forceExpulsion = value; }
    }
    [SerializeField]
    public List<Object> stockageInterne;

    //Couleur de la boite
    [SerializeField]
    private Color m_color;
    public Color color
    {
        get { return m_color; }
        set { m_color = value; }
    }
    public Color couleurNeutre;
    public Color couleurAspirationMax;
    public Color couleurExpulsionMax;
    #endregion

    //-------------
    // Methodes
    //-------------
#region Constructeur
    public Boite()
    {
        viseeATM = ModeVisee.Rayon;
        m_longueurRayon = 10f;
        m_longueurRayonMultiDir = 5f;
        utilisationATM = ModeUtilisation.Attirance;
        m_forceAttirance = 1000f;
        m_forceExpulsion = 1000f;
        stockageInterne = new List<Object>();
        m_aspire = m_expulse = false;
        couleurExpulsionMax = Color.blue; 
        couleurNeutre = Color.white;
        couleurAspirationMax = Color.red;
    }
#endregion

    public void AddForceAtt(float force)
    {
        float forceAdded = forceAttirance + force;
        if (forceAdded >= forceAttiranceMin && forceAdded <= forceAttiranceMax)
            forceAttirance = forceAdded;
        else if (forceAdded > forceAttiranceMax)
            forceAttirance = forceAttiranceMax;
        else if (forceAdded < forceAttiranceMin)
            forceAttirance = forceAttiranceMin;
    }

    public void AddForceExp(float force)
    {
        float forceAdded = forceExpulsion + force;
        if (forceAdded >= forceExpulsionMin && forceAdded <= forceExpulsionMax)
            forceExpulsion = forceAdded;
        else if (forceAdded > forceExpulsionMax)
            forceExpulsion = forceExpulsionMax;
        else if (forceAdded < forceExpulsionMin)
            forceExpulsion = forceExpulsionMin;
    }

}
