using System;
using UnityEngine;


public class CameraShake : MonoBehaviour
{

    #region Fields & Properties
    [Header("Parameters")]
    [SerializeField]
    private AnimationCurve xShakeCurve = null;
    [SerializeField]
    private AnimationCurve yShakeCurve = null;
    [SerializeField]
    private AnimationCurve zShakeCurve = null;

    [SerializeField]
    private float xShakeTime = 0.0f;
    [SerializeField]
    private float xShakeMultiplicator = 1.0f;
    [SerializeField]
    private float yShakeTime = 0.0f;
    [SerializeField]
    private float yShakeMultiplicator = 1.0f;
    [SerializeField]
    private float zShakeTime = 0.0f;
    [SerializeField]
    private float zShakeMultiplicator = 1.0f;

    [SerializeField]
    private float shakeMultiplicator = 5.0f;

    [Header("References")]
    [SerializeField]
    private Transform objToShake = null;
    #endregion

    #region Methods
    #region Initialization
    private void Awake()
    {
        this.Initialize();
    }

    private void Initialize()
    {
#if UNITY_EDITOR
        if (this.objToShake == null)
            Debug.LogError("[Missing Reference] - objToShake is missing !");
#endif
    }
    #endregion

    private void Update()
    {
        this.xShakeTime += Time.deltaTime * this.xShakeMultiplicator;
        if (this.xShakeTime > 1.0f)
            this.xShakeTime = 0.0f;
        this.yShakeTime += Time.deltaTime * this.yShakeMultiplicator;
        if (this.yShakeTime > 1.0f)
            this.yShakeTime = 0.0f;
        this.zShakeTime += Time.deltaTime * this.zShakeMultiplicator;
        if (this.zShakeTime > 1.0f)
            this.zShakeTime = 0.0f;
    }

    public void Shake()
    {
        float xRot = this.xShakeCurve.Evaluate(this.xShakeTime) * this.shakeMultiplicator;
        float yRot = this.yShakeCurve.Evaluate(this.yShakeTime) * this.shakeMultiplicator;
        float zRot = this.zShakeCurve.Evaluate(this.zShakeTime) * this.shakeMultiplicator;

        this.objToShake.localRotation = Quaternion.Euler(new Vector3(xRot, yRot, zRot));
    }

    public void Reset()
    {
        this.objToShake.localRotation = Quaternion.identity;
    }

    #endregion

    //--- First Camera Shake Test ---

    //    #region Fields & Properties
    //    [Header("Parameters")]
    //    [Header("Initialization")]
    //    [SerializeField]
    //    private bool randomSeeds = true;
    //    [SerializeField]
    //    private int xSeed = 0;
    //    [SerializeField]
    //    private int ySeed = 0;
    //    [SerializeField]
    //    private int zSeed = 0;

    //    [SerializeField]
    //    private System.Random xRandom = null;
    //    [SerializeField]
    //    private System.Random yRandom = null;
    //    [SerializeField]
    //    private System.Random zRandom = null;

    //    [Header("Settings")]
    //    [SerializeField]
    //    [Range(0.0f,1.0f)]
    //    private float trauma = 0.0f;

    //    [SerializeField]
    //    private float maxXShake = 1.0f;
    //    [SerializeField]
    //    private float maxYShake = 1.0f;
    //    [SerializeField]
    //    private float maxZShake = 1.0f;

    //    [SerializeField]
    //    private float xShakeTimeMultiplicator = 1.0f;
    //    [SerializeField]
    //    private float yShakeTimeMultiplicator = 1.0f;
    //    [SerializeField]
    //    private float xShakeTime = 0.0f;
    //    [SerializeField]
    //    private float yShakeTime = 0.0f;
    //    [SerializeField]
    //    private float maxShakeTime = 10.0f;

    //    [Header("Range")]
    //    [SerializeField]
    //    private float minXRange = -10.0f;
    //    [SerializeField]
    //    private float maxXRange = 10.0f;

