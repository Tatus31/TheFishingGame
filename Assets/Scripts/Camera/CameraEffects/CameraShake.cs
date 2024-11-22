using Cinemachine;
using System;
using UnityEngine;

public class CameraShake : CameraEffectsBaseState
{
    float throwStrength;
    float previousThrowStrength;
    bool isThrowing;

    void Throw_OnThrowStrenghtChange(object sender, float e)
    {
        throwStrength = e;
    }

    void Throw_OnThrowing(object sender, bool e)
    {
        isThrowing = e;
    }

    public override void EnterState(CameraEffectsManager cameraEffect)
    {
        cameraEffect.GetRodThrow().OnThrowStrenghtChange += Throw_OnThrowStrenghtChange;
        cameraEffect.GetRodThrow().OnThrowing += Throw_OnThrowing;
    }

    public override void UpdateState(CameraEffectsManager cameraEffect)
    {
        var perlin = cameraEffect.GetVirtualCamera().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        float targetFrequency;

        if (!isThrowing)
        {
            targetFrequency = Mathf.Max(perlin.m_FrequencyGain - 0.15f, cameraEffect.WalkFrequency);
            if(targetFrequency < cameraEffect.WalkFrequency)
            {
                cameraEffect.SwitchState(cameraEffect.headBobState);
            }
        }
        else
        {
            float strengthDifference = throwStrength - previousThrowStrength;
            if (Mathf.Abs(strengthDifference) > 0)
            {
                targetFrequency = Mathf.Min(perlin.m_FrequencyGain + 0.1f, 1);
            }
            else
            {
                targetFrequency = Mathf.Max(perlin.m_FrequencyGain - 0.15f, cameraEffect.WalkFrequency);
            }
        }

        perlin.m_FrequencyGain = Mathf.Lerp(perlin.m_FrequencyGain, targetFrequency, Time.deltaTime * 7f);

        previousThrowStrength = throwStrength;
    }
}
