
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class FPBoiteController : MonoBehaviour {

    #region Declaration variables

    //Game Manager
    public GameManager scriptManager;

    // La boite magique
    public Boite boite;
    public GameObject boiteGO;
    public GameObject boite3D;
    public GameObject boiteTexture;
    public Color emissiveColor;
    //public Slider sliderPuissance;
    public GameObject playerUIPrefab;
    private GameObject playerUIInstance;
    public AudioSource audioSourceBoite;
    public AudioSource audioSourceBoiteLoop;
    public int layerInteractiveObj;
    public bool canRotate;

    //Feedback
    [Header("Boite Aura")]
    public GameObject aura;
    private GameObject instanceAura;

    [Header("Beam")]
    public GameObject beamStartPoint;
    public GameObject beamLine;
    public GameObject beamEndPoint;
    private GameObject instanceBeamStartPoint;
    private GameObject instanceBeamLine;
    private GameObject instanceBeamEndPoint;

    [Header("LightJump")]
    public GameObject lightJump;
    private GameObject instanceLightJump;

    [Header("Shield")]
    public GameObject shield;
    private GameObject instanceShield;

    [Header("Nova")]
    public GameObject nova;
    private GameObject instanceNova;

    //Feedback Sounds
    [Header("Sounds")]
    public AudioClip boxSwitchRay;
    public AudioClip boxSwitch360;
    public AudioClip boxObjShotSingle;
    public AudioClip boxObjShot360;
    public AudioClip boxObjEnter;
    public AudioClip boxHologramSpawn;
    public AudioClip boxLoopHologram;
    public AudioClip boxLoopRay;
    public AudioClip boxLoop360;


    [Header("Other")]
    public GameObject[] particlesSystems;

    //public GameObject prefabFeedbackAspirationRayon;
    //private GameObject instanceFeedbackAspirationRayon;
    //ParticleSystem particleSystemFB;

    //Variable utile pour la vue fps
    public bool isOnGround = false;
    public bool isRunning = false;
    public float walkSpeed = 0.18f;
    public float runSpeed = 0.45f;
    public float jumpForce = 5f;
    [Range(0f,1f)]
    public float transparenceCrosshair = 0.5f;
    public Sprite crosshair;
    public DynamicCrosshair scriptCrosshair;
    public Camera mainCamera;
    public MouseLook m_MouseLook;

    //Réglages des variables correspondant à l'aspiration
    public float puissanceRayon = 100f;
    [Range(1f,50f)]
    public float distanceRayonAspiration = 10f;
    [Range(1f,50f)]
    public float tailleRayonSphereAspiration = 5f;

    //Réglages des variables correspondant à l'expulsion
    public bool placementPrecis = false;
    public float puissanceExpulsion = 100f;
    public float nbrObjectsExpulserParVague = 10f;
    public float tempsEntreChaqueExpulsion = 0.5f;
    public float tempsAvantPlacementPrecis = 0.5f;
    public float tempsAppuiePlacement = 0f;
    public float forceRetourDeForce = 0f;
    public GameObject ghostGM = default(GameObject);
    public Transform pivotPointGO = default(Transform);
    public Material materialGO = default(Material);


    //Debug
    //public LineRenderer line;
    //public GameObject sphereDebug;
    public Vector3 visee;
    [Range(-10000f,10000f)]
    public float puissance = 1000f;

    //Gestion de la coroutine correspondant à l'expulsion
    Coroutine expulsionCoroutine = null;

    #endregion
    // Get a child thanks to his name
    private Transform GetChildWithName(GameObject gameObject, string name)
    {
        Transform child = default(Transform);
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            child = gameObject.transform.GetChild(i);
            if (child.gameObject.name == name)
            {
                return child;
            }
        }
        return child;
    }
    private bool GetChildWithName(GameObject gameObject,out Transform child, string name)
    {
        Transform childSearched = default(Transform);
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            childSearched = gameObject.transform.GetChild(i);
            if (childSearched.gameObject.name == name)
            {
                child = childSearched;
                return true;
            }
        }
        child = default(Transform);
        return false;
    }

    // Use this for initialization
    void Start () {
        //Load GameObject
        beamStartPoint = Resources.Load<GameObject>("FX/BeamLaserStartBlue");
        beamEndPoint = Resources.Load<GameObject>("FX/BeamLaserEndBlue");
        beamLine = Resources.Load<GameObject>("FX/BeamLaserBlue");
        lightJump = Resources.Load<GameObject>("FX/ModularLightJump");
        shield = Resources.Load<GameObject>("FX/ModularSparkleShield");
        nova = Resources.Load<GameObject>("FX/ModularUltimateNova");
        aura = Resources.Load<GameObject>("FX/RegenerateModular");
        boite3D = GameObject.Find("Kheopted");
        boiteTexture = GameObject.Find("pPipe10");

        //Load Sounds
        boxSwitchRay = Resources.Load<AudioClip>("Sounds/box_switch_ray");
        boxSwitch360 = Resources.Load<AudioClip>("Sounds/box_switch_360");
        boxObjShotSingle = Resources.Load<AudioClip>("Sounds/box_object_shot_single");
        boxObjShot360 = Resources.Load<AudioClip>("Sounds/box_object_shot_360");
        boxObjEnter = Resources.Load<AudioClip>("Sounds/box_object_enter");
        boxHologramSpawn = Resources.Load<AudioClip>("Sounds/box_object_hologram_spawn");
        boxLoopHologram = Resources.Load<AudioClip>("Sounds/Loops/box_object_hologram_loop");
        boxLoopRay = Resources.Load<AudioClip>("Sounds/Loops/box_object_blow_ray_loop");
        boxLoop360 = Resources.Load<AudioClip>("Sounds/Loops/box_object_blow_360_loop");

        //Set Variables
        boite = new Boite();
        boiteGO = GameObject.FindGameObjectWithTag("Boite");
        mainCamera = GetComponentInChildren<Camera>();
        m_MouseLook.Init(transform, mainCamera.transform);
        visee = boiteGO.transform.position + boiteGO.transform.forward * boite.longueurRayon;
        //line.SetPositions(new Vector3[2] { transform.position, visee });
        //line.enabled = false;
        //sphereDebug.SetActive(false);
        puissanceRayon = puissanceExpulsion = puissance;
        playerUIPrefab = Resources.Load<GameObject>("UI/PlayerUICrosshair");
        playerUIInstance = Instantiate(playerUIPrefab);
        crosshair = playerUIInstance.GetComponentInChildren<Image>().sprite;
        scriptCrosshair = playerUIInstance.GetComponentInChildren<DynamicCrosshair>();
        layerInteractiveObj = (1 << LayerMask.NameToLayer("Interaction Object"));
        //instanceFeedbackAspirationRayon = Instantiate(prefabFeedbackAspirationRayon);
        //instanceFeedbackAspirationRayon.transform.SetParent(boiteGO.transform);
        //instanceFeedbackAspirationRayon.transform.localPosition = new Vector3(-0.006434143f, 0f, 2.506f);
        //instanceFeedbackAspirationRayon.transform.localRotation = Quaternion.Euler(-180f, 0f, 0f);
        //instanceFeedbackAspirationRayon.transform.localScale = new Vector3(1f, 1f, 1f);
        //particleSystemFB = instanceFeedbackAspirationRayon.GetComponent<ParticleSystem>();
        //particleSystemFB.Stop();
        emissiveColor = boiteTexture.GetComponent<MeshRenderer>().material.GetColor("_EmissionColor");
        boite.color = emissiveColor;
        canRotate  = false;
        instanceLightJump = Instantiate(lightJump, boite3D.transform.position, Quaternion.Euler(Vector3.zero), mainCamera.transform);
        instanceLightJump.GetComponent<Transform>().localPosition = new Vector3(0.553f, -0.85f, 11.14f);
        instanceLightJump.GetComponent<Transform>().localScale = new Vector3(0.5f, 0.5f, 0.5f);
        instanceLightJump.GetComponent<Transform>().localEulerAngles = new Vector3(0, 180f, 0);
        instanceLightJump.GetComponent<ParticleSystem>().Stop();
        instanceBeamStartPoint = Instantiate(beamStartPoint, boite3D.transform.position, Quaternion.identity, boite3D.transform);
        instanceBeamLine = Instantiate(beamLine);
        instanceBeamEndPoint = Instantiate(beamEndPoint, boite3D.transform.position, Quaternion.identity, mainCamera.transform);
        instanceBeamStartPoint.GetComponent<Transform>().localPosition = new Vector3(0.0091f, 0.0327f, 0.0177f);//new Vector3(0.536f, -0.778f, 2.042f);
        instanceBeamStartPoint.GetComponent<Transform>().localScale = new Vector3(0.1f, 0.1f, 0.1f);
        instanceBeamStartPoint.SetActive(false);
        instanceBeamLine.SetActive(false);
        instanceBeamEndPoint.SetActive(false);
        instanceShield = Instantiate(shield, boite3D.transform.position, Quaternion.identity);
        instanceShield.GetComponent<Transform>().localScale = new Vector3((tailleRayonSphereAspiration * 2f) / 4f, (tailleRayonSphereAspiration * 2f) / 4f, (tailleRayonSphereAspiration * 2f) / 4f);
        instanceShield.SetActive(false);
        instanceAura = Instantiate(aura, boite3D.transform);
        instanceAura.GetComponent<Transform>().localPosition = new Vector3(0.0084f, 0.0279f, 0f);
        instanceAura.GetComponent<Transform>().localScale = new Vector3(0.05f, 0.05f, 0.05f);
        particlesSystems = new GameObject[6] { instanceBeamStartPoint, instanceBeamEndPoint, instanceLightJump, instanceShield, nova, instanceAura };
        audioSourceBoite = boiteGO.GetComponents<AudioSource>()[0];
        audioSourceBoiteLoop = boiteGO.GetComponents<AudioSource>()[1];
        scriptManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
	
	// Update is called once per frame
	void Update () {
        instanceBeamStartPoint.GetComponent<Transform>().localEulerAngles = -boite3D.transform.localEulerAngles;
        Debug.DrawLine(boite3D.transform.position, boite3D.transform.position + boite3D.transform.up,Color.black);
        Debug.DrawLine(mainCamera.transform.position, (mainCamera.transform.position + mainCamera.transform.forward * boite.longueurRayon), Color.blue);
        Debug.DrawLine(boite3D.transform.position, (mainCamera.transform.position + mainCamera.transform.forward * boite.longueurRayon) - (boite3D.transform.position - mainCamera.transform.position), Color.yellow);
        if (puissance < -boite.forceMax)
            puissance = -boite.forceMax;
            //puissance = sliderPuissance.minValue;
        if (puissance > boite.forceMax)
            puissance = boite.forceMax;
        //puissance = sliderPuissance.maxValue;
        puissanceRayon = puissanceExpulsion = puissance;
        //sliderPuissance.value = puissance;
        //Réglages des variables 
        boite.forceAttirance = puissanceRayon;
        boite.forceExpulsion = puissanceExpulsion;
        boite.longueurRayon = distanceRayonAspiration;
        boite.longueurRayonMultiDir = tailleRayonSphereAspiration;
        if (boite.forceAttirance <= -boite.forceMax* 0.2)
            forceRetourDeForce = (boite.forceAttirance / 100f);
        if (puissance >= 0)
        {
            emissiveColor = Color.Lerp(boite.couleurNeutre, boite.couleurAspirationMax, (puissance / boite.forceMax));
            boite.color = emissiveColor;
            //Change particleSystems colors
            for (int particleSystem = 0; particleSystem < particlesSystems.Length; particleSystem++)
            {
                ParticleSystem.MainModule particleMain = particlesSystems[particleSystem].GetComponent<ParticleSystem>().main;
                particleMain.startColor = emissiveColor;
                for (int i = 0; i < particlesSystems[particleSystem].transform.childCount; i++)
                {
                    Transform child = particlesSystems[particleSystem].transform.GetChild(i);
                    if (child.GetComponent<ParticleSystem>() != null)
                    {
                        ParticleSystem.MainModule main = child.GetComponent<ParticleSystem>().main;
                        main.startColor = emissiveColor;
                    }
                    if (child.GetComponent<Light>() != null)
                    {
                        Light light = child.GetComponent<Light>();
                        light.color = emissiveColor;
                    }
                }
            }
            boiteTexture.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", emissiveColor);
        }
        else if (puissance < 0)
        {
            emissiveColor = Color.Lerp(boite.couleurNeutre, boite.couleurExpulsionMax, -(puissance / boite.forceMax));
            boite.color = emissiveColor;
            //Change particleSystems colors
            for (int particleSystem = 0; particleSystem < particlesSystems.Length; particleSystem++)
            {
                ParticleSystem.MainModule particleMain = particlesSystems[particleSystem].GetComponent<ParticleSystem>().main;
                particleMain.startColor = emissiveColor;
                for (int i = 0; i < particlesSystems[particleSystem].transform.childCount; i++)
                {
                    Transform child = particlesSystems[particleSystem].transform.GetChild(i);
                    if (child.GetComponent<ParticleSystem>() != null)
                    {
                        ParticleSystem.MainModule main = child.GetComponent<ParticleSystem>().main;
                        main.startColor = emissiveColor;
                    }
                    if (child.GetComponent<Light>() != null)
                    {
                        Light light = child.GetComponent<Light>();
                        light.color = emissiveColor;
                    }
                }
            }
            boiteTexture.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", emissiveColor);
        }
        //Debug.Log(sphereDebug.transform.localScale);
        //sphereDebug.transform.localScale = new Vector3(tailleRayonSphereAspiration * 2, tailleRayonSphereAspiration * 2, tailleRayonSphereAspiration * 2);

        //Check si le crosshair est sur un objet interactible
        if(boite.viseeATM == Boite.ModeVisee.Rayon)
        {
            if(Physics.Raycast(mainCamera.transform.position,mainCamera.transform.forward, boite.longueurRayon, (1 << LayerMask.NameToLayer("Interaction Object"))))
            {
                scriptCrosshair.ChangeCrosshairOpacity(1f);
                Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.forward * boite.longueurRayon, Color.red);
            }
            else
            {
                scriptCrosshair.ChangeCrosshairOpacity(transparenceCrosshair);
                Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.forward * boite.longueurRayon, Color.red);
            }
        }

        //Changement de mode pour la boite 
        if (!boite.Aspire && ((Input.GetKey(KeyCode.Keypad1) || Input.GetKey(KeyCode.Alpha1) && boite.viseeATM != Boite.ModeVisee.Rayon) || Input.GetMouseButtonDown(2) && boite.viseeATM != Boite.ModeVisee.Rayon) && scriptManager.gameState == GameManager.gameStates.Play)
        {
            boite.viseeATM = Boite.ModeVisee.Rayon;
            scriptCrosshair.ChangeCrosshair(boite);
            canRotate = false;
            audioSourceBoite.clip = boxSwitchRay;
            audioSourceBoite.Play();
        }
        else if (!boite.Aspire && ((Input.GetKey(KeyCode.Keypad2) || Input.GetKey(KeyCode.Alpha2) && boite.viseeATM != Boite.ModeVisee.MultiDirectionnel) || Input.GetMouseButtonDown(2) && boite.viseeATM != Boite.ModeVisee.MultiDirectionnel) && scriptManager.gameState == GameManager.gameStates.Play)
        {
            boite.viseeATM = Boite.ModeVisee.MultiDirectionnel;
            scriptCrosshair.ChangeCrosshair(boite);
            canRotate = false;
            audioSourceBoite.clip = boxSwitch360;
            audioSourceBoite.Play();
        }

        //Gestion du bouton d'aspiration 
        if (Input.GetMouseButtonDown(0) && !boite.Expulse && scriptManager.gameState == GameManager.gameStates.Play)
        {
            if (boite.viseeATM == Boite.ModeVisee.Rayon)
            {
                instanceBeamStartPoint.SetActive(true);
                instanceBeamLine.SetActive(true);
                instanceBeamEndPoint.SetActive(true);
                audioSourceBoiteLoop.clip = boxLoopRay;
                audioSourceBoiteLoop.Play();
            }
            else if (boite.viseeATM == Boite.ModeVisee.MultiDirectionnel)
            {
                //sphereDebug.SetActive(false);
                instanceShield.SetActive(true);
                audioSourceBoiteLoop.clip = boxLoop360;
                audioSourceBoiteLoop.Play();
            }        
            boite.Aspire = true;
        }
        if (Input.GetMouseButton(0) && !boite.Expulse && scriptManager.gameState == GameManager.gameStates.Play)
        {
            if(boite.viseeATM == Boite.ModeVisee.Rayon)
            {
                if (boite.forceAttirance <= -2000)
                    GetComponent<Rigidbody>().velocity += (forceRetourDeForce * transform.forward)* Time.deltaTime;
                visee = mainCamera.transform.position + mainCamera.transform.forward * boite.longueurRayon;
                instanceBeamLine.GetComponent<LineRenderer>().SetPositions(new Vector3[2] { instanceBeamStartPoint.transform.position, visee });
                Ray tir = new Ray(mainCamera.transform.position, visee - mainCamera.transform.position);
                RaycastHit rayInfo = new RaycastHit();
                if (Physics.Raycast(tir, out rayInfo, boite.longueurRayon, layerInteractiveObj))
                {
                    GameObject obj = rayInfo.transform.gameObject;
                    obj.GetComponent<Rigidbody>().AddForce((transform.position - obj.transform.position).normalized * boite.forceAttirance * Time.deltaTime);
                }
                if (Physics.Raycast(tir, out rayInfo, boite.longueurRayon))
                {
                    instanceBeamLine.GetComponent<LineRenderer>().SetPosition(1, rayInfo.point);
                    instanceBeamEndPoint.transform.position = rayInfo.point;
                }
                else
                {
                    instanceBeamEndPoint.transform.position = visee;
                }
            }
            else if (boite.viseeATM == Boite.ModeVisee.MultiDirectionnel)
            {
                if (puissance >= 0)
                {
                    boite3D.GetComponent<Transform>().Rotate(boite3D.transform.up, (puissance / 10000f) * -5f, Space.World);
                }
                else if (puissance < 0)
                { 
                    boite3D.GetComponent<Transform>().Rotate(boite3D.transform.up, (puissance / 10000f) * -5f, Space.World);
                }
                //sphereDebug.transform.position = boiteGO.transform.position;
                instanceShield.transform.position = boite3D.transform.position;
                Collider[] colliders = Physics.OverlapSphere(boiteGO.transform.position, boite.longueurRayonMultiDir);
                if(colliders.Length != 0)
                {
                    foreach (Collider col in colliders)
                    {
                        if (col.gameObject.layer == LayerMask.NameToLayer("Interaction Object"))
                        {
                            GameObject obj = col.gameObject;
                            obj.GetComponent<Rigidbody>().AddForce((boiteGO.transform.position - obj.transform.position).normalized * boite.forceAttirance * Time.deltaTime);
                        }
                    }
                }
            }
        }
        if(Input.GetMouseButtonUp(0) && !boite.Expulse && scriptManager.gameState == GameManager.gameStates.Play)
        {
            if (boite.viseeATM == Boite.ModeVisee.Rayon)
            {
                //line.enabled = false;
                //particleSystemFB.Stop();
                instanceBeamStartPoint.SetActive(false);
                instanceBeamLine.SetActive(false);
                instanceBeamEndPoint.SetActive(false);
            }
            else if (boite.viseeATM == Boite.ModeVisee.MultiDirectionnel)
            {
                //sphereDebug.SetActive(false);
                instanceShield.SetActive(false);
            }
            audioSourceBoiteLoop.Stop();
            boite.Aspire = false;
        }

        //Gestion du bouton d'expulsion
        if (Input.GetMouseButtonDown(1) && boite.stockageInterne.Count != 0 && !boite.Aspire && !boite.Expulse && puissance > 0 && scriptManager.gameState == GameManager.gameStates.Play)
        {
            if (boite.viseeATM == Boite.ModeVisee.Rayon)
            {
                instanceLightJump.GetComponent<ParticleSystem>().Stop();
                tempsAppuiePlacement += Time.deltaTime;
            }
            if (boite.viseeATM == Boite.ModeVisee.MultiDirectionnel)
            {
                expulsionCoroutine = StartCoroutine(ExpulseObjetcs(nbrObjectsExpulserParVague));
            }
            boite.Expulse = true;
        }
        if(Input.GetMouseButton(1) && boite.stockageInterne.Count != 0 && !boite.Aspire && puissance > 0 && scriptManager.gameState == GameManager.gameStates.Play)
        {
            if (boite.viseeATM == Boite.ModeVisee.Rayon)
            {
                if (!placementPrecis)
                {
                    tempsAppuiePlacement += Time.deltaTime;
                    if (tempsAppuiePlacement >= tempsAvantPlacementPrecis)
                    {
                        placementPrecis = true;
                        //line.enabled = true;
                    }
                }
                else if (placementPrecis)
                {
                    visee = mainCamera.transform.position + mainCamera.transform.forward * boite.longueurRayon;
                    //line.SetPositions(new Vector3[2] { mainCamera.transform.position, visee - mainCamera.transform.forward});
                    if (ghostGM == default(GameObject))
                    {
                        ghostGM = (GameObject)GameObject.Instantiate(boite.stockageInterne[boite.stockageInterne.Count - 1], visee, Quaternion.Euler(0f, 0f, 0f));
                        ghostGM.transform.SetParent(gameObject.transform);
                        ghostGM.GetComponent<Rigidbody>().isKinematic = true;
                        for(int i = 0; i < ghostGM.transform.childCount; i++)
                        {
                            Transform transformPoint = ghostGM.transform.GetChild(i);
                            if(transformPoint.gameObject.name == "DownPoint")
                            {
                                pivotPointGO = ghostGM.transform.GetChild(i);
                                break;
                            }
                        }
                        ghostGM.GetComponent<Rigidbody>().detectCollisions = false;
                        ghostGM.SetActive(true);
                        ghostGM.name = boite.stockageInterne[boite.stockageInterne.Count - 1].name;
                        materialGO = ghostGM.GetComponent<Renderer>().material;
                        ghostGM.GetComponent<Renderer>().material = Resources.Load<Material>("materials/transparent");
                        Vector3 pos = (-pivotPointGO.localPosition * ghostGM.transform.localScale.y);
                        ghostGM.GetComponent<Transform>().position += pos;
                        audioSourceBoiteLoop.clip = boxLoopHologram;
                        audioSourceBoiteLoop.Play();
                    } 
                    Ray rayPlacement = new Ray(mainCamera.transform.position, visee - mainCamera.transform.position);
                    RaycastHit hitInfos = new RaycastHit();
                    if (Physics.Raycast(rayPlacement, out hitInfos, boite.longueurRayon))
                    {
                        Debug.DrawRay(hitInfos.point,hitInfos.normal,Color.red);
                        if (hitInfos.transform.gameObject.layer == LayerMask.NameToLayer("Interaction Object"))
                        {
                            Debug.Log("Interact");
                            Transform snapPoint1;
                            if (hitInfos.normal.y >= 0.9 && GetChildWithName(hitInfos.transform.gameObject,out snapPoint1, "UpPoint"))
                            {
                                if (Vector3.Dot(snapPoint1.up, Vector3.up) >= 0.9)
                                {
                                    ghostGM.transform.position = snapPoint1.position - (pivotPointGO.localPosition * ghostGM.transform.localScale.y);
                                    Debug.Log("Normal checked");
                                }
                                //Transform snapPoint = GetChildWithName(hitInfos.transform.gameObject, "UpPoint");
                                //ghostGM.transform.position = snapPoint1.position - (pivotPointGO.localPosition * ghostGM.transform.localScale.y);
                                //ghostGM.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                            }
                            else if(hitInfos.normal.y <= 0.1 && GetChildWithName(hitInfos.transform.gameObject, out snapPoint1, ""))
                            {
                                Debug.Log("Normal checked");
                               
                            }
                            else
                            {
                                ghostGM.transform.position = hitInfos.point;
                                ghostGM.transform.position -= (pivotPointGO.localPosition * ghostGM.transform.localScale.y);
                            }
                        }
                        else
                        {
                            ghostGM.transform.position = hitInfos.point;
                            ghostGM.transform.position -= (pivotPointGO.localPosition * ghostGM.transform.localScale.y);
                        }
                    }
                    else
                    {
                        ghostGM.transform.position = visee;
                        ghostGM.transform.position -= (pivotPointGO.localPosition * ghostGM.transform.localScale.y);
                    }
                    //ghostGM.transform.position = ((Physics.Raycast(rayPlacement, out hitInfos, boite.longueurRayon)) ? hitInfos.point : visee) - pivotPointGO.localPosition;
                }
            }
        }
        if(Input.GetMouseButtonUp(1) && boite.stockageInterne.Count != 0 && !boite.Aspire && puissance > 0 && scriptManager.gameState == GameManager.gameStates.Play)
        {
            if(boite.viseeATM == Boite.ModeVisee.Rayon)
            {
                if (!placementPrecis)
                {
                    GameObject newObject = (GameObject)GameObject.Instantiate(boite.stockageInterne[boite.stockageInterne.Count - 1], boiteGO.transform.position + boiteGO.transform.forward * 1.2f, boiteGO.transform.rotation);
                    newObject.SetActive(true);
                    newObject.name = boite.stockageInterne[boite.stockageInterne.Count - 1].name;
                    newObject.GetComponent<Rigidbody>().AddForce(boiteGO.transform.forward * boite.forceExpulsion);
                    Object.Destroy(boite.stockageInterne[boite.stockageInterne.Count - 1]);
                    boite.stockageInterne.RemoveAt(boite.stockageInterne.Count - 1);
                    tempsAppuiePlacement = 0f;
                    instanceLightJump.GetComponent<ParticleSystem>().Play();
                    audioSourceBoite.clip = boxObjShotSingle;
                    audioSourceBoite.Play();
                }
                else if(placementPrecis)
                { 
                    ghostGM.GetComponent<Rigidbody>().detectCollisions = true;
                    ghostGM.GetComponent<Renderer>().material = materialGO;
                    ghostGM.GetComponent<Rigidbody>().isKinematic = false;
                    ghostGM.transform.SetParent(default(Transform));
                    Object.Destroy(boite.stockageInterne[boite.stockageInterne.Count - 1]);
                    boite.stockageInterne.RemoveAt(boite.stockageInterne.Count - 1);
                    ghostGM = default(GameObject);
                    pivotPointGO = default(Transform);
                    materialGO = default(Material);
                    tempsAppuiePlacement = 0f;
                    placementPrecis = false;
                    audioSourceBoite.clip = boxHologramSpawn;
                    audioSourceBoite.Play();
                    audioSourceBoiteLoop.Stop();
                    //line.enabled = false;
                }
            }
            else if (boite.viseeATM == Boite.ModeVisee.MultiDirectionnel)
            {
                if(expulsionCoroutine != null)
                {
                    StopCoroutine(expulsionCoroutine);
                    expulsionCoroutine = null;
                }
            }
            boite.Expulse = false;
        }
        if (Input.GetMouseButtonUp(1) && !boite.Aspire && scriptManager.gameState == GameManager.gameStates.Play)
        {
            boite.Expulse = false;
        }
        if (Input.GetKeyDown(KeyCode.Space) && isOnGround && scriptManager.gameState == GameManager.gameStates.Play)
        {
            GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isOnGround = false;
        }
        if(Input.GetKey(KeyCode.LeftShift) && scriptManager.gameState == GameManager.gameStates.Play)
        {
            isRunning = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) && scriptManager.gameState == GameManager.gameStates.Play)
        {
            isRunning = false;
        }

        //Gestion de la puissance grâce au scrolling
        if (Input.mouseScrollDelta.y != 0 && scriptManager.gameState == GameManager.gameStates.Play)
        {
            if (placementPrecis)
            {
                ghostGM.transform.Rotate(Vector3.up, Input.mouseScrollDelta.y * 5);
            }
            else if(!boite.Aspire && !boite.Expulse)
            {
                puissance += Input.mouseScrollDelta.y * 100;
            }
        }
        if(Input.GetKeyDown(KeyCode.Escape) && !boite.Aspire && !boite.Expulse)
        {
            if (scriptManager.gameState == GameManager.gameStates.Pause)
            {
                scriptManager.SetGameState(GameManager.gameStates.Play);
                m_MouseLook.SetCursorLock(true);
            }
            else
            {
                scriptManager.SetGameState(GameManager.gameStates.Pause);
                m_MouseLook.SetCursorLock(false);
            }
        }
    }

    private void FixedUpdate()
    {
        isOnGround = false;
        m_MouseLook.LookRotation(transform, mainCamera.transform);     
        if (Input.GetKey(KeyCode.Q))
        {
            Vector3 moveDirection = -(transform.right);
            moveDirection.y = 0;
            float speed = (isRunning) ? runSpeed : walkSpeed;
            transform.Translate(moveDirection * speed, Space.World);
        }
        if (Input.GetKey(KeyCode.D))
        {
            Vector3 moveDirection = (transform.right);
            moveDirection.y = 0;
            float speed = (isRunning) ? runSpeed : walkSpeed;
            transform.Translate(moveDirection * speed, Space.World);
        }
        if (Input.GetKey(KeyCode.Z))
        {
            Vector3 moveDirection = (transform.forward);
            moveDirection.y = 0;
            float speed = (isRunning) ? runSpeed : walkSpeed;
            transform.Translate(moveDirection * speed, Space.World);
        }
        if (Input.GetKey(KeyCode.S))
        {
            Vector3 moveDirection = -(transform.forward);
            moveDirection.y = 0;
            float speed = (isRunning) ? runSpeed : walkSpeed;
            transform.Translate(moveDirection * speed, Space.World);
        }
    }
    IEnumerator ExpulseObjetcs(float nbrObjects)
    {
        while (boite.stockageInterne.Count != 0)
        {
            instanceNova = Instantiate(nova, boite3D.transform.position, Quaternion.identity, mainCamera.transform);
            Destroy(instanceNova,2);
            for (int i = 0; i < nbrObjects; i++)
            {
                if (boite.stockageInterne.Count == 0)
                {
                    boite.Expulse = false;
                    yield break;
                }
                GameObject newObject = (GameObject)GameObject.Instantiate(boite.stockageInterne[boite.stockageInterne.Count - 1], boiteGO.transform.position + boiteGO.transform.forward * 1.2f, boiteGO.transform.rotation);
                newObject.SetActive(true);
                newObject.name = boite.stockageInterne[boite.stockageInterne.Count - 1].name;
                newObject.transform.Translate(Random.Range(-1f, 1f), Random.Range(0f, 1f), Random.Range(-1f, 1f));
                newObject.GetComponent<Rigidbody>().AddExplosionForce(boite.forceExpulsion, boiteGO.transform.position, 4f);
                Object.Destroy(boite.stockageInterne[boite.stockageInterne.Count - 1]);
                boite.stockageInterne.RemoveAt(boite.stockageInterne.Count - 1);
                audioSourceBoite.clip = boxObjShot360;
                audioSourceBoite.Play();
                //Debug.Log("EXPULSION !!!");
            }
            yield return new WaitForSeconds(tempsEntreChaqueExpulsion);
        }
        boite.Expulse = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        float infoNormal_y = Mathf.RoundToInt(collision.contacts[0].normal.y);
        if(infoNormal_y == 1)
        {
            isOnGround = true;
        }   
    }

    private void OnCollisionStay(Collision collision)
    {
        float infoNormal_y = Mathf.RoundToInt(collision.contacts[0].normal.y);
        if (infoNormal_y == 1)
        {
            isOnGround = true;
        }
    }

    private void OnDestroy()
    {
        Destroy(playerUIInstance);
    }


}


