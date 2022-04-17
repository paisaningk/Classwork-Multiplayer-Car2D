using Mirror;
using UnityEngine;

namespace Script
{
    public class Controller : NetworkBehaviour
    {
        public float speed = 3;
        private Rigidbody2D rb;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        void FixedUpdate () 
        {
            float mH = Input.GetAxis ("Horizontal");
            float mV = Input.GetAxis ("Vertical");
            rb.velocity = new Vector2 (mH * speed, mV * speed);
        }
    
        void HandleMovement()
        {
            if (!isLocalPlayer) return;
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");
            Vector3 movement = new Vector3(moveHorizontal * 0.1f, moveVertical * 0.1f, 0);
            this.transform.position +=  movement;
        }
    
        public override void OnStartLocalPlayer()
        {
            // var setCamera = GameObject.FindWithTag("Cam").GetComponent<CinemachineVirtualCamera>();
            // var gameobject = this.gameObject;
            // setCamera.LookAt = gameobject.transform;
            // setCamera.Follow = gameobject.transform;
        }
    }
}
