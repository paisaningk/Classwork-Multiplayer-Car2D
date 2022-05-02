using Script.Car;
using UnityEngine;

namespace Script
{
    public class CheckPoint : MonoBehaviour
    {
        public bool isPast = false;
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("PlayerHitbox"))
            {
                Debug.Log(col.GetComponentInParent<CarController>().isLocalPlayer);
                if (col.GetComponentInParent<CarController>().isLocalPlayer)
                {
                    isPast = true;
                }
            }
        }
    }
}
