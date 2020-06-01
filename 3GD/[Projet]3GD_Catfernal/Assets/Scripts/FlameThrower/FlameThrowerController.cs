using System;
using UnityEngine;
using UnityEngine.UI;
using Game.Heat;
using Game.Entities.Player;
using Game.Sound;

namespace Game.Entities.FlameThrower
{

    [Serializable]
    public class FlameThrowerController : GameBehavior
    {
        #region Enums
        public enum FlameThrowerMode
        {
            Spray,
            Lazer
        }
        #endregion

        #region Fields
        [Header("References")]
        [SerializeField] private new Camera camera = null;
        [SerializeField] private Rigidbody2D playerRigidbody = null;
        [SerializeField] private PlayerController playerController = null;
        [SerializeField] private GameSoundManager soundManager = null;
        [Header("FlameThrower")]
        [SerializeField] private ParticleSystem spray = null;
        [SerializeField] private ParticleSystem lazer = null;
        [SerializeField] private ParticleSystem spread = null;
        [SerializeField] private ParticleSystem steam = null;
        [Header("Lights")] 
        [SerializeField] private GameObject lightSpray1 = null;
        [SerializeField] private GameObject lightSpray2 = null;
        [SerializeField] private Transform lightSprayPos1 = null;
        [SerializeField] private Transform lightSprayPos2 = null;
        [SerializeField] private float smoothSpray = 0.125f; 
        [SerializeField] private GameObject lightLazer1 = null;
        [SerializeField] private GameObject lightLazer2 = null;
        [Header("Backfire")]
        [SerializeField] private ParticleSystem backfire = null;
        [SerializeField] private ParticleSystem backfireLeftFoot = null;
        [SerializeField] private ParticleSystem backfireRightFoot = null;
        [Header("Gauge")]
        [SerializeField] private HeatGauge heatGauge = null;
        [SerializeField] private SwitchStance switchStance = null;
        [SerializeField] private Image switchCooldownImage = null;
        [Header("Inferno")]
        [SerializeField] private ParticleSystem fireWave = null;
        [SerializeField] private LaserInfernoBombBehaviour infernoBomb= null;

        private bool slowCooldown = false;
        private bool overHeat = false;
        private bool cooldown = false;
        private bool isFiring = false;
        private bool itsTimeToStopSpray = false;
        private float sprayStopTime = 0;
        private float lifeTimeSpray;
        private bool itsTimeToStopLazer = false;
        private float lazerStopTime = 0;
        private float lifeTimeLazer;
        private FlameThrowerMode mode = FlameThrowerMode.Spray;

        [Header("Spray")]
        [SerializeField, Range(0.0f, 1.0f)] private float sprayHeatPerSecPercent = 0.05f;
        private float baseSprayLifetime;
        [Header("Lazer")]
        [SerializeField, Range(0.0f, 1.0f)] private float lazerHeatPerSecPercent = 0.02f;
        private float baseLazerLifetime;
        [Header("Switch")]
        [SerializeField, Range(0.0f,1.0f)] private float switchHeatDecreasePercent = 0.1f;
        private bool canSwitch = true;
        [SerializeField] private float cooldownBetweenSwitches = 5.0f;
        private float switchTimer = 0.0f;

        [Header("Backfire")]
        [SerializeField, Range(0.0f, 1.0f)] private float backfireHeatPercent = 0.1f;
        [SerializeField] private float backfireForce = 10.0f;

        [Header("PassiveHeatDecrease")]
        [SerializeField, Range (0.0f, 0.5f)] private float passiveHeatDecreaseRate = 0.01f;
        #endregion

