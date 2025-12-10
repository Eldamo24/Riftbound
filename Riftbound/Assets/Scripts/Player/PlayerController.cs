using Riftbound.Core;
using UnityEngine;

namespace Riftbound.Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInputHandler))]
    public class PlayerController : MonoBehaviour, IMovable, IAttack, IDamageable
    {
        [Header("References")]
        [SerializeField] private Transform cameraTransform;

        [Header("Stats")]
        [SerializeField] private CharacterStats stats;

        [Header("Movement")]
        [SerializeField] private float acceleration;
        [SerializeField] private float deceleration;
        [SerializeField] private float rotationSpeed;

        [Header("Jump & Gravity")]
        [SerializeField] private float jumpHeight;
        [SerializeField] private float gravity;
        [SerializeField] private float groundedCheckDistance;
        [SerializeField] private LayerMask groundLayer;

        [SerializeField] private Animator animator;

        private CharacterController controller;
        private PlayerInputHandler input;

        private Vector2 moveInputRaw;
        private Vector3 moveDirection;
        private Vector3 moveVelocity;
        private Vector3 verticalVelocity;

        private bool jumpRequested;
        private bool isGrounded;
        private float currentHealth;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            input = GetComponent<PlayerInputHandler>();
            currentHealth = stats.maxHealth;
            if (cameraTransform == null && Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
            }
        }

        private void OnEnable()
        {
            input.OnMove += HandleMoveInput;
            input.OnJump += HandleJumpInput;
            input.OnAttack += Attack;
        }

        private void OnDisable()
        {
            input.OnMove -= HandleMoveInput;
            input.OnJump -= HandleJumpInput;
            input.OnAttack -= Attack;
        }

        private void Update()
        {
            GroundCheck();
            CalculateCameraRelativeDirection();
            HandleHorizontalMovement();
            HandleVerticalMovement();
            ApplyMovement();
            RotateTowardsMovement();
            UpdateAnimator();
        }

        private void HandleMoveInput(Vector2 inputDir)
        {
            moveInputRaw = inputDir;
        }

        private void HandleJumpInput()
        {
            if(isGrounded)
                jumpRequested = true;
        }

        private void GroundCheck()
        {
            isGrounded = controller.isGrounded;
            if (!isGrounded)
            {
                Vector3 origin = transform.position + Vector3.up * 0.1f;
                if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, groundedCheckDistance, groundLayer))
                {
                    isGrounded = true;
                }
            }

            if (isGrounded && verticalVelocity.y < 0)
            {
                verticalVelocity.y = -2f;
            }
            if (animator != null)
            {
                animator.SetBool("Grounded", isGrounded);
            }
        }

        private void CalculateCameraRelativeDirection()
        {
            if (moveInputRaw.sqrMagnitude < 0.0001f || cameraTransform == null)
            {
                moveDirection = Vector3.zero;
                return;
            }
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();
            Vector3 desiredDirection =
                camForward * moveInputRaw.y +
                camRight * moveInputRaw.x;
            moveDirection = desiredDirection.normalized;
        }

        private void HandleHorizontalMovement()
        {
            Vector3 targetVelocity = moveDirection * stats.moveSpeed;
            float lerpSpeed = (moveDirection.sqrMagnitude > 0.001f) ? acceleration : deceleration;
            moveVelocity = Vector3.Lerp(moveVelocity, targetVelocity, lerpSpeed * Time.deltaTime);
        }

        private void HandleVerticalMovement()
        {
            if (isGrounded && jumpRequested)
            {
                verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                jumpRequested = false;
                if (animator != null)
                {
                    animator.SetTrigger("Jump");
                }
            }
            verticalVelocity.y += gravity * Time.deltaTime;
        }

        private void ApplyMovement()
        {
            Vector3 finalVelocity = moveVelocity + verticalVelocity;
            controller.Move(finalVelocity * Time.deltaTime);
        }

        private void RotateTowardsMovement()
        {
            Vector3 horizontalVelocity = moveVelocity;
            horizontalVelocity.y = 0f;
            if (horizontalVelocity.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(horizontalVelocity);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }
        }

        private void UpdateAnimator()
        {
            if (animator == null)
                return;
            Vector3 horizontalVelocity = moveVelocity;
            horizontalVelocity.y = 0f;

            float speed = horizontalVelocity.magnitude;
            animator.SetFloat("Speed", speed);
        }

        public void Move(Vector3 direction)
        {
            moveDirection = direction.normalized;
        }

        public void Attack()
        {
            Debug.Log("Ataque básico de lanza (placeholder)");
            // Más adelante: animación + hitbox
        }

        public void TakeDamage(float amount)
        {
            currentHealth -= amount;
            Debug.Log($"Player recibe daño: {amount}, vida actual: {currentHealth}");

            if (currentHealth <= 0f)
            {
                currentHealth = 0f;
                Debug.Log("Player muerto (placeholder)");
                // Event OnPlayerDied, respawn, etc.
            }
        }
    }
}

