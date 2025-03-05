using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StableFloatingRigidBody : MonoBehaviour
{
    [SerializeField]
    bool floatToSleep = false;

    [SerializeField]
    bool safeFloating = false;

    [SerializeField]
    float submergenceOffset = 0.5f;

    [SerializeField, Min(0.1f)]
    float submergenceRange = 1f;

    [SerializeField, Min(0f)]
    float buoyancy = 1f;

    [SerializeField]
    Vector3[] buoyancyOffsets = default;

    [SerializeField, Range(0f, 10f)]
    float waterDrag = 1f;

    [SerializeField]
    LayerMask waterMask = 0;

    [SerializeField] float waveSpeed = 1f;

    Rigidbody body;

    float floatDelay;

    float[] submergence;

    Vector3 gravity;

    public bool FloatToSleep { get { return floatToSleep; } set { floatToSleep = value; } }
    public bool SafeFloating { get { return safeFloating; } set { safeFloating = value; } }
    public float Buoyancy { get { return buoyancy; } set { buoyancy = value; } }

    [System.Serializable]
    public struct GerstnerWaveParams
    {
        [Tooltip("Direction of wave propagation")]
        public Vector2 direction;

        [Tooltip("Length of the wave (distance between crests)")]
        [Min(0.1f)]
        public float wavelength;

        [Tooltip("Height of the wave")]
        [Min(0f)]
        public float amplitude;

        [Tooltip("Speed of wave movement")]
        public float speed;

        [Tooltip("Steepness of the wave (controls pointiness)")]
        [Range(0f, 1f)]
        public float steepness;
    }

    [Header("Gerstner Wave Settings")]
    [SerializeField]
    private GerstnerWaveParams[] waves = new GerstnerWaveParams[]
    {
        new GerstnerWaveParams
        {
            direction = new Vector2(1f, 0f),
            wavelength = 10f,
            amplitude = 0.5f,
            speed = 1f,
            steepness = 0.5f
        },
        new GerstnerWaveParams
        {
            direction = new Vector2(0.5f, 0.5f),
            wavelength = 5f,
            amplitude = 0.25f,
            speed = 1.5f,
            steepness = 0.3f
        }
    };

    void Awake()
    {
        body = GetComponent<Rigidbody>();
        body.useGravity = false;
        submergence = new float[buoyancyOffsets.Length];
    }

    void FixedUpdate()
    {
        if (floatToSleep)
        {
            if (body.IsSleeping())
            {
                floatDelay = 0f;
                return;
            }

            if (body.velocity.sqrMagnitude < 0.0001f)
            {
                floatDelay += Time.deltaTime;
                if (floatDelay >= 1f)
                {
                    return;
                }
            }
            else
            {
                floatDelay = 0f;
            }
        }

        gravity = CustomGravity.GetGravity(body.position);
        float dragFactor = waterDrag * Time.deltaTime / buoyancyOffsets.Length;
        float buoyancyFactor = -buoyancy / buoyancyOffsets.Length;
        for (int i = 0; i < buoyancyOffsets.Length; i++)
        {
            if (submergence[i] > 0f)
            {
                float drag =
                    Mathf.Max(0f, 1f - dragFactor * submergence[i]);
                body.velocity *= drag;
                body.angularVelocity *= drag;
                body.AddForceAtPosition(
                    gravity * (buoyancyFactor * submergence[i]),
                    transform.TransformPoint(buoyancyOffsets[i]),
                    ForceMode.Acceleration
                );
                submergence[i] = 0f;
            }
        }
        body.AddForce(gravity, ForceMode.Acceleration);
    }

    void OnTriggerEnter(Collider other)
    {
        if ((waterMask & (1 << other.gameObject.layer)) != 0)
        {
            EvaluateSubmergence();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (
            !body.IsSleeping() &&
            (waterMask & (1 << other.gameObject.layer)) != 0
        )
        {
            EvaluateSubmergence();
        }
    }

    void EvaluateSubmergence()
    {
        Vector3 down = gravity.normalized;
        Vector3 offset = down * -submergenceOffset;
        float time = Time.time * waveSpeed;

        for (int i = 0; i < buoyancyOffsets.Length; i++)
        {
            Vector3 worldPoint = transform.TransformPoint(buoyancyOffsets[i]);

            float waveOffset = CalculateWaveHeight(worldPoint.x, worldPoint.z, time);

            Vector3 p = offset + worldPoint + down * waveOffset;

            if (Physics.Raycast(
                p, down, out RaycastHit hit, submergenceRange + 1f,
                waterMask, QueryTriggerInteraction.Collide
            ))
            {
                float adjustedDistance = hit.distance - waveOffset;
                submergence[i] = 1f - adjustedDistance / submergenceRange;
            }
            else if (
                !safeFloating || Physics.CheckSphere(
                    p, 0.01f, waterMask, QueryTriggerInteraction.Collide
                )
            )
            {
                submergence[i] = 1f;
            }
        }
    }
    float CalculateWaveHeight(float x, float z, float time)
    {
        float height = 0f;

        foreach (var wave in waves)
        {
            float frequency = 2f * Mathf.PI / wave.wavelength;
            float wavePhase = frequency * Vector2.Dot(wave.direction, new Vector2(x, z)) - (time * wave.speed);

            height += wave.amplitude * Mathf.Sin(wavePhase) * (1f - wave.steepness);
        }

        return height;
    }
}
