using System;
using UnityEngine;
using UnityEngine.Events;
using Game.Sound;
using UnityEngine.UI;

namespace Game.Heat
{

    public interface IHeatGauge
    {
        #region Properties
        float Heat { get; }
        HeatState HeatState { get; }
        HeatGauge.GaugeState GaugeState { get; }
        #endregion

        #region Methods
        void AddHeat(float amount);
        void AddHeatPercent(float percentToAdd);
        #endregion
    }

    [Serializable]
    public class HeatGauge : GameBehavior, IHeatGauge
    {
        #region Class
        [Serializable]
        public class OnGaugeStateChangedEvent : UnityEvent<HeatState>
        {
            public OnGaugeStateChangedEvent() : base() { }
        }

        [Serializable]
        public class OnGaugeOverHeatEvent : UnityEvent
        {
            public OnGaugeOverHeatEvent() : base() { }
        }

        [Serializable]
        public class OnGaugeCoolDownEvent : UnityEvent
        {
            public OnGaugeCoolDownEvent() : base() { }
        }

        [Serializable]
        public class OnGaugeNormalEvent : UnityEvent
        {
            public OnGaugeNormalEvent() : base() { }
        }
        #endregion

        #region Enums
        public enum GaugeState
        {
            Normal,
            Cooldown,
            OverHeat,
            Disable
        }
        #endregion

        #region Fields

        [Header("References")] 
        [SerializeField] private Slider arrow = null;
        //[SerializeField] private GaugeArrow arrow = null;

        [Header("Data")]
        protected float heat = 0.0f;
        [SerializeField, Range(0.0f,1.0f)] protected float heatPercent = 0.0f;
        protected float maxHeat = 100.0f;

        [SerializeField, Range(0.0f, 1.0f)] protected float coldThreshold = 0.2f;
        [SerializeField, Range(0.0f, 1.0f)] protected float hotThreshold = 0.8f;

        protected HeatState heatState = HeatState.None;
        protected GaugeState gaugeState = GaugeState.Normal;

        [Header("Inferno State")]
        [SerializeField] protected float infernoIncreaseHeatAmountPerSec = 1.0f;

        [Header("Cooldown State")]
        [SerializeField] protected float cooldownDecreaseHeatAmountPerSec = 1.0f;

        [Header("Events")]
        private OnGaugeStateChangedEvent onGaugeStateChanged = null;
        private OnGaugeOverHeatEvent onGaugeOverHeat = null;
        private OnGaugeCoolDownEvent onGaugeCoolDown = null;
        private OnGaugeNormalEvent onGaugeNormal = null;
        public delegate void OnGaugeHeatEqualZeroEvent();
        private OnGaugeHeatEqualZeroEvent onGaugeHeatEqualZero = null;

        [Header("Cheats")]
        [SerializeField] private bool statesLocked = false;
        private float coldLockedStateValue = 0.0f;
        private float hotLockedStateValue = 50.0f;
        private float infernoLockedStateValue = 90.0f;
        #endregion

        #region Init
        public override void CustomAwake()
        {
            this.onGaugeStateChanged = new OnGaugeStateChangedEvent();
            this.onGaugeOverHeat = new OnGaugeOverHeatEvent();
            this.onGaugeCoolDown = new OnGaugeCoolDownEvent();
            this.onGaugeNormal = new OnGaugeNormalEvent();

            this.heatState = HeatState.Cold;
        }
        #endregion

        #region Properties
        public float Heat { get { return this.heat; } }

        public HeatState HeatState { get { return this.heatState; } }

        GaugeState IHeatGauge.GaugeState { get { return this.gaugeState; } }

        public OnGaugeHeatEqualZeroEvent OnGaugeHeatEqualZero { get { return this.onGaugeHeatEqualZero; } set { this.onGaugeHeatEqualZero = value; } }
        #endregion

        #region Methods

        #region Update
        public override void CustomUpdate()
        {
            switch(this.gaugeState)
            {
                case GaugeState.Normal:
                    switch(this.HeatState)
                    {
                        case HeatState.Cold:
                            this.ColdState();
                            break;
                        case HeatState.Hot:
                            this.HotState();
                            break;
                        default:
                            break;
                    }
                    break;
                case GaugeState.OverHeat:
                    this.InfernoState();
                    break;
                case GaugeState.Cooldown:
                    this.CooldownState();
                    break;
                case GaugeState.Disable:
                    break;
                default:
                    break;
            }

            this.arrow.value = this.heatPercent;
            //this.arrow.UpdateArrowRotation(this.heatPercent);
        }

        private void LateUpdate()
        {
            if(this.statesLocked)
            {
                switch(heatState)
                {
                    case HeatState.Cold:
                        this.heat = this.coldLockedStateValue;
                        break;
                    case HeatState.Hot:
                        this.heat = this.hotLockedStateValue;
                        break;
                    case HeatState.Inferno:
                        this.heat = this.infernoLockedStateValue;
                        break;
                }
            }
        }
        #endregion

        #region Heat
        public void AddHeat(float amount)
        {
            if (this.gaugeState == GaugeState.Normal || this.gaugeState == GaugeState.OverHeat)
            {
                float result = this.heat + Math.Abs(amount);
                if(result > this.maxHeat)
                {
                    // Max heat
                    result = this.maxHeat;
                }

                this.heat = result;
                this.heatPercent = this.heat / this.maxHeat;

                this.UpdateGaugeState();
            }
        }

