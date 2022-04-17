using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Script
{
    public class CarSfxHandler : MonoBehaviour
    {
        [Header("Audio sources")] 
        public AudioSource tiresScreechingAudioSource;
        public AudioSource engineAudioSource;
        public AudioSource carHitAudioSource;

        private CarController carController;
        private float desiredEnginePitch = 0.5f;
        private float tireScreechPitch = 0.5f;

        public void Awake()
        {
            carController = GetComponent<CarController>();
        }

        public void Update()
        {
            UpdateEngineSfx();
            UpdateTiresScreechingSfx();
        }

        private void UpdateEngineSfx()
        {
            //Handle engine SFX
            float velocityMagnitude = carController.GetVelocityMagnitude();
            
            //Increase the engine volume as the car goes faster
            float desiredEngineVolume = velocityMagnitude * 0.05f;
            
            //But keep a minimum level so it player even if the car is idle
            desiredEngineVolume = Mathf.Clamp(desiredEngineVolume, 0.2f, 1.0f);

            engineAudioSource.volume = Mathf.Lerp(engineAudioSource.volume, desiredEngineVolume, Time.deltaTime * 10);
            
            //To add more variation to the engine sound we also change the pitch
            desiredEngineVolume = velocityMagnitude * 0.2f;
            desiredEngineVolume = Mathf.Clamp(desiredEngineVolume, 0.5f, 2f);
            engineAudioSource.pitch = Mathf.Lerp(engineAudioSource.pitch, desiredEngineVolume, Time.deltaTime * 1.5f);
        }

        private void UpdateTiresScreechingSfx()
        {
            //Handle tire screeching SFX
            if (carController.IsTireScreeching(out float lateralVelocity, out bool isBraking))
            {
                //If the car is braking we want the tire screech to be louder and also change the pitch
                if (isBraking)
                {
                    tiresScreechingAudioSource.volume = 
                        Mathf.Lerp(tiresScreechingAudioSource.volume, 1, Time.deltaTime * 10);

                    tireScreechPitch = Mathf.Lerp(tireScreechPitch, 0.5f, Time.deltaTime * 10);
                }
                else
                {
                    //If we are not braking we still want to play this screech sound if the player is drifting
                    tiresScreechingAudioSource.volume = Mathf.Abs(lateralVelocity) * 0.05f;
                    tireScreechPitch = Mathf.Abs(lateralVelocity) * 0.1f;
                }
            }
            else
            {
                tiresScreechingAudioSource.volume =
                    Mathf.Lerp(tiresScreechingAudioSource.volume, 0, Time.deltaTime * 10);
            }
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            //Get the relative velocity of the collison
            float relativeVelocity = col.relativeVelocity.magnitude;
            float volume = relativeVelocity * 0.1f;
            
            carHitAudioSource.pitch = Random.Range(0.95f, 1.05f);
            carHitAudioSource.volume = volume;

            if (!carHitAudioSource.isPlaying)
            {
                carHitAudioSource.Play();
            }
        }
    }
}