        #region Init
        public override void CustomAwake()
        {
            if(this.soundManager == null) this.soundManager = GameSoundManager.Instance;

            this.baseSprayLifetime = this.spray.startLifetime;
            this.baseLazerLifetime = this.lazer.startLifetime;
            this.EnableSwitch();

            this.lazer.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
            this.spray.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            this.spread.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            this.steam.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            this.lazer.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            this.spray.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            this.spread.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            this.steam.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            
            this.lightSpray1.SetActive(false);
            this.lightSpray2.SetActive(false);
            this.lightLazer1.SetActive(false);
            this.lightLazer2.SetActive(false);

            this.heatGauge.RegisterOnGaugeOverHeat(this.FlameThrowerOverHeat);
            this.heatGauge.RegisterOnGaugeCoolDown(this.StartCooldown);
            this.heatGauge.RegisterOnGaugeNormal(this.EndCooldown);
            this.heatGauge.RegisterOnGaugeStateChanged(this.OnHeatStateUpdated);
            this.heatGauge.OnGaugeHeatEqualZero += this.OnHeatEqualZero;
        }
        #endregion

        #region Methods
        public void ActivateFlameThrower()
        {
            if (!this.overHeat && !this.cooldown)
            {
                this.isFiring = true;
                var sprayShape = this.spray.shape;

                switch (this.mode)
                {
                    case FlameThrowerMode.Spray:
                        this.itsTimeToStopSpray = false;
                        this.sprayStopTime = 0;
                        this.lightSpray1.SetActive(true);
                        this.lightSpray2.SetActive(true);
                        this.lightSpray1.transform.position = this.lightSprayPos1.position;
                        this.lightSpray2.transform.position = this.lightSprayPos2.position; 
                        switch(this.heatGauge.HeatState)
                        {
                            case HeatState.Cold:
                                this.spray.Play(true);
                                this.lifeTimeSpray = this.baseSprayLifetime / 2;
                                this.spray.startLifetime = this.lifeTimeSpray;
                                sprayShape.shapeType = ParticleSystemShapeType.Cone;
                                break;
                            case HeatState.Hot:
                                this.spray.Play(true);
                                this.lifeTimeSpray = this.baseSprayLifetime;
                                this.spray.startLifetime = this.baseSprayLifetime;
                                sprayShape.shapeType = ParticleSystemShapeType.Cone;
                                break;
                            case HeatState.Inferno:
                                this.spray.Play(true);
                                sprayShape.shapeType = ParticleSystemShapeType.ConeVolume;
                                break;
                        }
                        this.soundManager.PlaySpray();
                        break;
                    case FlameThrowerMode.Lazer:
                        this.itsTimeToStopLazer = false;
                        this.lazerStopTime = 0;
                        this.lightLazer1.SetActive(true);
                        switch (this.heatGauge.HeatState)
                        {
                            case HeatState.Cold:
                                this.lazer.transform.localScale = Vector3.one;
                                this.lazer.transform.GetChild(0).localScale = Vector3.one;
                                this.lazer.Play(true);
                                this.lifeTimeLazer = this.baseLazerLifetime / 3;
                                this.lazer.startLifetime = this.lifeTimeLazer;
                                break;
                            case HeatState.Hot:
                                this.lazer.transform.localScale = Vector3.one;
                                this.lazer.transform.GetChild(0).localScale = Vector3.one;
                                this.lightLazer2.SetActive(true);
                                this.lazer.Play(true);
                                this.lifeTimeLazer = this.baseLazerLifetime;
                                this.lazer.startLifetime = this.baseLazerLifetime;
                                break;
                            case HeatState.Inferno:
                                this.lazer.transform.localScale = new Vector3(4,4,1);
                                this.lazer.transform.GetChild(0).localScale = new Vector3(2.5f, 2.5f, 1);
                                this.lazer.Play(true);
                                break;
                        }
                        this.soundManager.PlayLazer();
                        break;
                    default:
                        break;
                }
                this.soundManager.PlayTrigger();
            }
        }

