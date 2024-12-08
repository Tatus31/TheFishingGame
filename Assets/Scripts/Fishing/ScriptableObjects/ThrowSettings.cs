using UnityEngine;

[CreateAssetMenu(fileName = "ThrowSettings", menuName = "Fishing/ThrowSettings", order = 1)]
public class ThrowSettings : ScriptableObject
{
    [Header("Throw Settings")]
    public float maxLineLength = 10f;
    public float minLineLength = 1f;
    public float lineGrowthRate = 5f;
    public float minTrajectoryHeight = 1f;
    public float reelInTime = 2f;
    public float rotationSpeed = 3f;
    public float cooldownTime = 2f;

    [Header("Gizmo Visuals")]
    public Color lineColor = Color.red;
}
