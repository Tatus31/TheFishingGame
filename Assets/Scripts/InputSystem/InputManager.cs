using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    PlayerInputActions playerInputActions;

    void Awake()
    {
        if (Instance != null)
            Debug.LogWarning($"there exists a {Instance.name} in the scene already");

        Instance = this;

        playerInputActions = new PlayerInputActions();
    }

    void OnEnable()
    {
        playerInputActions.Enable();
    }

    void OnDisable()
    {
        playerInputActions.Disable();
    }

    public Vector2 GetPlayerMovement()
    {
        return playerInputActions.Player.Movement.ReadValue<Vector2>();
    }

    public Vector2 GetMouseDelta()
    {
        return playerInputActions.Player.Camera.ReadValue<Vector2>();
    }

    public bool IsHoldingSprintKey()
    {
        return playerInputActions.Player.Sprint.IsPressed();
    }

    public bool IsLeftMouseButtonPressed()
    {
        return playerInputActions.Player.LeftMouse.WasPerformedThisFrame();
    }
}
