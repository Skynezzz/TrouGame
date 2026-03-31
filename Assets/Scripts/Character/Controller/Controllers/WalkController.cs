using System.Collections;
using System.Linq;
using TrouGame.Character.Controller.Controllers.CameraControllers;
using TrouGame.Miscellaneous;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TrouGame.Character.Controller.Controllers
{
    public class WalkController : ControllerBase
    {
        [SerializeField] private Transform head;
        [SerializeField, Range(0, 1)] private float viewSensitivity = 0.1f;
        [SerializeField] private float maxSpeed = 20f;
        [SerializeField] private float runMaxSpeed = 20f;
        [SerializeField] private float walkForce = 20f;
        [SerializeField] private float runForce = 20f;
        [SerializeField] private float breakStrenght = 0.3f;
        [SerializeField] private float airConstraint = 0.3f;
        [SerializeField] private float garvityForce = 10f;
        [SerializeField] private float jumpForce = 50f;
        [SerializeField] private float voidJumpTime = 0.2f;
        [SerializeField] private float strafStrenght = 1f;
        [Header("Step")]
        [SerializeField] private float stepAssist;
        [SerializeField] private float bigStepAccelerationMult;

        private Transform character;
        private Transform skin;
        private Rigidbody rigidbody;
        private Vector3 velocity
        {
            get => rigidbody.linearVelocity;
            set => rigidbody.linearVelocity = value;
        }

        private FirstAndThirdPersonCameraController cameraController;

        [SerializeField] private Vector2 currentDirection;
        [SerializeField] private bool _isGrounded = false;
        [SerializeField] private bool canJump = false;
        [SerializeField] private bool isRunning = false;
        [SerializeField] private bool isFirstPerson = true;

        private Coroutine voidJumpTimerCoroutine; 

        private bool isGrounded
        {
            get
            {
                return _isGrounded;
            }
            set
            {
                if (value != isGrounded)
                {
                    _isGrounded = value;
                    if (_isGrounded)
                    {
                        if (voidJumpTimerCoroutine != null)
                            StopCoroutine(voidJumpTimerCoroutine);
                        canJump = true;
                    }
                    else
                    {
                        if (canJump)
                        {
                            voidJumpTimerCoroutine = StartCoroutine(VoidJumpTimer());
                        }
                    }
                }
            }
        }

        public override void MouseMove(Vector2 playerInput)
        {
            cameraController.Rotate(playerInput * viewSensitivity);
        }

        public override void DirectionChange(Vector2 playerInput)
        {
            //if (playerInput == Vector2.zero)
            //    isRunning = false;

            currentDirection = playerInput;
        }

        public override void Jump()
        {
            if (canJump)
            {
                if (velocity.y < 0)
                    velocity = new(velocity.x, 0f, velocity.z);
                rigidbody.AddForce(Vector3.up * jumpForce * 10f);
                character.transform.position += Vector3.up * 0.1f;
                canJump = false;
            }
        }

        public override void Sprint()
        {
            isRunning = !isRunning;
        }

        public override void Crouch()
        {
            //isRunning = false;
            cameraController.SwitchCamera();
            //rigidbody.AddForce(Vector3.up * jumpForce * 10f);
        }

        public override void ControllerStart()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        public override void ControllerStop()
        {
            Cursor.lockState = CursorLockMode.None;
        }

        private void Start()
        {
            character = Anchor.TRANSFORMS["Character"];
            skin = Anchor.TRANSFORMS["Skin"];
            rigidbody = character.GetComponent<Rigidbody>();
            cameraController = Anchor.TRANSFORMS["LookAt"].GetComponent<FirstAndThirdPersonCameraController>();
            cameraController.OnRotationChange += HandleOnLookAtRotate;
        }

        private void FixedUpdate()
        {
            Vector3 initialVelocity = velocity;

            // Grounded Check //
            if (Physics.BoxCast(character.position, new Vector3(0.15f, 0.01f, 0.15f), Vector3.down, out RaycastHit hitInfo, Quaternion.identity, 1.01f))
            {
                if (initialVelocity.y <= 0f || (initialVelocity.y > 0f && hitInfo.distance <= 1f))
                {
                    isGrounded = true;
                    character.position += Vector3.up * (1 - hitInfo.distance);
                    initialVelocity = new Vector3(initialVelocity.x, 0, initialVelocity.z);

                    // Climb Step //
                    if (hitInfo.distance < 1 - stepAssist)
                    {
                        float climbHeight = hitInfo.distance + stepAssist;
                        initialVelocity = initialVelocity * (climbHeight * bigStepAccelerationMult);
                    }
                }
            }
            else
            {
                isGrounded = false;
            }

            // Gravity //
            if (!isGrounded)
                initialVelocity += Vector3.down * garvityForce * Time.fixedDeltaTime;

            if (currentDirection != Vector2.zero)
            {
                // Skin Rotation //
                skin.rotation = Quaternion.Euler(character.rotation.eulerAngles.x,
                    cameraController.transform.rotation.eulerAngles.y,
                    character.rotation.eulerAngles.z);
                HandleOnLookAtRotate(cameraController.transform);

                // Straf //
                /// Angle
                Vector3 worldCurrentDirection = (currentDirection.y * skin.transform.forward
                    + currentDirection.x * skin.transform.right).normalized;
                float rotationWCD = Quaternion.LookRotation(worldCurrentDirection).eulerAngles.y;
                float rotationVelocity = Quaternion.LookRotation(new Vector3(initialVelocity.x, 0, initialVelocity.z)).eulerAngles.y;
                float gapAngle = rotationWCD - rotationVelocity;
                if (Mathf.Abs(gapAngle) > 180f)
                {
                    if (rotationWCD > rotationVelocity)
                        rotationWCD -= 360f;
                    else
                        rotationVelocity -= 360f;

                    gapAngle = rotationWCD - rotationVelocity;
                }

                /// Speed Keeping Calcul
                float speedKeptPercentage;
                float angleFullStraf = 45;
                if (Mathf.Abs(gapAngle) - angleFullStraf <= 0f)
                    speedKeptPercentage = 1f;
                else
                    speedKeptPercentage = 1f - (((Mathf.Abs(gapAngle) - angleFullStraf) / 180f) / ((180f - angleFullStraf) / 180f));

                float speedKept = new Vector3(initialVelocity.x, 0, initialVelocity.z).magnitude * speedKeptPercentage;

                // Walk Acceleration //
                float currentMaxSpeed = maxSpeed;
                float currentWalkForce = walkForce;
                if (isRunning)
                {
                    currentMaxSpeed = runMaxSpeed;
                    currentWalkForce = runForce;
                }
                if (speedKept < currentMaxSpeed)
                {
                    speedKept += (isGrounded ? currentWalkForce : currentWalkForce * airConstraint) * Time.fixedDeltaTime;
                    if (speedKept > currentMaxSpeed)
                        speedKept = currentMaxSpeed;
                }
                else
                {
                    // Friction //
                    speedKept -= isGrounded ? breakStrenght : breakStrenght * airConstraint;
                    if (speedKept < currentMaxSpeed)
                        speedKept = currentMaxSpeed;
                }

                Vector3 newDirection = worldCurrentDirection * strafStrenght + new Vector3(initialVelocity.x, 0, initialVelocity.z);
                initialVelocity = newDirection.normalized * speedKept + Vector3.up * initialVelocity.y;
            }
            else
            {
                // Break //
                Vector3 initialVelocity2D = new Vector3(initialVelocity.x, 0, initialVelocity.z);
                var currentBreakStrenght = (isGrounded ? breakStrenght : breakStrenght * airConstraint);

                if (initialVelocity2D.magnitude - (initialVelocity2D.normalized * currentBreakStrenght).magnitude < 0f)
                    initialVelocity = Vector3.up * initialVelocity.y;
                else
                    initialVelocity = initialVelocity2D - initialVelocity2D.normalized * currentBreakStrenght + Vector3.up * initialVelocity.y;
            }
            velocity = initialVelocity;
        }

        private void HandleOnLookAtRotate(Transform lookAt)
        {
            head.rotation = Quaternion.Euler(head.rotation.eulerAngles.x, lookAt.rotation.eulerAngles.y - 90f, -90f - lookAt.rotation.eulerAngles.x);

            float lookAngle = lookAt.rotation.eulerAngles.y;
            float skinAngle = skin.rotation.eulerAngles.y;
            float gapAngle = lookAngle - skinAngle;
            if (Mathf.Abs(gapAngle) > 180f)
            {
                if (lookAngle > skinAngle)
                {
                    lookAngle -= 360f;
                }
                else
                {
                    skinAngle -= 360f;
                }
                gapAngle = lookAngle - skinAngle;
            }
            if (Mathf.Abs(gapAngle) > 30f)
            {
                if (gapAngle > 0)
                {
                    skin.Rotate(Vector3.up, gapAngle - 30f);
                }
                else
                {
                    skin.Rotate(Vector3.up, gapAngle + 30f);
                }
            }
        }

        private IEnumerator VoidJumpTimer()
        {
            yield return new WaitForSeconds(voidJumpTime);
            canJump = false;
        }
    }
}