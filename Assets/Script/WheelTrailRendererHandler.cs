using System;
using UnityEngine;

namespace Script
{
    public class WheelTrailRendererHandler : MonoBehaviour
    {
        private CarController carController;
        private TrailRenderer trailRenderer;

        public void Awake()
        {
            //Get the car controller 
            carController = GetComponentInParent<CarController>();
            
            //Get the trail renderer component 
            trailRenderer = GetComponent<TrailRenderer>();
            
            //Set the trail renderer to not emit in the start.
            trailRenderer.emitting = false;
        }

        private void Update()
        {
            if (carController.IsTireScreeching(out float lateralVelocity, out bool isBraking))
            {
                trailRenderer.emitting = true;
            }
            else
            {
                trailRenderer.emitting = false;
            }
        }
    }
}
