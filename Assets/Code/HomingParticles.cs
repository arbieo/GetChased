using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class HomingParticles : MonoBehaviour
{
    public GameObject target;
    public float homingStart = 0.25f;
    public float homingFull = 0.5f;
    public float homingAccel = 100;

    ParticleSystem.Particle[] particles;
    
    ParticleSystem ps;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[ps.particleCount];
    }

    void LateUpdate()
    {
        if (ps.particleCount > particles.Length)
        {
            particles = new ParticleSystem.Particle[ps.particleCount];
        }

        if (ps.time < homingStart)
        {
            return;
        }

        ps.GetParticles(particles);

        for (int i = 0; i < ps.particleCount; i++)
        {
            ParticleSystem.Particle p = particles[i];

            Vector3 particleWorldPosition;

            if (ps.main.simulationSpace == ParticleSystemSimulationSpace.Local)
            {
                particleWorldPosition = transform.TransformPoint(p.position);
            }
            else if (ps.main.simulationSpace == ParticleSystemSimulationSpace.Custom)
            {
                particleWorldPosition = ps.main.customSimulationSpace.TransformPoint(p.position);
            }
            else
            {
                particleWorldPosition = p.position;
            }

            if (target == null)
            {
                particles[i].remainingLifetime = 0.5f - ps.time;
            }
            else
            {
                Vector3 distanceToTarget = target.transform.position - particleWorldPosition;
                Vector3 directionToTarget = distanceToTarget.normalized;

                p.velocity += directionToTarget * Time.deltaTime * homingAccel;
                if (ps.time > homingStart)
                {
                    p.velocity = p.velocity.magnitude * directionToTarget;
                }

                if (distanceToTarget.magnitude < p.velocity.magnitude * Time.deltaTime)
                {
                    p.remainingLifetime = -1;
                }
            }

            particles[i] = p;
        }

        ps.SetParticles(particles, ps.particleCount);
    }
}