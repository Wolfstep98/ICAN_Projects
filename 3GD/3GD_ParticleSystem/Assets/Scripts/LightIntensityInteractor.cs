using System;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Light))]
public class LightIntensityInteractor : MonoBehaviour
{
    #region Fields
    [Header("References")]
    [SerializeField] private Transform magicStone = null;
    [SerializeField] private new Light light = null;

    private float previousLightIntensity = 0.0f;

    [Header("Light")]
    [SerializeField, Range(0.0f, 20.0f)] private float lightIntensity = 5.0f; 

    [Header("Magic Stone")]
    [SerializeField] private Rigidbody magicStoneRigidbody = null;
    [SerializeField] private ParticleSystemForceField magicStoneParticleSystemForceField = null;

    [SerializeField] private float minSize = 0.1f;
    [SerializeField] private float maxSize = 5.0f;

    [SerializeField] private float minRotationTorqueForce = 0.0f;
    [SerializeField] private float maxRotationTorqueForce = 20.0f;

    [SerializeField] private float minForceFieldForce = 0.0f;
    [SerializeField] private float maxForceFieldForce = 4.0f;

    [Header("Flare")]
    [SerializeField] private ParticleSystem flareParticleSystem = null;

    [SerializeField] private float minDurationLifeTime = 3.0f;
    [SerializeField] private float maxDurationLifeTime = 1.0f;

    [Header("SubLight")]
    [SerializeField] private ParticleSystem subLightParticleSystem = null;

    [SerializeField] private float minRateOverTimeSubLight = 1.0f;
    [SerializeField] private float maxRateOverTimeSubLight = 10.0f;

    [Header("Small Flares")]
    [SerializeField] private ParticleSystem smallFlaresParticleSystem = null;

    [SerializeField] private float minRateOverTimeSmallFlares = 20.0f;
    [SerializeField] private float maxRateOverTimeSmallFlares = 1000.0f;

    [Header("Flying Stones")]
    [SerializeField] private ParticleSystem flyingStonesParticleSystem = null;

    [SerializeField] private float minRateOverTimeFlyingStones = 5.0f;
    [SerializeField] private float maxRateOverTimeFlyingStones = 50.0f;

    [SerializeField] private float minOrbitalZSpeed = 1.0f;
    [SerializeField] private float maxOrbitalZSpeed = 10.0f;

    [SerializeField] private Vector2 minStartSizeXCurve = Vector2.zero;
    [SerializeField] private Vector2 maxStartSizeXCurve = Vector2.zero;

    [SerializeField] private Vector2 minStartSizeYCurve = Vector2.zero;
    [SerializeField] private Vector2 maxStartSizeYCurve = Vector2.zero;

    [SerializeField] private Vector2 minStartSizeZCurve = Vector2.zero;
    [SerializeField] private Vector2 maxStartSizeZCurve = Vector2.zero;

    [Header("Trails")]
    [SerializeField] private ParticleSystem trailsParticleSystem = null;

    [SerializeField] private float minRateOverTimeTrails = 20.0f;
    [SerializeField] private float maxRateOverTimeTrails = 100.0f;

    [SerializeField] private float minStartSize = 0.2f;
    [SerializeField] private float maxStartSize = 1.0f;

    [SerializeField] private Vector2 minStartLifeTime = Vector2.zero;
    [SerializeField] private Vector2 maxStartLifeTime = Vector2.zero;

    [Header("Shine")]
    [SerializeField] private ParticleSystem shineParticleSystem = null;

    [SerializeField, Range(0.0f,1.0f)] private float minAlpha = 0.05f;
    [SerializeField, Range(0.0f, 1.0f)] private float maxAlpha = 1.0f;

    [SerializeField] private float minBurstCount = 7.0f;
    [SerializeField] private float maxBurstCount = 20.0f;

    #endregion

