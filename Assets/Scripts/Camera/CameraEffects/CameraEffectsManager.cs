using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffectsManager : MonoBehaviour
{
    public static CameraEffectsManager Instance { get; private set; }

    [Header("HeadBob Frequency")]
    [SerializeField] float walkFrequency = 0.01f;
    [SerializeField] float sprintFrequency = 0.09f;
    [Header("Collision Frequency")]
    [SerializeField] float collisionFrequency = 3f;
    [SerializeField] float collisionShakeDuration = 0.5f;

    public float WalkFrequency {  get { return walkFrequency; } set {  walkFrequency = value; } }
    public float SprintFrequency { get { return sprintFrequency; } set {  sprintFrequency = value; } }
    public float CollisionFrequency { get { return collisionFrequency; } set { collisionFrequency = value; } }
    public float CollisionShakeDuration { get { return collisionShakeDuration; } set { collisionShakeDuration = value; } }

    CinemachineVirtualCamera virtualCamera;
    InputManager inputManager;
    Throw throwState;

    CameraEffectsBaseState currentState;

    public HeadBob headBobState = new HeadBob();
    public CameraShake cameraShakeState = new CameraShake();
    public ColisionShake colisionShakeState = new ColisionShake();

    void Awake()
    {
        if (Instance != null)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"there exists a {Instance.name} in the scene already");
#endif
        }

        Instance = this;

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
