using Cinemachine;
using System;
using UnityEngine;

public class HeadBob : MonoBehaviour
{
    [Header("HeadBob Frequency")]
    [SerializeField] float walkFrequency = 0.03f;
    [SerializeField] float sprintFrequency = 0.13f;

    CinemachineVirtualCamera virtualCamera;
    InputManager inputManager;

    Vector3 flatVel;

    void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    void Start()
    {
        PlayerMovement.Instance.OnPlayerSpeedChange += PlayerMovement_OnPlayerSpeedChange;

        inputManager = InputManager.Instance;
    }

    private void PlayerMovement_OnPlayerSpeedChange(object sender, Vector3 e)
    {
        flatVel = e;
    }

    void Update()
    {
        if (inputManager.IsHoldingSprintKey() && flatVel.magnitude > 0.01f)
        {
            //Debug.Log("sprintFrequency");

            virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = sprintFrequency;
        }
        else
        {
            //Debug.Log("walkFrequency");

            virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = walkFrequency;
        }
    }
}
