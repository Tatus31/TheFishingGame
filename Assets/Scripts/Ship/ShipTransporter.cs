using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipTransporter : MonoBehaviour
{
    [SerializeField] Transform targetPoint;
    [SerializeField] LayerMask collisionLayerMask;
    [SerializeField] GameObject player;

    private void OnTriggerEnter(Collider other)
    {
        if (IsCollision(other))
            MovePlayer();
    }

    protected void MovePlayer()
    {
        Rigidbody playerRigidbody = player.GetComponent<Rigidbody>();
        if (playerRigidbody != null)
        {
            playerRigidbody.isKinematic = true;
        }

        player.transform.position = targetPoint.position;

        if (playerRigidbody != null)
        {
            playerRigidbody.isKinematic = false;
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
}