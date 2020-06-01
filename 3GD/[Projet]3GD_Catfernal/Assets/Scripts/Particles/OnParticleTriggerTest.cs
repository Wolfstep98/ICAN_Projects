using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class OnParticleTriggerTest : MonoBehaviour
{
    #region Fields
    [SerializeField] private ParticleSystem particleSystem;

    private List<ParticleSystem.Particle> particlesEnter;
    #endregion

    #region Init
    private void Awake()
    {
        this.particlesEnter = new List<ParticleSystem.Particle>();
    }
    #endregion

    #region Properties

    #endregion

    #region Methods
    private void OnParticleTrigger()
    {
        Debug.Log("[Particles] - Trigger");
        //int enterCount = particleSystem.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, particlesEnter);
        //if(enterCount > 0)
        //{
        //    List<GameObject> objTrigger = new List<GameObject>();
        //    for(int i = 0; i < enterCount; i++)
        //    {
        //        ParticleSystem.Particle particle = this.particlesEnter[i];
        //    }
        //}
    }
    #endregion
}
