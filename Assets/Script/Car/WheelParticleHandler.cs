using System;
using UnityEngine;

namespace Script
{
    public class WheelParticleHandler : MonoBehaviour
    {
        private float particleEmissionRate = 0;
        private CarController carController;
        private ParticleSystem particleSystemSmoke;
        private ParticleSystem.EmissionModule particleSystemEmissionModule;

        private void Awake()
        {
            //get the top down car controller
            carController = GetComponentInParent<CarController>();
            
            //Get the particle system
            particleSystemSmoke = GetComponent<ParticleSystem>();

            //Get the emission component
            particleSystemEmissionModule = particleSystemSmoke.emission;
        }

        private void Update()
        {
            //Reduce the particles over time
            particleEmissionRate = Mathf.Lerp(particleEmissionRate, 0, Time.deltaTime * 5);
            particleSystemEmissionModule.rateOverTime = particleEmissionRate;

            if (carController.IsTireScreeching(out float lateralVelocity, out  bool isBraking))
            {
                //if the car tire are screeching then we'll emitted smoke. If the player is braking then enitt a lot of smoke
                if (isBraking)
                {
                    particleEmissionRate = 30;
                }
                //If the player is drifting we'll emitt smoke based on how much the player is drifting.
                else
                {
                    particleEmissionRate = Mathf.Abs(lateralVelocity) * 2;
                }
            }
        }
    }
}
