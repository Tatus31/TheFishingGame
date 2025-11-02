using System.Collections;
using UnityEngine;

public class CameraOverlay : MonoBehaviour
{
    public static CameraOverlay Instance;

    [SerializeField] private Transform cameraOverlayParentTransform;
    [SerializeField] private GameObject cameraOverlayPrefab;

    private GameObject _instance;
    private CanvasGroup _canvasGroup;

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
    
    public void TriggerOverlayWithDelay(float startDelay)
    {
        StartCoroutine(TriggerOverlayRoutine(startDelay));
    }
    
    public void TriggerOverlayWithDelay(float startDelay, float duration)
    {
        StartCoroutine(TriggerOverlayRoutine(startDelay, duration));
    }

    public void EndOverlayEvent(float fadeDuration)
    {
        if (!_canvasGroup) return;

        UIFadeManager.Instance.Fade(_canvasGroup, 0f, fadeDuration, () =>
        {
            _instance.SetActive(false);
        });
    }

    private IEnumerator TriggerOverlayRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!_instance)
        {
            _instance = Instantiate(cameraOverlayPrefab, cameraOverlayParentTransform);
            
            if (!_instance.TryGetComponent(out _canvasGroup))
                _canvasGroup = _instance.AddComponent<CanvasGroup>();
        }

        _instance.SetActive(true);

        _canvasGroup.alpha = 0f;
        UIFadeManager.Instance.Fade(_canvasGroup, 0.6f, 1f);
    }
    
    private IEnumerator TriggerOverlayRoutine(float delay, float duration)
    {
        yield return new WaitForSeconds(delay);

        if (!_instance)
        {
            _instance = Instantiate(cameraOverlayPrefab, cameraOverlayParentTransform);
            
            if (!_instance.TryGetComponent(out _canvasGroup))
                _canvasGroup = _instance.AddComponent<CanvasGroup>();
        }

        _instance.SetActive(true);

        _canvasGroup.alpha = 0f;
        UIFadeManager.Instance.Fade(_canvasGroup, 1f, duration);
    }
}