    #region Methods
    private void Update()
    {
        if(this.previousLightIntensity != this.lightIntensity)
        {
            this.light.intensity = this.lightIntensity;

            float lerp = this.lightIntensity / 20.0f;

            // Update Magic Stone
            this.UpdateMagicStone(lerp);

            // Updtae Flare
            this.UpdateFlare(lerp);

            //Update SubLight
            this.UpdateSubLight(lerp);

            //Update Small Flares
            this.UpdateSmallFlares(lerp);

            //Update Flying Stones
            this.UpdateFlyingStones(lerp);

            //Update Trails
            this.UpdateTrails(lerp);

            //Update Shine
            this.UpdateShine(lerp);

            this.previousLightIntensity = this.lightIntensity;
        }
    }

    private void UpdateMagicStone(float lerpValue)
    {
        // Update Stone Size
        float size = Mathf.Lerp(this.minSize, this.maxSize, lerpValue);

        this.magicStone.localScale = new Vector3(size, size, size);

        // Update Stone rotation torque
        float rotationTorqueForce = Mathf.Lerp(this.minRotationTorqueForce, this.maxRotationTorqueForce, lerpValue);

        this.magicStoneRigidbody.angularVelocity = new Vector3(0, rotationTorqueForce, 0);

        // Update force field gravity force and drag strength
        float forceFieldForce = Mathf.Lerp(this.minForceFieldForce, this.maxForceFieldForce, lerpValue);

        ParticleSystem.MinMaxCurve gravityCurve = this.magicStoneParticleSystemForceField.gravity;
        ParticleSystem.MinMaxCurve dragCurve = this.magicStoneParticleSystemForceField.drag;
        gravityCurve.constant = forceFieldForce;
        dragCurve.constant = forceFieldForce;
        this.magicStoneParticleSystemForceField.gravity = gravityCurve;
        this.magicStoneParticleSystemForceField.drag = dragCurve;

    }

    private void UpdateFlare(float lerpValue)
    {
        float lifeTime = Mathf.Lerp(this.minDurationLifeTime, this.maxDurationLifeTime, lerpValue);

        ParticleSystem.MainModule main = this.flareParticleSystem.main;
        ParticleSystem.MinMaxCurve lifeTimeCurve = main.startLifetime;
        lifeTimeCurve.constant = lifeTime;
        main.startLifetime = lifeTimeCurve;
    }

    private void UpdateSubLight(float lerpValue)
    {
        float rateOverTime = Mathf.Lerp(this.minRateOverTimeSubLight, this.maxRateOverTimeSubLight, lerpValue);

        ParticleSystem.EmissionModule emission = this.subLightParticleSystem.emission;
        ParticleSystem.MinMaxCurve curve = emission.rateOverTime;
        curve.constant = Mathf.RoundToInt(rateOverTime);
        emission.rateOverTime = curve;
    }

    private void UpdateSmallFlares(float lerpValue)
    {
        float rateOverTime = Mathf.Lerp(this.minRateOverTimeSmallFlares, this.maxRateOverTimeSmallFlares, lerpValue);

        ParticleSystem.EmissionModule emission = this.smallFlaresParticleSystem.emission;
        ParticleSystem.MinMaxCurve curve = emission.rateOverTime;
        curve.constant = Mathf.RoundToInt(rateOverTime);
        emission.rateOverTime = curve;
    }

