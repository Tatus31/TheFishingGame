using UnityEngine;

[CreateAssetMenu(fileName = "FishingSettings", menuName = "Fishing/FishingSettings", order = 1)]
public class FishingSettings : ScriptableObject
{
    [Header("Throw Settings")]
    public float maxLineLength = 10f;
    public float minLineLength = 1f;
    public float lineGrowthRate = 5f;
    public float minTrajectoryHeight = 1f;
    public float cooldownTime = 2f;
    [Header("Camera Settings")]
    public float rotationSpeed = 3f;
    [Header("Reel Settings")]
    public float reelInTime = 2f;
    public float minStartFleeingTime = 2.5f;
    public float maxStartFleeingTime = 4.35f;
    [Header("Flee Settings")]
    public float minFleeTime = 2f;
    public float maxFleeTime = 5f;
    public float fleeRadius = 1.75f;
    public float fleeTimes = 3;

    [Header("Gizmo Visuals")]
    public Color lineColor = Color.red;
}
