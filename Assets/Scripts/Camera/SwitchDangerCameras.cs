using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchDangerCameras : MonoBehaviour
{
    [SerializeField]
    private GameObject[] dangerCameras;

    private GameObject currentActiveCamera;

    private void Start()
    {
        if (dangerCameras == null || dangerCameras.Length == 0)
        {
            Debug.LogWarning("No danger cameras assigned in the inspector.");
            return;
        }

        //foreach (GameObject cam in dangerCameras)
        //{
        //    cam.SetActive(false);
        //}
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            SwitchCamera();
        }
    }

    private void SwitchCamera()
    {
        if (currentActiveCamera == null)
        {
            currentActiveCamera = dangerCameras[0];
        }
        else
        {
            int currentIndex = System.Array.IndexOf(dangerCameras, currentActiveCamera);
            currentActiveCamera.SetActive(false);
            int nextIndex = (currentIndex + 1) % dangerCameras.Length;
            currentActiveCamera = dangerCameras[nextIndex];
        }

        currentActiveCamera.SetActive(true);
    }

}