    private void UpdateFlyingStones(float lerpValue)
    {
        float rateOverTime = Mathf.Lerp(this.minRateOverTimeFlyingStones, this.maxRateOverTimeFlyingStones, lerpValue);

        ParticleSystem.EmissionModule emission = this.flyingStonesParticleSystem.emission;
        ParticleSystem.MinMaxCurve curve = emission.rateOverTime;
        curve.constant = Mathf.RoundToInt(rateOverTime);
        emission.rateOverTime = curve;



        float orbitalZspeed = Mathf.Lerp(this.minOrbitalZSpeed, this.maxOrbitalZSpeed, lerpValue);

        ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = this.flyingStonesParticleSystem.velocityOverLifetime;
        ParticleSystem.MinMaxCurve orbitalZCurve = velocityOverLifetime.orbitalZ;
        orbitalZCurve.constant = orbitalZspeed;
        velocityOverLifetime.orbitalZ = orbitalZCurve;

        // Update Start Size 3D

        ParticleSystem.MainModule main = this.flyingStonesParticleSystem.main;
        ParticleSystem.MinMaxCurve startSizeXCurve = main.startSizeX;
        float startSizeXMin = Mathf.Lerp(this.minStartSizeXCurve.x, this.maxStartSizeXCurve.x, lerpValue);
        float startSizeXMax = Mathf.Lerp(this.minStartSizeXCurve.y, this.maxStartSizeXCurve.y, lerpValue);
        startSizeXCurve.constantMin = startSizeXMin;
        startSizeXCurve.constantMax = startSizeXMax;
        main.startSizeX = startSizeXCurve;

        ParticleSystem.MinMaxCurve startSizeYCurve = main.startSizeY;
        float startSizeYMin = Mathf.Lerp(this.minStartSizeYCurve.x, this.maxStartSizeYCurve.x, lerpValue);
        float startSizeYMax = Mathf.Lerp(this.minStartSizeYCurve.y, this.maxStartSizeYCurve.y, lerpValue);
        startSizeYCurve.constantMin = startSizeYMin;
        startSizeYCurve.constantMax = startSizeYMax;
        main.startSizeY = startSizeYCurve;

        ParticleSystem.MinMaxCurve startSizeZCurve = main.startSizeZ;
        float startSizeZMin = Mathf.Lerp(this.minStartSizeZCurve.x, this.maxStartSizeZCurve.x, lerpValue);
        float startSizeZMax = Mathf.Lerp(this.minStartSizeZCurve.y, this.maxStartSizeZCurve.y, lerpValue);
        startSizeZCurve.constantMin = startSizeZMax;
        startSizeZCurve.constantMax = startSizeZMax;
        main.startSizeZ = startSizeZCurve;
    }

    private void UpdateTrails(float lerpValue)
    {
        float rateOverTime = Mathf.Lerp(this.minRateOverTimeTrails, this.maxRateOverTimeTrails, lerpValue);

        ParticleSystem.EmissionModule emission = this.trailsParticleSystem.emission;
        ParticleSystem.MinMaxCurve curve = emission.rateOverTime;
        curve.constant = Mathf.RoundToInt(rateOverTime);
        emission.rateOverTime = curve;

        float startSize = Mathf.Lerp(this.minStartSize, this.maxStartSize, lerpValue);

        ParticleSystem.MainModule main = this.trailsParticleSystem.main;
        ParticleSystem.MinMaxCurve startSizeCurve = main.startSize;
        startSizeCurve.constant = startSize;
        main.startSize = startSizeCurve;

        float lifeTimeMin = Mathf.Lerp(this.minStartLifeTime.x, this.maxStartLifeTime.x, lerpValue);
        float lifeTimeMax = Mathf.Lerp(this.minStartLifeTime.y, this.maxStartLifeTime.y, lerpValue);
        ParticleSystem.MinMaxCurve lifeTimeCurve = main.startLifetime;
        lifeTimeCurve.constantMin = lifeTimeMin;
        lifeTimeCurve.constantMax = lifeTimeMax;
        main.startLifetime = lifeTimeCurve;
    }

    private void UpdateShine(float lerpValue)
    {
        float alpha = Mathf.Lerp(this.minAlpha, this.maxAlpha, lerpValue);

        ParticleSystem.MainModule main = this.shineParticleSystem.main;
        ParticleSystem.MinMaxGradient gradient = main.startColor;
        Color color = gradient.color;
        color.a = alpha;
        gradient.color = color;
        main.startColor = gradient;

        float burstCount = Mathf.Lerp(this.minBurstCount, this.maxBurstCount, lerpValue);
        ParticleSystem.EmissionModule emission = this.shineParticleSystem.emission;
        ParticleSystem.Burst burst = emission.GetBurst(0);
        burst.count = Mathf.RoundToInt(burstCount);
        emission.SetBurst(0, burst);
    }
    #endregion
}
