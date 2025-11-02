using System;
using UnityEngine;
using UnityEngine.Serialization;

public class LightsManager : MonoBehaviour
{
    public static event EventHandler<bool> OnLightsToggled;
    public static event EventHandler<bool> OnLightsFlicker;

    [SerializeField] GameObject lightsPrefab;
    [SerializeField] float flickerTimer = 0.2f;
    [SerializeField] float lightProtectionRadius = 20f;
    
    private ShipCorruptionProtection _shipCorruptionProtectionCached;
        
    float _flickerTime;
    
    bool _areLightsOn;
    bool _startFlickering;

    private void Start()
    {
        if (lightsPrefab == null)
        {
            return;
        }
        
        if(TryGetComponent<ShipCorruptionProtection>(out ShipCorruptionProtection shipCorruptionProtection))
        {
            _shipCorruptionProtectionCached = shipCorruptionProtection;
        }

        ElectricalDevice.OnDegradation += ElectricalDevice_OnDegradation;
    }

    private void ElectricalDevice_OnDegradation(object sender, EventArgs e)
    {
        ElectricalDevice electricalDevice = (ElectricalDevice)sender;

        if(electricalDevice)
        {
            if (electricalDevice.CurrentDegradation == ElectricalDevice.DegradationCondition.Bad)
            {
                _startFlickering = true;
            }
            else
            {
                _startFlickering = false;
            }
        }
    }

    void Update()
    {
        if (InputManager.Instance.GetLightsInputDown())
        {
            ToggleLights();
        }

        if (_startFlickering)
        {
            _flickerTime += Time.deltaTime;
            if (_flickerTime >= flickerTimer)
            {
                lightsPrefab.SetActive(!lightsPrefab.activeSelf);
                _flickerTime = 0;
            }

            OnLightsFlicker?.Invoke(this, _startFlickering);
        }

    }

    public void ToggleLights()
    {
        lightsPrefab.SetActive(!lightsPrefab.activeSelf);
        _areLightsOn = lightsPrefab.activeSelf;
        OnLightsToggled?.Invoke(this, _areLightsOn);

        float previous = 8f;
            
        if (_shipCorruptionProtectionCached)
        {
            previous = _shipCorruptionProtectionCached.radius;
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogWarning("The lights are cached but the lights are not in cache");
#endif
        }

        _shipCorruptionProtectionCached.radius = _areLightsOn ? lightProtectionRadius : previous;
        
    }
}
