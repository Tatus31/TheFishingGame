using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffectsManager : MonoBehaviour
{
    [Header("HeadBob Frequency")]
    [SerializeField] float walkFrequency;
    [SerializeField] float sprintFrequency;

    public float WalkFrequency { get => walkFrequency = 0.03f; set => value = walkFrequency; }
    public float SprintFrequency { get => sprintFrequency = 0.13f; set => value = sprintFrequency; }

    CinemachineVirtualCamera virtualCamera;
    InputManager inputManager;
    Throw throwState;
    FishingStateManager fishingStateManager;

    CameraEffectsBaseState currentState;

    public HeadBob headBobState = new HeadBob();
    public CameraShake cameraShakeState = new CameraShake();

    void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    void Start()
    {
        inputManager = InputManager.Instance;
        fishingStateManager = FindObjectOfType<FishingStateManager>();
        throwState = fishingStateManager.throwState;

        if (throwState == null)
            Debug.LogError("Throw component not found!");

        currentState = headBobState;
        currentState.EnterState(this);
    }


    void Update()
    {
        currentState.UpdateState(this);
    }

    public void SwitchState(CameraEffectsBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }

    public CinemachineVirtualCamera GetVirtualCamera(){ return virtualCamera; }

    public InputManager GetInputManager() { return inputManager; }

    public Throw GetRodThrow() { return throwState; }

    public FishingStateManager GetFishingStateManager() { return fishingStateManager; }
}
