using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCanvasTowardsPlayer : MonoBehaviour
{
    public enum CanvasType
    {
        ShipControls,
        Heart,
        pump,
        Harpoon,
        repair,

    }

    [SerializeField] Transform playerTransform;
    [SerializeField] float rotationSpeed = 5f;
    public CanvasType canvasType;

    private void Start()
    {

    }

    void Update()
    {
        if (playerTransform != null)
        {
            Vector3 directionToPlayer = transform.position - playerTransform.position;

            //directionToPlayer.y = 0f;

            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }
}