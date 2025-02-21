using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffectsManager : MonoBehaviour
{
    [Header("HeadBob Frequency")]
    [SerializeField] float walkFrequency = 0.01f;
    [SerializeField] float sprintFrequency = 0.09f;

    public float WalkFrequency {  get { return walkFrequency; } set {  walkFrequency = value; } }
    public float SprintFrequency { get { return sprintFrequency; } set {  sprintFrequency = value; } }

    CinemachineVirtualCamera virtualCamera;
    InputManager inputManager;
    Throw throwState;

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

    //public FishingStateManager GetFishingStateManager() { return fishingStateManager; }
}