        public void AddHeatPercent(float percentToAdd)
        {
            percentToAdd = Mathf.Clamp01(percentToAdd);

            float heatAdded = this.maxHeat * percentToAdd;

            this.AddHeat(heatAdded);
        }

        public void RemoveHeat(float amount)
        {
            float result = this.heat - Math.Abs(amount);
            if(result < 0.0f)
            {
                result = 0.0f;
                this.OnGaugeHeatEqualZero?.Invoke();
            }
            this.heat = result;
            this.heatPercent = this.heat / this.maxHeat;

            this.UpdateGaugeState();
        }
        public void RemoveHeatPercent(float percentToRemove)
        {
            percentToRemove = Mathf.Clamp01(percentToRemove);

            float heatRemoved = this.maxHeat * percentToRemove;

            this.RemoveHeat(heatRemoved);
        }
        #endregion

        #region Gauge
        protected void UpdateGaugeState()
        {
            switch(this.gaugeState)
            {
                case GaugeState.Normal:
                    if(this.heatPercent < this.coldThreshold && this.heatState == HeatState.Hot)
                    {
                        this.HotToCold();
                        return;
                    }
                    if (this.heatPercent >= this.coldThreshold && this.heatPercent < this.hotThreshold && this.heatState == HeatState.Cold)
                    {
                        this.ColdToHot();
                        return;
                    }
                    else if (this.heatPercent >= this.hotThreshold && this.heatState == HeatState.Hot)
                    {
                        this.HotToInferno();
                        return;
                    }
                    break;
                case GaugeState.OverHeat:
                    if(this.heat == this.maxHeat)
                    {
                        this.StartCooldown();
                    }
                    break;
                case GaugeState.Cooldown:
                    if(this.heat == 0.0f)
                    {
                        this.EndCooldown();
                    }
                    break;
            }
        }

        // States
        protected void ColdState()
        {

        }
        protected void HotState()
        {

        }
        protected void InfernoState()
        {
            this.AddHeat(this.infernoIncreaseHeatAmountPerSec * Time.deltaTime);
        }
        protected void CooldownState()
        {
            this.RemoveHeat(this.cooldownDecreaseHeatAmountPerSec * Time.deltaTime);
        }

        // Transition
        protected void HotToCold()
        {
            this.heatState = HeatState.Cold;
            this.onGaugeStateChanged?.Invoke(this.heatState);
        }
        protected void ColdToHot()
        {
            this.heatState = HeatState.Hot;
            this.onGaugeStateChanged?.Invoke(this.heatState);
        }
        protected void HotToInferno()
        {
            this.heatState = HeatState.Inferno;
            this.gaugeState = GaugeState.OverHeat;

            this.onGaugeOverHeat?.Invoke();
            this.onGaugeStateChanged?.Invoke(this.heatState);
        }
        protected void StartCooldown()
        {
            this.heatState = HeatState.None;
            this.gaugeState = GaugeState.Cooldown;

            this.onGaugeCoolDown?.Invoke();
            this.onGaugeStateChanged?.Invoke(this.heatState);
        }
        protected void EndCooldown()
        {
            this.heatState = HeatState.Cold;
            this.gaugeState = GaugeState.Normal;

            this.onGaugeNormal?.Invoke();
            this.onGaugeStateChanged?.Invoke(this.heatState);
        }

        //Cheats
        public void LockColdState()
        {
            this.statesLocked = true;
            this.heat = this.coldLockedStateValue;
            this.heatState = HeatState.Cold;
            this.onGaugeStateChanged?.Invoke(this.heatState);
        }
        public void LockHotState()
        {
            this.statesLocked = true;
            this.heat = this.hotLockedStateValue;
            this.heatState = HeatState.Hot;
            this.onGaugeStateChanged?.Invoke(this.heatState);
        }
        public void LockInfernoState()
        {
            this.statesLocked = true;
            this.heat = this.infernoLockedStateValue;
            this.heatState = HeatState.Inferno;
            this.onGaugeStateChanged?.Invoke(this.heatState);

            this.gaugeState = GaugeState.OverHeat;
            this.onGaugeOverHeat?.Invoke();
        }
        public void UnlockStates()
        {
            this.statesLocked = false;
        }
        #endregion

        #region Events
        public void RegisterOnGaugeOverHeat(UnityAction unityAction)
        {
            this.onGaugeOverHeat?.AddListener(unityAction);
        }
        public void UnregisterOnGaugeOverHeat(UnityAction unityAction)
        {
            this.onGaugeOverHeat?.RemoveListener(unityAction);
        }

        public void RegisterOnGaugeCoolDown(UnityAction unityAction)
        {
            this.onGaugeCoolDown?.AddListener(unityAction);
        }
        public void UnregisterOnGaugeCoolDown(UnityAction unityAction)
        {
            this.onGaugeCoolDown?.RemoveListener(unityAction);
        }

        public void RegisterOnGaugeNormal(UnityAction unityAction)
        {
            this.onGaugeNormal?.AddListener(unityAction);
        }
        public void UnregisterOnGaugeNormal(UnityAction unityAction)
        {
            this.onGaugeNormal?.RemoveListener(unityAction);
        }

        public void RegisterOnGaugeStateChanged(UnityAction<HeatState> unityAction)
        {
            this.onGaugeStateChanged?.AddListener(unityAction);
        }
        public void UnregisterOnGaugeStateChanged(UnityAction<HeatState> unityAction)
        {
            this.onGaugeStateChanged?.RemoveListener(unityAction);
        }
        #endregion

        #endregion
    }
}