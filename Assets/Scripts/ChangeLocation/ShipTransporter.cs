using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipTransporter : MonoBehaviour
{
    [SerializeField] public Transform targetPoint;
    [SerializeField] LayerMask collisionLayerMask;
    [SerializeField] GameObject transportedObject;
    [SerializeField] float teleportCooldown = 0.5f;

    public bool isUnderDeck = false;
    bool isInsideTransportArea = false;
    Collider playerCollider = null;
    float lastTeleportTime = 0f;

    private void Update()
    {
        if (InputManager.Instance.IsLeftMouseButtonPressed() &&
            isInsideTransportArea &&
            playerCollider != null &&
            Time.time - lastTeleportTime > teleportCooldown)
        {
            if (IsCollision(playerCollider))
            {
                MovePlayer(targetPoint, transportedObject);
                lastTeleportTime = Time.time;
                StartCoroutine(ResetTransportState());
            }
        }
    }

    IEnumerator ResetTransportState()
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        if (playerCollider != null && !GetComponent<Collider>().bounds.Contains(playerCollider.bounds.center))
        {
            isInsideTransportArea = false;
            playerCollider = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsCollision(other))
        {
            isInsideTransportArea = true;
            playerCollider = other;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsCollision(other))
        {
            isInsideTransportArea = false;
            playerCollider = null;
        }
    }

    public void MovePlayer(Transform targetPoint, GameObject transportedObject)
    {
        if (transportedObject == null || targetPoint == null) return;

        Rigidbody playerRigidbody = transportedObject.GetComponent<Rigidbody>();
        CharacterController characterController = transportedObject.GetComponent<CharacterController>();

        if (characterController != null)
        {
            characterController.enabled = false;
        }

        if (playerRigidbody != null)
        {
            playerRigidbody.velocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
            playerRigidbody.isKinematic = true;
        }

        Vector3 targetPosition = targetPoint.position;

        if (Physics.CheckSphere(targetPosition, 0.5f, ~collisionLayerMask))
        {
            targetPosition += Vector3.up * 0.15f;
        }

        transportedObject.transform.position = targetPosition;
        transportedObject.transform.rotation = targetPoint.rotation;

        StartCoroutine(ReenablePhysics(playerRigidbody, characterController));
    }

    IEnumerator ReenablePhysics(Rigidbody rb, CharacterController cc)
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (cc != null)
        {
            cc.enabled = true;
        }
    }

    protected void RespawnShip()
    {
        if (transportedObject == null || targetPoint == null) return;

        GameObject obj = transportedObject;
        Rigidbody shipRigidbody = obj.GetComponent<Rigidbody>();
        CharacterController characterController = obj.GetComponent<CharacterController>();

        if (characterController != null)
        {
            characterController.enabled = false;
        }

        if (shipRigidbody != null)
        {
            shipRigidbody.velocity = Vector3.zero;
            shipRigidbody.angularVelocity = Vector3.zero;
            shipRigidbody.isKinematic = true;
        }

        obj.transform.position = targetPoint.position;
        obj.transform.rotation = targetPoint.rotation;

        StartCoroutine(ReenablePhysics(shipRigidbody, characterController));
    }

    bool IsCollision(Collider other)
    {
        return (collisionLayerMask.value & (1 << other.gameObject.layer)) > 0;
    }

    public virtual void MovePlayerManually()
    {
        if (Time.time - lastTeleportTime > teleportCooldown)
        {
            MovePlayer(targetPoint, transportedObject);
            lastTeleportTime = Time.time;
        }
    }

    public virtual void RespawnShipManually()
    {
        RespawnShip();
    }
}