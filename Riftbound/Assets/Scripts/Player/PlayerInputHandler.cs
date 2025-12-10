using System;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    public event Action<Vector2> OnMove;
    public event Action OnJump;
    public event Action OnAttack;

    private Controls input;

    private void Awake()
    {
        input = new Controls();
    }

    private void OnEnable()
    {
        input.Enable();

        input.Gameplay.Move.performed += ctx => OnMove?.Invoke(ctx.ReadValue<Vector2>());
        input.Gameplay.Move.canceled += ctx => OnMove?.Invoke(Vector2.zero);
        input.Gameplay.Jump.performed += ctx => OnJump?.Invoke();
        input.Gameplay.Attack.performed += ctx => OnAttack?.Invoke();
    }

}
