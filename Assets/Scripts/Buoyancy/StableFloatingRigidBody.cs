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

    [SerializeField] float waveHeight = 0.5f;
    [SerializeField] float waveFrequency = 1f;
    [SerializeField] float waveSpeed = 1f;

    Rigidbody body;

    float floatDelay;

    float[] submergence;

    Vector3 gravity;

    public bool FloatToSleep {  get { return floatToSleep; } set {  floatToSleep = value; } }
    public bool SafeFloating { get {  return safeFloating; } set {  safeFloating = value; } }
    public float Buoyancy {  get { return buoyancy; } set {  buoyancy = value; } }

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
        return waveHeight * Mathf.Sin(x * waveFrequency + time)
               * Mathf.Sin(z * waveFrequency + time);

        // return waveHeight * (
        //     Mathf.Sin(x * waveFrequency + time) * 0.5f + 
        //     Mathf.Sin(x * waveFrequency * 2.7f + time * 0.8f) * 0.25f +
        //     Mathf.Sin(z * waveFrequency * 0.6f + time * 1.3f) * 0.25f
        // );
    }
}
