using System;
using System.Collections;
using UnityEngine;

public class ShipCorruptionProtection : MonoBehaviour
{
    [Range(0f, 20f)]
    [SerializeField] public float radius;
    [SerializeField] private float corruptionTime;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform respawnTransform;

    private bool _isInside = true;
    private Coroutine _corruptRoutine;

    private ShipTransporter _shipTransporter;

    private void Start()
    {
        _shipTransporter = FindObjectOfType<ShipTransporter>();

        if (!_shipTransporter)
        {
            Debug.LogWarning("ShipTransporter not found");
        }
    }

    void Update()
    {
        float distance = Vector3.Distance(playerTransform.position, transform.position);
        bool insideNow = distance <= radius;
        
        if (IsUnderDeck.isUnderDeck || TraderArea.IsInsideTraderArea)
        {
            if (!_isInside)
            {
                _isInside = true;
                StopCorruption();
            }

        }
        
        if (!insideNow && _isInside)
        {
            _isInside = false;
            StartCorruption();
        }

        if (insideNow && !_isInside)
        {
            _isInside = true;
            StopCorruption();
        }
    }

    private void StartCorruption()
    {
        if(IsUnderDeck.isUnderDeck || TraderArea.IsInsideTraderArea)
            return;
        
        CameraOverlay.Instance.TriggerOverlayWithDelay(0f, corruptionTime);
        _corruptRoutine = StartCoroutine(CorruptionTimer());
    }


    private void StopCorruption()
    {
        if (_corruptRoutine != null)
        {
            StopCoroutine(_corruptRoutine);
            _corruptRoutine = null;
        }

        StartCoroutine(FadeOutAfterFrame());
    }

    private IEnumerator FadeOutAfterFrame()
    {
        yield return null;
        CameraOverlay.Instance.EndOverlayEvent( 1f);
    }

    private IEnumerator CorruptionTimer()
    {
        float timer = 0f;

        while (timer < corruptionTime)
        {
            if (_isInside) 
                yield break;
            
            timer += Time.deltaTime;
            
            yield return null;
        }
        
#if UNITY_EDITOR
        Debug.Log("Player corrupted and returned to ship.");
#endif
        CameraOverlay.Instance.EndOverlayEvent(1f);
        yield return new WaitForSeconds(1f);
        _shipTransporter.MovePlayer(respawnTransform, playerTransform.gameObject);
    }

    void OnDrawGizmosSelected()
    {
        if (!playerTransform) 
            return;
        
        Gizmos.color = Vector3.Distance(playerTransform.position, transform.position) <= radius ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
