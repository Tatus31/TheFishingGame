using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraOverlayManager : MonoBehaviour
{
    public static CameraOverlayManager Instance;

    [SerializeField]
    private Transform cameraOverlayParentTransform;
    [SerializeField]
    private GameObject cameraOverlayObj;

    GameObject instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void TriggerEventWithDelay()
    {
        StartCoroutine(TriggerEvent());
    }

    public void EndEvent()
    {
        FadeCameraOverlayOut(cameraOverlayObj);
    }

    private IEnumerator TriggerEvent()
    {
        yield return new WaitForSeconds(1);

        if (cameraOverlayObj.transform != null && cameraOverlayParentTransform != null)
        {
            if(instance == null)
            {
                instance = Instantiate(cameraOverlayObj, cameraOverlayParentTransform);
            }
            instance.SetActive(true);

            FadeCameraOverlayIn(instance);
        }
    }
    private void FadeCameraOverlayIn(GameObject monsterEvent)
    {
        if (monsterEvent.TryGetComponent(out Image image))
        {
            Debug.Log("Fading in camera overlay.");
            image.canvasRenderer.SetAlpha(0f);
            image.CrossFadeAlpha(0.6f, 1f, false);
        }
        else
        {
            Debug.LogWarning("CameraOverlayTransform does not have an Image component or is null.");
        }
    }

    private void FadeCameraOverlayOut(GameObject monsterEvent)
    {
        if (monsterEvent.TryGetComponent(out Image image))
        {
            Debug.Log("Fading out camera overlay.");
            image.CrossFadeAlpha(0f, 1f, false);
            monsterEvent.SetActive(false);
        }
        else
        {
            Debug.LogWarning("CameraOverlayTransform does not have an Image component or is null.");
        }
    }
}
