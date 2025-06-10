using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColisionShake : CameraEffectsBaseState
{
    float shakeTimer;
    float shakeDuration;

    public override void EnterState(CameraEffectsManager cameraEffect)
    {
        shakeDuration = cameraEffect.CollisionShakeDuration; 
        shakeTimer = 0f;

        cameraEffect.GetVirtualCamera().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = cameraEffect.CollisionFrequency;
        cameraEffect.GetVirtualCamera().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_PivotOffset = new Vector3(10, 8.5f, 6.75f);
    }

    public override void UpdateState(CameraEffectsManager cameraEffect)
    {
        shakeTimer += Time.deltaTime;

        if (shakeTimer >= shakeDuration)
        {
            cameraEffect.GetVirtualCamera().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0f;
            cameraEffect.SwitchState(cameraEffect.headBobState); 
        }
    }
}