        public void DesactivatedFlameThrower(bool forceChange = false)
        {
            if (this.isFiring || forceChange)
            {
                this.isFiring = false;
                switch (this.mode)
                {
                    case FlameThrowerMode.Spray:
                        itsTimeToStopSpray = true;
                        switch (this.heatGauge.HeatState)
                        {
                            case HeatState.Cold:
                                this.spray.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                                this.StartSlowCooldown();
                                break;
                            case HeatState.Hot:
                                this.spray.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                                this.StartSlowCooldown();
                                break;
                            case HeatState.None:
                            case HeatState.Inferno:
                                this.spray.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                                this.fireWave.Play(true);
                                break;
                        }
                        this.soundManager.StopSpray();
                        break;
                    case FlameThrowerMode.Lazer:
                        itsTimeToStopLazer = true;
                        switch (this.heatGauge.HeatState)
                        {
                            case HeatState.Cold:
                                this.lazer.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                                this.StartSlowCooldown();
                                break;
                            case HeatState.Hot:
                                this.lazer.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                                this.StartSlowCooldown();
                                break;
                            case HeatState.None:
                            case HeatState.Inferno:
                                this.lazer.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                                Vector2 bombDirection = this.playerController.GetDirection();
                                this.infernoBomb.Shoot(bombDirection);
                                break;
                        }
                        this.soundManager.StopLazer();
                        break;
                    default:
                        break;
                }
            }
        }

        public void FlameThrowerOverHeat()
        {
            if (!this.isFiring)
            {
                this.ActivateFlameThrower();
            }
            else
            {
                this.DesactivatedFlameThrower(true);
                this.ActivateFlameThrower();
            }
            this.overHeat = true;
            this.isFiring = false;
        }

        public void StartCooldown()
        {
            this.cooldown = true;
            this.overHeat = false;
            this.isFiring = false;

            this.DesactivatedFlameThrower(true);

            this.steam.Play(true);

            this.soundManager.PlayCooldown();
        }

        public void EndCooldown()
        {
            this.cooldown = false;

            this.steam.Stop(true, ParticleSystemStopBehavior.StopEmitting);

            this.soundManager.StopCooldown();
        }

        public void SwitchMode()
        {
            if (!this.isFiring && !this.overHeat && !this.cooldown && this.canSwitch)
            {
                switch (this.mode)
                {
                    case FlameThrowerMode.Spray:
                        switch(this.heatGauge.HeatState)
                        {
                            case HeatState.Cold:
                                this.steam.Play(true);
                                break;
                            case HeatState.Hot:
                                this.steam.Play(true);
                                break;
                        }
                        this.switchStance.SwitchToLazer();
                        this.mode = FlameThrowerMode.Lazer;
                        break;
                    case FlameThrowerMode.Lazer:
                        switch (this.heatGauge.HeatState)
                        {
                            case HeatState.Cold:
                                this.steam.Play(true);
                                break;
                            case HeatState.Hot:
                                this.steam.Play(true);
                                break;
                        }
                        this.switchStance.SwitchToSpray();
                        this.mode = FlameThrowerMode.Spray;
                        break;
                    default:
                        break;
                }
                this.heatGauge.RemoveHeatPercent(this.switchHeatDecreasePercent);
                this.DisableSwitch();
                this.soundManager.PlaySwitch();
            }
        }

        private void OnHeatStateUpdated(HeatState heatState)
        {
            switch(heatState)
            {
                case HeatState.Cold:
                    if (this.isFiring)
                    {
                        this.DesactivatedFlameThrower(true);
                        this.ActivateFlameThrower();
                    }
                    break;
                case HeatState.Hot:
                    if (this.isFiring)
                    {
                        this.DesactivatedFlameThrower(true);
                        this.ActivateFlameThrower();
                    }
                    break;
                default:
                    break;
            }
        }

        private void EnableSwitch()
        {
            this.canSwitch = true;
            this.switchTimer = 0.0f;
            this.switchCooldownImage.color = Color.green;
        }

        private void DisableSwitch()
        {
            this.canSwitch = false;
            this.switchCooldownImage.color = Color.red;
        }

