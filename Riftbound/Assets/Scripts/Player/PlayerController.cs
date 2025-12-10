using System;
using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
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

    private CharacterController controller;
    private PlayerInputHandler input;

    private Vector3 moveInput;
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
        HandleHorizontalMovement();
        HandleVerticalMovement();
        ApplyMovement();
        RotateTowardsMovement();
    }


    private void HandleMoveInput(Vector2 inputDir)
    {
        moveInput = new Vector3(inputDir.x, 0f, inputDir.y);
    }


    private void HandleJumpInput()
    {
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
    }

    private void HandleHorizontalMovement()
    {
        Vector3 targetVelocity = moveInput.normalized * stats.moveSpeed;
        float lerpSpeed = (moveInput.magnitude > 0.1f) ? acceleration : deceleration;
        moveVelocity = Vector3.Lerp(moveVelocity, targetVelocity, lerpSpeed * Time.deltaTime);
    }

    private void HandleVerticalMovement()
    {
        if (isGrounded && jumpRequested)
        {
            verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpRequested = false;
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
        Vector3 direction = moveVelocity;
        direction.y = 0f;
        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }

    public void Move(Vector3 direction)
    {
        moveInput = direction;
    }

    public void Attack()
    {
        Debug.Log("Ataque básico de lanza (placeholder)");
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log($"Player recibe daño: {amount}, vida actual: {currentHealth}");
        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            Debug.Log("Player muerto (placeholder)");
        }
    }

}
