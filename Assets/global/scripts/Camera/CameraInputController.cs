using UnityEngine;

namespace PlayerLocomotion
{
    public class CameraInputController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InputManager inputManager;
        [SerializeField] private Rigidbody player;
        [SerializeField] private Transform cameraPivot;

        [Header("Normal Camera")]
        [SerializeField] private float normalPitch = 15f;
        [SerializeField] private float orbitSpeed = 120f;
        [SerializeField] private float recenterDelay = 3f;
        [SerializeField] private float recenterSpeed = 180f;

        [Header("Orbit Restrictions")]
        [SerializeField] private float maxOrbitAngle = 100f;

        

        [Header("Camera Settings")]
        [SerializeField] private float minPitch = 5f;
        [SerializeField] private float maxPitch = 85f;
        [SerializeField] private float followHeight = 1.5f;
        [SerializeField] private float pitchSmoothSpeed = 5f;

        // Camera input
        private Vector2 lookInput;

        // Camera rotation
        private float orbitOffset;
        private float currentPitch;
        private float targetPitch;

        // Camera state
        private float lastLookTime;
    

        private void Start()
        {
            currentPitch = normalPitch;
            targetPitch = normalPitch;

            lastLookTime = Time.time;
        }

        // Called through InputManager UnityEvent
        public void OnCameraMove()
        {
            lookInput = inputManager.ReadCameraMovement();
        }

      
        private void HandleCameraInput()
        {
            // Check whether the player is actively moving the camera
            bool isLooking = lookInput.sqrMagnitude > 0.001f;

            if (isLooking)
            {
                // Horizontal orbit
                orbitOffset += lookInput.x * orbitSpeed * Time.deltaTime;

                // Restrict normal camera to rear hemisphere

                orbitOffset = Mathf.Clamp( orbitOffset,-maxOrbitAngle, maxOrbitAngle);
               

                // Vertical orbit
                targetPitch += lookInput.y * orbitSpeed * Time.deltaTime;

                targetPitch = Mathf.Clamp(targetPitch, minPitch, maxPitch);

                // Reset recenter timer only when player moves camera
                lastLookTime = Time.time;
            }

            // Recenter after inactivity
            orbitOffset = Mathf.MoveTowardsAngle( orbitOffset, 0f,recenterSpeed * Time.deltaTime );
           
            // Smooth pitch
            currentPitch = Mathf.Lerp( currentPitch,targetPitch,pitchSmoothSpeed * Time.deltaTime);
        }

        private void UpdateCameraPivot()
        {
            if (player == null || cameraPivot == null)
                return;

            // Follow player's position
            cameraPivot.position =
                player.position + Vector3.up * followHeight;

            // Get player's Y rotation
            float playerYaw = player.rotation.eulerAngles.y;

            // Combine player rotation and camera orbit
            float cameraYaw = playerYaw + orbitOffset;

            // Apply rotation without pitch or roll from player
            cameraPivot.rotation = Quaternion.Euler(currentPitch,cameraYaw,0f);
        }

        private void LateUpdate()
        {
            HandleCameraInput();
            UpdateCameraPivot();
        }
    }
}
