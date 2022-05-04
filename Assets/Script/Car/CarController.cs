using System.Collections;
using Cinemachine;
using Mirror;
using Script.Network;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script.Car
{
    public class CarController : NetworkBehaviour
    {
        [Header("Car Setting")] 
        public float driftFactor = 0.95f;
        public float accelerationFactor = 30;
        public float turnFactor = 3.5f;
        public float maxSpeed = 30;
        [Header("Sprites")] 
        public SpriteRenderer carSpriteRenderer;
        public SpriteRenderer carShadowSpriteRenderer;
        
        [Header("Jumping")] 
        public AnimationCurve jumpCurve;
        public ParticleSystem landingParticleSystem;
        [Header("Setup")] public GameObject Setup;
        
        private float accelerationInput;
        private float steeringInput;
        private float velocityVsUp = 0;
        public bool isJumping = false;
        private float rotationAngle;
        private Rigidbody2D rb;
        private Collider2D collider2D;
        private CarSfxHandler carSfxHandler;
        private PlayerData playerData;

        //CheckPoint 
        public GameObject[] checkPoints;
        public int currentIndexCheckPoint;
        public bool isFinishLine = false;
        public bool isSetup = false;
        public bool isCanGo = false;

        private void Awake()
        {
            isFinishLine = false;
            currentIndexCheckPoint = 0;
            collider2D = GetComponentInChildren<Collider2D>();
            rb = GetComponent<Rigidbody2D>();
            carSfxHandler = GetComponent<CarSfxHandler>();
            playerData = GetComponent<PlayerData>();
            Setup.SetActive(false);
        }

        public void Update()
        {
            if (SceneManager.GetActiveScene().name == "Lobby") return;

            // Get input
            CheckPoint();
            CheckScene();
            float inputX = Input.GetAxis ("Horizontal");
            float inputY = Input.GetAxis ("Vertical");
            steeringInput = inputX;
            accelerationInput = inputY;
        }

        
        public void FixedUpdate()
        {
            if (!isCanGo) return;
            //check is local player
            if (!isLocalPlayer) return;
            
            ApplyEngineForce();
            KillOrthogonalVelocity();
            ApplySteering();
        }

        private void CheckPoint()
        {
            if (!isLocalPlayer) return;
            if (!isSetup) return;
            
            if (currentIndexCheckPoint == checkPoints.Length)
            {
                isFinishLine = true;
                return;
            }
            var checking = checkPoints[currentIndexCheckPoint].GetComponent<CheckPoint>().isPast;
            checkPoints[currentIndexCheckPoint].SetActive(!checking);
            if (checking == true)
            {
                checkPoints[currentIndexCheckPoint].SetActive(!checking);
                currentIndexCheckPoint++;
            }
        }

        private void CheckScene()
        {
            playerData.UpdateColor();
            Setup.SetActive(true);
            if (isLocalPlayer)
            {
                var setCamera = GameObject.FindWithTag("MainCamera").GetComponentInChildren<CinemachineVirtualCamera>();
                var gameObject = this.gameObject;
                setCamera.LookAt = gameObject.transform;
                setCamera.Follow = gameObject.transform;
            }

        }

        private void ApplyEngineForce()
        {
            //calculate how much "forward" we are going in terms of the direction of our velocity
            velocityVsUp = Vector2.Dot(transform.up, rb.velocity);
        
            //Limit so we cannot go faster than the max speed in the "Forward" direction
            if (velocityVsUp > maxSpeed && accelerationInput > 0)
            {
                return;
            }
        
            //Limit so we cannot go faster than the max speed in the "reverse" direction
            if (velocityVsUp < -maxSpeed * 0.5f && accelerationInput < 0)
            {
                return;
            }

            //Limit so we cannot go faster in any direction while accel
            if (rb.velocity.sqrMagnitude > maxSpeed * maxSpeed && accelerationInput > 0)
            {
                return;
            }
        
            //Apply drag if there is no acceleration input so the car stops when the player let's go of the accelerator
            if (accelerationInput == 0)
            {
                rb.drag = Mathf.Lerp(rb.drag, 3, Time.fixedDeltaTime * 3);
            }
            else
            {
                rb.drag = 0;
            }
        
            //Create a force for the engine
            Vector2 engineForeVector = transform.up * accelerationInput * accelerationFactor;
        
            //Apply force and pushes the car forward
            rb.AddForce(engineForeVector,ForceMode2D.Force);
        }

        private void ApplySteering()
        {
            //Limit the cars ability to turn when moving slowly
            float minSpeedBeforeAllowTurningFactor = rb.velocity.magnitude / 8;
            minSpeedBeforeAllowTurningFactor = Mathf.Clamp01(minSpeedBeforeAllowTurningFactor);
        
            //Update the rotation angle based on input
            rotationAngle -= steeringInput * turnFactor * minSpeedBeforeAllowTurningFactor;
        
            //Apply steering by rotation the car object
            rb.MoveRotation(rotationAngle);
        }

        private void KillOrthogonalVelocity()
        {
            Vector2 forwardVelocity = transform.up * Vector2.Dot(rb.velocity, transform.up);
            Vector2 rightVelocity = transform.right * Vector2.Dot(rb.velocity, transform.right);

            rb.velocity = forwardVelocity + rightVelocity * driftFactor;
        }

        private float GetLateralVelocity()
        {
            //Return how fast the car is moving sideways.
            return Vector2.Dot(transform.right, rb.velocity);
        }

        public bool IsTireScreeching(out float lateralVelocity, out bool isBraking)
        {
            lateralVelocity = GetLateralVelocity();
            isBraking = false;

            //Check if we are moving forward and it the player is hitting the brakes
            if (accelerationInput < 0 && velocityVsUp > 0)
            {
                isBraking = true;
                return true;
            }
            
            //If we have a lot of side movement then the tires should be screeching
            if (Mathf.Abs(GetLateralVelocity()) > 4.0f)
            {
                return true;
            }
            return false;
        }
        
        public float GetVelocityMagnitude()
        {
            return rb.velocity.magnitude;
        }

        public void Jump(float jumpHeightScale, float jumpPushScale)
        {
            if (!isJumping)
            {
                StartCoroutine(JumpCo(jumpHeightScale, jumpPushScale));
            }
        }
        
        private IEnumerator JumpCo(float jumpHeightScale, float jumpPushScale)
        {
            isJumping = true;
            carSfxHandler.PlayJumpSfx();

            float jumpStarting = Time.time;
            float jumpDuration = rb.velocity.magnitude * 0.05f;
            
            jumpHeightScale = jumpHeightScale * jumpDuration;
            jumpHeightScale = Mathf.Clamp(jumpHeightScale, 0, 1);
            
            collider2D.enabled = false;
            
            //add force forward
            rb.AddForce(rb.velocity.normalized * jumpHeightScale * 10 ,ForceMode2D.Impulse);

            //Set layer
            carSpriteRenderer.sortingLayerName = "Fly";
            carShadowSpriteRenderer.sortingLayerName = "Fly";
            
            while (isJumping)
            {
                //Percentage 0 - 1.0 OF Where we are in the jumping process
                float jumpCompletedPercentage = (Time.time - jumpStarting) / jumpDuration;
                jumpCompletedPercentage = Mathf.Clamp01(jumpCompletedPercentage);
                float jumpCurveEvaluate = jumpCurve.Evaluate(jumpCompletedPercentage) * jumpHeightScale;
                
                //Take the base scale of 1 amd add how much we should increase the scale with
                carSpriteRenderer.transform.localScale = Vector3.one + Vector3.one * jumpCurveEvaluate;
                
                //Change the shadow scale also but make it a bit smaller.
                carShadowSpriteRenderer.transform.localScale = carSpriteRenderer.transform.localScale * 0.75f;
                
                //Offset the shadow a bit.
                carShadowSpriteRenderer.transform.localPosition = new Vector3(1, -1, 0) * 3 * jumpCurveEvaluate;
                                                                  
                //When we reach 100% we are done
                if (jumpCompletedPercentage == 1)
                {
                    break;
                }
                yield return null;
            }
            
            //check if landing is ok
            if (Physics2D.OverlapCircle(transform.position,1.5f))
            {
                isJumping = false;
                
                Jump(0.2f,0);
            }
            else
            {
                //play sfx and particle system
                landingParticleSystem.Play();
                carSfxHandler.PlayLandingSfx();
                
                //Handle landing, scale back the object
                carShadowSpriteRenderer.transform.localScale = carSpriteRenderer.transform.localScale;
                carShadowSpriteRenderer.transform.localPosition = Vector3.zero;

                //Change state
                isJumping = false;
                collider2D.enabled = true;
            
                //Set layer
                carSpriteRenderer.sortingLayerName = "Player";
                carShadowSpriteRenderer.sortingLayerName = "Player";
            }
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Jump"))
            {
                //get jump data from the jump
                JumpData jumpData = col.GetComponent<JumpData>();
                Jump(jumpData.jumpHeightScale,jumpData.jumpPushScale);
            }
        }
    }
}