        public void Backfire(Vector2 mousePos)
        {
            if (!this.cooldown && !this.overHeat && !this.isFiring)
            {
                this.backfire.Play();
                this.backfireLeftFoot.Play();
                this.backfireRightFoot.Play();

                Vector3 mouseWorldPos = this.camera.ScreenToWorldPoint(mousePos);
                mouseWorldPos.z = 0.0f;
                Vector2 trajectory = (Vector2)mouseWorldPos - this.playerRigidbody.position;
                trajectory.Normalize();
                this.backfire.transform.rotation = this.playerRigidbody.transform.rotation;
                Vector3 position = this.playerRigidbody.position + (Vector2)trajectory * 3.0f;
                position += Vector3.back;
                this.backfire.transform.position = position;
                

                Vector2 inverseTrajectory = -trajectory;
                this.playerController.AddBackfireForce(inverseTrajectory.normalized * this.backfireForce);
                this.heatGauge.AddHeatPercent(this.backfireHeatPercent);

                this.soundManager.PlayBackfire();
            }
        }

        private void StartSlowCooldown()
        {
            this.slowCooldown = true;
            this.soundManager.PlaySlowCooldown();
        }

        private void OnHeatEqualZero()
        {
            if (this.slowCooldown)
            {
                this.slowCooldown = false;
                this.soundManager.StopSlowCooldown();
            }
        }

        public override void CustomUpdate()
        {
            if(this.isFiring)
            {
                switch (this.mode)
                {
                    case FlameThrowerMode.Spray:
                        this.heatGauge.AddHeatPercent(this.sprayHeatPerSecPercent * Time.deltaTime);
                        break;
                    case FlameThrowerMode.Lazer:
                        this.heatGauge.AddHeatPercent(this.lazerHeatPerSecPercent * Time.deltaTime);
                        break;
                    default:
                        break;
                }
            }

            if (this.itsTimeToStopLazer){
                this.lazerStopTime += GameTime.deltaTime;
            }

            if (this.itsTimeToStopSpray){
                this.sprayStopTime += GameTime.deltaTime;
            }

            if (this.sprayStopTime >= this.lifeTimeSpray){
                this.itsTimeToStopSpray = false;
                this.sprayStopTime = 0;
                this.lightSpray1.SetActive(false);
                this.lightSpray2.SetActive(false);
            }
            
            if (this.heatGauge.HeatState == HeatState.Hot && this.lazerStopTime >= this.lifeTimeLazer * 0.5f){
                this.lightLazer1.SetActive(false);
            }
            
            if (this.lazerStopTime >= this.lifeTimeLazer){
                this.itsTimeToStopLazer = false;
                this.lazerStopTime = 0;
                this.lightLazer1.SetActive(false);
                this.lightLazer2.SetActive(false);
            }

            this.lightSpray1.transform.position = Vector3.Lerp(this.lightSpray1.transform.position, this.heatGauge.HeatState == HeatState.Hot ? this.lightSprayPos1.position + this.lightSprayPos1.right * 1.5f : this.lightSprayPos1.position, smoothSpray * GameTime.deltaTime);
            this.lightSpray2.transform.position = Vector3.Lerp(this.lightSpray2.transform.position, this.heatGauge.HeatState == HeatState.Hot ? this.lightSprayPos2.position + this.lightSprayPos2.right * 1.5f : this.lightSprayPos2.position, smoothSpray * GameTime.deltaTime);

            if(this.overHeat)
            {
                switch (this.mode)
                {
                    case FlameThrowerMode.Spray:
                        this.heatGauge.AddHeatPercent(this.sprayHeatPerSecPercent * Time.deltaTime);
                        break;
                    case FlameThrowerMode.Lazer:
                        this.heatGauge.AddHeatPercent(this.lazerHeatPerSecPercent * Time.deltaTime);
                        break;
                    default:
                        break;
                }
            }

            if(this.cooldown)
            {
                if (!this.steam.isPlaying)
                {
                    this.steam.Play(true);
                }
            }

            if(!this.canSwitch)
            {
                this.switchTimer += Time.deltaTime;
                if(this.switchTimer >= this.cooldownBetweenSwitches)
                {
                    this.EnableSwitch();
                }
            }

            if (!this.isFiring && !this.overHeat && !this.cooldown && this.slowCooldown)
            {
                this.heatGauge.RemoveHeatPercent(this.passiveHeatDecreaseRate);
            }
        }
        #endregion
    }
}