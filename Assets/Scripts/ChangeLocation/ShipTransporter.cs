using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShipTransporter : MonoBehaviour
{
    public static UnityAction<bool> OnClimb;

    [SerializeField] public Transform targetPoint;
    [SerializeField] LayerMask collisionLayerMask;
    [SerializeField] GameObject transportedObject;
    [SerializeField] float teleportCooldown = 0.5f;

    public bool isUnderDeck = false;
    bool _isInsideTransportArea = false;
    Collider _playerCollider = null;
    float _lastTeleportTime = 0f;

    private void Start()
    {
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && _isInsideTransportArea && _playerCollider &&
            Time.time - _lastTeleportTime > teleportCooldown)
        {
            if (IsCollision(_playerCollider))
            {
                MovePlayer(targetPoint, transportedObject);
                _lastTeleportTime = Time.time;
                StartCoroutine(ResetTransportState());
            }
        }
    }

    IEnumerator ResetTransportState()
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        if (_playerCollider && !GetComponent<Collider>().bounds.Contains(_playerCollider.bounds.center))
        {
            _isInsideTransportArea = false;
            _playerCollider = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsCollision(other))
        {
            _isInsideTransportArea = true;
            _playerCollider = other;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsCollision(other))
        {
            _isInsideTransportArea = false;
            _playerCollider = null;
        }
    }

    public void MovePlayer(Transform targetPoint, GameObject transportedObject)
    {

        OnClimb?.Invoke(true);

        if (!transportedObject || !targetPoint) return;

        Rigidbody playerRigidbody = transportedObject.GetComponent<Rigidbody>();
        CharacterController characterController = transportedObject.GetComponent<CharacterController>();

        if (characterController)
        {
            characterController.enabled = false;
        }

        if (playerRigidbody)
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

        if (rb)
        {
            rb.isKinematic = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (cc)
        {
            cc.enabled = true;
        }

        OnClimb?.Invoke(false);
    }

    protected void RespawnShip()
    {
        if (!transportedObject || !targetPoint) return;

        GameObject obj = transportedObject;
        Rigidbody shipRigidbody = obj.GetComponent<Rigidbody>();
        CharacterController characterController = obj.GetComponent<CharacterController>();

        if (characterController)
        {
            characterController.enabled = false;
        }

        if (shipRigidbody)
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
        if (Time.time - _lastTeleportTime > teleportCooldown)
        {
            MovePlayer(targetPoint, transportedObject);
            _lastTeleportTime = Time.time;
        }
    }

    public virtual void RespawnShipManually()
    {
        RespawnShip();
    }
}