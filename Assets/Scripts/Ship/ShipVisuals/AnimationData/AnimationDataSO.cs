using DG.Tweening;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Animation Data", menuName = "AnimationDataSO/Create Animation Data", order = 3)]
public class AnimationDataSO : ScriptableObject
{
    public AnimationData[] AnimationData;

    [Header("Preview Settings")]
    public Transform previewTarget; 
    public bool loopPreview = false;
}

[Serializable]
public class AnimationData
{
    public string TransformName;
    [Space(5f)]
    public Vector3 MoveToPosition;       
    public Quaternion RotateToRotation;  
    [Space(5f)]
    public float MoveDuration;
    public float RotateDuration;
    [Space(5f)]
    public Ease MoveEase;
    public Ease RotateEase;
    [Space(5f)]
    public bool UseWorldSpace = false;
}
