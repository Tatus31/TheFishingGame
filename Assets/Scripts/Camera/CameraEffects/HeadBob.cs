using Cinemachine;
using UnityEngine;

public class HeadBob : CameraEffectsBaseState
{
    Vector3 flatVel;
    bool isThrowing;

    void Throw_OnThrowing(object sender, bool e)
    {
        isThrowing = e;
    }

    void PlayerMovement_OnPlayerSpeedChange(object sender, Vector3 e)
    {
        flatVel = e;
    }

    public override void EnterState(CameraEffectsManager cameraEffect)
    {
        PlayerMovement.Instance.OnPlayerSpeedChange += PlayerMovement_OnPlayerSpeedChange;
        cameraEffect.GetRodThrow().OnThrowing += Throw_OnThrowing;
    }

    public override void UpdateState(CameraEffectsManager cameraEffect)
    {
        if (isThrowing)
        {
            cameraEffect.SwitchState(cameraEffect.cameraShakeState);
        }

        if (cameraEffect.GetInputManager().IsHoldingSprintKey() && flatVel.magnitude > 0.01f)
        {
            cameraEffect.GetVirtualCamera().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = cameraEffect.SprintFrequency;
        }
        else
        {
            cameraEffect.GetVirtualCamera().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = cameraEffect.WalkFrequency;
        }
    }
}