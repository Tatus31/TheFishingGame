using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipTransporter : MonoBehaviour
{
    [SerializeField] public Transform targetPoint;
    [SerializeField] LayerMask collisionLayerMask;
    [SerializeField] GameObject transportedObject;

    private void OnTriggerEnter(Collider other)
    {
        if (IsCollision(other))
            MovePlayer();
    }

    protected void MovePlayer()
    {
        Rigidbody playerRigidbody = transportedObject.GetComponent<Rigidbody>();
        if (playerRigidbody != null)
        {
            playerRigidbody.isKinematic = true;
        }

        transportedObject.transform.position = targetPoint.position;
        if (playerRigidbody != null)
        {
            playerRigidbody.isKinematic = false;
        }
    }

    protected void RespawnShip()
    {
        GameObject obj = transportedObject;
        Rigidbody shipRigidbody = obj.GetComponent<Rigidbody>();

        if (shipRigidbody != null)
        {
            shipRigidbody.isKinematic = true;
        }

        obj.transform.position = targetPoint.position;
        obj.transform.rotation = targetPoint.rotation;

        if (shipRigidbody != null)
        {
            shipRigidbody.isKinematic = false;
            shipRigidbody.velocity = Vector3.zero;
            shipRigidbody.angularVelocity = Vector3.zero;
        }
    }

    bool IsCollision(Collider other)
    {
        return (collisionLayerMask.value & (1 << other.gameObject.layer)) > 0;
    }

    public virtual void MovePlayerManually()
    {
        MovePlayer();
    }
    public virtual void RespawnShipManually()
    {
        RespawnShip();
    }
}