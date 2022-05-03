using Script.Car;
using UnityEngine;

namespace Script
{
    public class CheckPoint : MonoBehaviour
    {
        public bool isPast = false;
        public AudioSource audioSource;
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("PlayerHitbox"))
            {
                if (col.GetComponentInParent<CarController>().isLocalPlayer)
                {
                    isPast = true;
                    audioSource.Play();
                }
            }
        }
    }
}
