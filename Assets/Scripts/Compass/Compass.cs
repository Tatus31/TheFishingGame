using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour
{
    [SerializeField] GameObject markerPrefab;
    [SerializeField] RawImage compassImage;
    [SerializeField] Transform playerOrientationTransform;
    [SerializeField] float compassViewAngle = 180f;

    float halfCompassWidth;
    public List<Marker> markers = new List<Marker>();

    private void Start()
    {
        halfCompassWidth = compassImage.rectTransform.rect.width / 2f;
    }

    private void Update()
    {
        Transform orientationTransform = GetOrientationTransform();

        compassImage.uvRect = new Rect(orientationTransform.localEulerAngles.y / 360f, 0f, 1f, 1f);

        foreach (var marker in markers)
        {
            Vector2 position = GetPositionOnCompass(marker, orientationTransform);
            marker.image.gameObject.SetActive(Mathf.Abs(position.x) <= halfCompassWidth);
            marker.image.rectTransform.anchoredPosition = position;
        }
    }

    private Transform GetOrientationTransform()
    {
        if (StickToShip.Instance != null && StickToShip.Instance.IsOnShip)
        {
            ShipMovement shipMovement = FindObjectOfType<ShipMovement>();
            if (shipMovement != null && shipMovement.IsControllingShip)
            {
                return shipMovement.transform;
            }
        }

        return playerOrientationTransform;
    }

    public void AddMarker(Marker marker)
    {
        if (markers.Contains(marker))
        {
            return;
        }
        GameObject newMarker = Instantiate(markerPrefab, compassImage.transform);
        marker.image = newMarker.GetComponent<Image>();
        marker.image.sprite = marker.icon;
        markers.Add(marker);
    }

    public void DeleteMarker(Marker marker)
    {
        if (marker == null)
            return;
        if (marker.image != null && marker.image.gameObject != null)
            Destroy(marker.image.gameObject);
        markers.Remove(marker);
    }

    Vector2 GetPositionOnCompass(Marker marker, Transform orientationTransform)
    {
        Vector3 currentPos = orientationTransform.position;
        Vector3 markerPos = new Vector3(marker.position.x, currentPos.y, marker.position.y);
        Vector3 directionToMarker = markerPos - currentPos;
        float angle = Vector3.SignedAngle(orientationTransform.forward, directionToMarker, Vector3.up);
        float pixelsPerDegree = compassImage.rectTransform.rect.width / compassViewAngle;
        float positionX = angle * pixelsPerDegree;
        positionX = Mathf.Clamp(positionX, -halfCompassWidth, halfCompassWidth);
        return new Vector2(positionX, 0f);
    }
}