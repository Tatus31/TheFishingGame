using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelinePlayer : MonoBehaviour
{
    PlayableDirector director;
    [SerializeField] GameObject UIObj;
    [SerializeField] GameObject playerObj;

    private void Awake()
    {
        director = GetComponent<PlayableDirector>();
        if (director == null)
        {
            Debug.LogError("PlayableDirector component is missing on this GameObject.");
        }

        QuestManager.OnLastQuestCompleted += PlayTimeline;

        director.played += OnTimelinePlayed;
        director.stopped += OnTimelineStopped;
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        //if(InputManager.Instance.IsLeftMouseButtonPressed())
        //    PlayTimeline();
    }

    private void OnTimelineStopped(PlayableDirector director)
    {
        UIObj.SetActive(true);
        playerObj.SetActive(true);
    }

    private void OnTimelinePlayed(PlayableDirector director)
    {
        UIObj.SetActive(false);
        playerObj.SetActive(false);
        Debug.Log("Timeline started playing.");
    }

    public void PlayTimeline()
    {
        if (director != null)
        {
            director.Play();
        }
        else
        {
            Debug.LogError("PlayableDirector is not assigned or missing.");
        }
    }
}
