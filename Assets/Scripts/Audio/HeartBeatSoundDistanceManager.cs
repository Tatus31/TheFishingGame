using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class HeartBeatSoundDistanceManager : MonoBehaviour
{
    [SerializeField]
    Transform monsterTransform;
    [SerializeField]
    Transform shipTransform;

    [SerializeField]
    float maxDistance = 1000f;

    [SerializeField]
    float minPitch = 0.5f;
    [SerializeField]
    float maxPitch = 1.3f;

    float maxDistanceSqr;
    float invertedMaxDistanceSqr; 

    private void Start()
    {
        if (monsterTransform == null)
        {
            Debug.LogError("Monster Transform is not assigned in HeartBeatSoundDistanceManager.");
        }
        if (shipTransform == null)
        {
            Debug.LogError("Ship Transform is not assigned in HeartBeatSoundDistanceManager.");
        }

        maxDistanceSqr = maxDistance * maxDistance;
        invertedMaxDistanceSqr = 1f / maxDistanceSqr;
    }

    void Update()
    {
        float distanceSqr = (monsterTransform.position - shipTransform.position).sqrMagnitude;
        float normalizedDistance = Mathf.Clamp01(distanceSqr * invertedMaxDistanceSqr);
        float pitch = Mathf.Lerp(maxPitch, minPitch, normalizedDistance);

        AudioManager.ChangeAudioPitch(AudioManager.HeartBeatSlowSound, pitch);
    }

    private void OnDrawGizmos()
    {
        if (monsterTransform != null)
        {
            Gizmos.DrawLine(monsterTransform.position, shipTransform.position);

            if ((monsterTransform.position - shipTransform.position).sqrMagnitude < maxDistanceSqr)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(monsterTransform.position, shipTransform.position);
            }
            else
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(monsterTransform.position, shipTransform.position);
            }

            string distanceLabel = $"Distance: {(monsterTransform.position - shipTransform.position).sqrMagnitude:F2} < {maxDistanceSqr}";

            Vector3 labelPosition = shipTransform.position - Vector3.down * 0.5f;
            Handles.Label(labelPosition, distanceLabel);
        }
    }
}
