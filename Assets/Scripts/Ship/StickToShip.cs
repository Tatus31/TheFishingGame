using Game;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickToShip : MonoBehaviour
{
    public static StickToShip Instance;

    [Header("References")]
    [SerializeField] Rigidbody shipRb;

    [Header("Mask")]
    [SerializeField] LayerMask shipControlsLayerMask;

    bool isOnShip;
    public bool IsControllingShip;

    Vector3 localPositionOffset;
    Quaternion localRotationOffset;

    ShipMovement shipMovement;
    Rigidbody playerRb;
    CharacterController characterController;
    Transform shipTransform;
    Vector3 previousShipPosition;
    Quaternion previousShipRotation;
    Vector3 moveDirection;

    public bool IsOnShip { get { return isOnShip; } }

    private void Awake()
    {
        if (Instance != null)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"there exists a {Instance.name} in the scene already");
#endif
        }

        Instance = this;

        playerRb = GetComponent<Rigidbody>();
        characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        if (shipRb != null)
        {
            shipTransform = shipRb.transform;
            previousShipPosition = shipTransform.position;
            previousShipRotation = shipTransform.rotation;
        }
    }

    private void FixedUpdate()
    {
        if (!shipRb || !isOnShip)
            return;

        if (IsControllingShip)
        {
            transform.localPosition = localPositionOffset;
            transform.localRotation = localRotationOffset;
        }
        else
        {
            Vector3 shipTransformPosition = shipTransform.position - previousShipPosition;

            if (characterController && characterController.enabled)
            {
                characterController.Move(shipTransformPosition);
            }
            else if (playerRb)
            {
                playerRb.MovePosition(playerRb.position + shipTransformPosition);

                Quaternion rotationDelta = shipTransform.rotation * Quaternion.Inverse(previousShipRotation);

                Vector3 playerRelativePosition = transform.position - shipTransform.position;
                Vector3 rotatedPosition = shipTransform.position + rotationDelta * playerRelativePosition;
                Vector3 rotationMovement = rotatedPosition - transform.position;

                playerRb.MovePosition(playerRb.position + rotationMovement);

                //playerRb.AddForce(-transform.up * additionalDownForce, ForceMode.Force);
            }
            else
            {
                transform.position += shipTransformPosition;
            }
        }

        previousShipPosition = shipTransform.position;
        previousShipRotation = shipTransform.rotation;
    }

    private void LateUpdate()
    {
        
    }

    private void Update()
    {
        if (InputManager.Instance.GetInteractInputDown() && MouseWorldPosition.GetInteractable(shipControlsLayerMask) && isOnShip)
        {
            ToggleShipControl();
        }
    }

    IEnumerator DataCollectionForControllingShipTime()
    {
        float timeControlingShip = 0f;

        while (IsControllingShip)
        {
            timeControlingShip += Time.deltaTime;
            yield return null;
        }

        timeControlingShip = (float)Math.Round(timeControlingShip, 2);

        var analyticsData = new Dictionary<string, object>
        {
            { "timeControllingShip", timeControlingShip }
        };

        AnalyticsEvents.SendAnalyticsEvent("OnPlayerControlShipTime", analyticsData);
    }

    private void ToggleShipControl()
    {
        if (IsControllingShip)
        {
            IsControllingShip = false;

            PlayerMovement.Instance.IsControllable = true;

            if (shipMovement)
            {
                shipMovement.IsControllingShip = false;
            }

            transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
            PlayerMovement.Instance.orientation.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
        }
        else
        {
            localPositionOffset = transform.localPosition;
            localRotationOffset = transform.localRotation;

            IsControllingShip = true;

            PlayerMovement.Instance.IsControllable = false;

            if (shipMovement)
            {
                shipMovement.IsControllingShip = true;
            }

            StartCoroutine(DataCollectionForControllingShipTime());
        }

        Events.onEnteredInteraction.CanShowPanel = IsControllingShip;
        EventManager.Broadcast(Events.onEnteredInteraction);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ship") && !isOnShip && !ChangeWaterLevelUnderDeck.Instance.ShipSank)
        {
            isOnShip = true;

            transform.SetParent(shipRb.transform);
            PlayerMovement.Instance.orientation.SetParent(null, true);
            shipMovement = collision.gameObject.GetComponent<ShipMovement>();
            shipTransform = shipRb.transform;

            previousShipPosition = shipTransform.position;
            previousShipRotation = shipTransform.rotation;

            if (playerRb != null && playerRb.isKinematic == false)
            {
                playerRb.velocity = (shipTransform.position - previousShipPosition) / Time.fixedDeltaTime;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Ship") && isOnShip)
        {
            isOnShip = false;
            IsControllingShip = false;
            transform.SetParent(null);

            PlayerMovement.Instance.orientation.SetParent(transform, true);
            PlayerMovement.Instance.IsControllable = true;

            if (playerRb != null && playerRb.isKinematic == false)
            {
                Vector3 exitVelocity = (shipTransform.position - previousShipPosition) / Time.fixedDeltaTime;
                playerRb.velocity = new Vector3(playerRb.velocity.x + exitVelocity.x * 0.8f, playerRb.velocity.y, playerRb.velocity.z + exitVelocity.z * 0.8f);
            }
        }
    }
}