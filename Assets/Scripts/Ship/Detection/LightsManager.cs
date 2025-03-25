using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsManager : MonoBehaviour
{
    public static event EventHandler<bool> OnLightsToggled;
    public static event EventHandler<bool> OnLightsFlicker;

    [SerializeField] GameObject lightsPrefab;
    [SerializeField] float flickerTimer = 0.2f;
    float flickerTime;

    bool areLightsOn;
    bool startFlickering;

    private void Start()
    {
        ElectricalDevice.OnDegradation += ElectricalDevice_OnDegradation;
    }

    private void ElectricalDevice_OnDegradation(object sender, EventArgs e)
    {
        ElectricalDevice electricalDevice = (ElectricalDevice)sender;

        if(electricalDevice != null)
        {
            if (electricalDevice.CurrentDegradation == ElectricalDevice.DegradationCondition.Bad)
            {
                startFlickering = true;
            }
            else
            {
                startFlickering = false;
            }
        }
    }

    void Update()
    {
        if (lightsPrefab == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            ToggleLights();
        }

        if (startFlickering)
        {
            flickerTime += Time.deltaTime;
            if (flickerTime >= flickerTimer)
            {
                lightsPrefab.SetActive(!lightsPrefab.activeSelf);
                flickerTime = 0;
            }

            OnLightsFlicker?.Invoke(this, startFlickering);
        }

    }

    public void ToggleLights()
    {
        lightsPrefab.SetActive(!lightsPrefab.activeSelf);
        areLightsOn = lightsPrefab.activeSelf;
        OnLightsToggled?.Invoke(this, areLightsOn);
    }
}
