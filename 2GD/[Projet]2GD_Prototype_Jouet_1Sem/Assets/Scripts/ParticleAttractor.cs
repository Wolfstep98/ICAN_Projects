using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAttractor : MonoBehaviour {

    public ParticleSystem instanceParticleSystem;

    public ParticleSystem.Particle[] particles;

    public int particlesNbr;

    public float puissanceVelocity = 2f;

    private void Start()
    {
        if(instanceParticleSystem == null)
        {
            instanceParticleSystem = GameObject.Find("ParticleSpawnPoint").GetComponent<ParticleSystem>();
        }
        particles = new ParticleSystem.Particle[instanceParticleSystem.main.maxParticles];
    }

    private void LateUpdate()
    {
        
        particlesNbr = instanceParticleSystem.GetParticles(particles);
        for(int i = 0; i < particlesNbr; i++)
        {
            if ((transform.position - particles[i].position).magnitude < 0.025f)
            {
                particles[i].remainingLifetime = 0;
            }
            particles[i].velocity = Vector3.zero;
            particles[i].position = Vector3.Lerp(particles[i].position, transform.position, Time.deltaTime);
            //particles[i].velocity += (transform.position - particles[i].position) * puissanceVelocity * Time.deltaTime;
        }
        instanceParticleSystem.SetParticles(particles, particlesNbr);
    }
}