    //    [SerializeField]
    //    private float minYRange = -10.0f;
    //    [SerializeField]
    //    private float maxYRange = 10.0f;

    //    [SerializeField]
    //    private float minZRange = -10.0f;
    //    [SerializeField]
    //    private float maxZRange = 10.0f;

    //    [Header("Rotation")]
    //    [SerializeField]
    //    private float xRotationMultiplicator = 1.0f;
    //    [SerializeField]
    //    private float yRotationMultiplicator = 1.0f;
    //    [SerializeField]
    //    private float zRotationMultiplicator = 1.0f;

    //    [Header("References")]
    //    [SerializeField]
    //    private Transform obj = null;
    //	#endregion

    //	#region Methods
    //	#region Intialization
    //	private void Awake()
    //	{
    //		this.Initialize();
    //	}

    //	private void Initialize()
    //	{
    //#if UNITY_EDITOR
    //        if (this.obj == null)
    //            Debug.LogError("[Missing Reference] - obj is missing !");
    //#endif

    //        if(this.randomSeeds)
    //        {
    //            xSeed = UnityEngine.Random.Range(0, int.MaxValue);
    //            ySeed = UnityEngine.Random.Range(0, int.MaxValue);
    //            zSeed = UnityEngine.Random.Range(0, int.MaxValue);
    //        }
    //        this.xRandom = new System.Random(xSeed);
    //        this.yRandom = new System.Random(ySeed);
    //        this.zRandom = new System.Random(zSeed);
    //    }
    //    #endregion

    //    private void Update()
    //    {
    //        this.xShakeTime += Time.deltaTime * this.xShakeTimeMultiplicator;
    //        if (this.xShakeTime >= 100.0f)
    //        {
    //            this.xShakeTime = 0.0f;
    //            this.UpdateMultiplicator();
    //        }
    //        this.yShakeTime += Time.deltaTime * this.yShakeTimeMultiplicator;
    //        if (this.yShakeTime >= 100.0f)
    //            this.yShakeTime = 0.0f;
    //    }

    //    private void UpdateMultiplicator()
    //    {
    //    //    this.xRotationMultiplicator = UnityEngine.Random.Range(this.minXRange,this.maxXRange);
    //    //    this.yRotationMultiplicator = UnityEngine.Random.Range(this.minYRange, this.maxYRange);
    //    //    this.zRotationMultiplicator = UnityEngine.Random.Range(this.minZRange, this.maxZRange);
    //    }

    //    public void Shake(float trauma)
    //    {
    //        float xShake = this.maxXShake * (trauma * trauma) * (Mathf.PerlinNoise(this.xShakeTime / 100.0f, this.yShakeTime / 100.0f) - 0.5f); //(float)this.xRandom.NextDouble() % (this.maxXRange - this.minXRange) + this.minXRange, (float)this.xRandom.NextDouble() % (this.maxXRange - this.minXRange) + this.minXRange);
    //        float yShake = this.maxYShake * (trauma * trauma) * (Mathf.PerlinNoise(this.xShakeTime / 100.0f, this.yShakeTime / 100.0f) - 0.5f); //(float)this.yRandom.NextDouble() % (this.maxYRange - this.minYRange) + this.minYRange, (float)this.yRandom.NextDouble() % (this.maxYRange - this.minYRange) + this.minYRange);
    //        float zShake = this.maxZShake * (trauma * trauma) * (Mathf.PerlinNoise(this.xShakeTime / 100.0f, this.yShakeTime / 100.0f) - 0.5f); //(float)this.zRandom.NextDouble() % (this.maxZRange - this.minZRange) + this.minZRange, (float)this.zRandom.NextDouble() % (this.maxZRange - this.minZRange) + this.minZRange);

    //        this.obj.rotation = this.obj.transform.parent.rotation * new Quaternion(xShake * this.xRotationMultiplicator, yShake * this.yRotationMultiplicator, zShake * this.zRotationMultiplicator, 1);
    //    }
    //	#endregion
}
