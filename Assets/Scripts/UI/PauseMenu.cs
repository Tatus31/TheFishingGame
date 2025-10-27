using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    GameObject pauseMenuUIObj;

    private void Start()
    {
        if (pauseMenuUIObj == null)
        {
            Debug.LogError("PauseMenu: Pause Menu UI Object is not assigned in the inspector.");
        }

        pauseMenuUIObj.SetActive(false);
    }

    private void Update()
    {
        if (InputManager.Instance.GetPauseInputDown())
        {
            TogglePause();
        }
    }

    void TogglePause()
    {
        bool isPaused = !pauseMenuUIObj.activeSelf;
        pauseMenuUIObj.SetActive(isPaused);

        //if (!ElectronicRepairMiniGame.IsMiniGameActive)
        //{
        //    CameraLook.LockCamera(isPaused);
        //}
    }


    public void CollectDataButton()
    {
        DataCollectionController.Instance.StartCollectingData();
    }

    public void StopCollectingDataButton()
    {
        DataCollectionController.Instance.StopCollectingData();
    }

    public void QuitGameButton()
    {
        Application.Quit();
    }

    public void ResumeGameButton()
    {
        TogglePause();
    }
}
