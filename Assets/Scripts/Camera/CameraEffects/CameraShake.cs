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
        var throwState = cameraEffect.GetRodThrow();

        throwState.OnThrowStrenghtChange += Throw_OnThrowStrenghtChange;
        throwState.OnThrowing += Throw_OnThrowing;

        previousThrowStrength = throwStrength;
        isThrowing = false;
    }

    public override void UpdateState(CameraEffectsManager cameraEffect)
    {
        var perlin = cameraEffect.GetVirtualCamera().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        float targetFrequency = Mathf.Max(perlin.m_FrequencyGain - 0.15f, cameraEffect.WalkFrequency);

        float strengthDifference = throwStrength - previousThrowStrength;
        if (Mathf.Abs(strengthDifference) > 0.01f)
        {
            targetFrequency = Mathf.Min(perlin.m_FrequencyGain + 0.1f, 1f);
        }
        else
        {
            targetFrequency = Mathf.Max(perlin.m_FrequencyGain - 0.15f, cameraEffect.WalkFrequency);
        }

        perlin.m_FrequencyGain = Mathf.Lerp(perlin.m_FrequencyGain, targetFrequency, Time.deltaTime * 7f);

        previousThrowStrength = throwStrength;

        if (targetFrequency <= cameraEffect.WalkFrequency + 0.01f && !isThrowing)
        {
            cameraEffect.SwitchState(cameraEffect.headBobState);
        }
    }

}
