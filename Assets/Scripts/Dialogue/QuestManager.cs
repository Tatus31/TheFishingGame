using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using System;

[System.Serializable]
public class Quest
{
    [Tooltip("The JSON file containing the Ink dialogue for this quest")]
    public TextAsset inkJSONAsset;
    [Tooltip("The item required to complete this quest")]
    public ItemObject requiredItem;
    [Tooltip("The item reward after completing a quest")]
    public ItemObject rewardItem;
    [Tooltip("The item reward amount")]
    public int rewardAmount = 1;
    [Tooltip("Whether this quest has been completed")]
    public bool isCompleted = false;
    [Tooltip("The marker for the compass for this quest")]
    public Marker questMarker;
}

public class QuestManager : MonoBehaviour
{
    [SerializeField] List<Quest> quests = new List<Quest>();
    InkDialogueController dialogueController;
    [SerializeField] List<Marker> staticMarkers = new List<Marker>();
    Compass compass;
    int currentQuestIndex = 0;

    private int lastCheckedQuestIndex = -1;

    private void Start()
    {
        if (dialogueController == null)
        {
            dialogueController = FindObjectOfType<InkDialogueController>();
        }
        if (compass == null)
        {
            compass = FindObjectOfType<Compass>();
        }

        if(staticMarkers.Count > 0)
        {
            foreach (var marker in staticMarkers)
            {
                compass.AddMarker(marker);
            }
        }

        InkDialogueController.OnStartQuest += InkDialogueController_OnStartQuest;

        InitializeFirstQuest();
    }

    private void OnDestroy()
    {
        InkDialogueController.OnStartQuest -= InkDialogueController_OnStartQuest;
    }

    private void LateUpdate()
    {
        CheckQuestCompletion();
    }

    private void InkDialogueController_OnStartQuest(bool shouldAddMarker)
    {
        if (shouldAddMarker && compass != null && IsValidQuestIndex(currentQuestIndex) && quests[currentQuestIndex].questMarker != null)
        {
            compass.AddMarker(quests[currentQuestIndex].questMarker);
        }
    }

    void InitializeFirstQuest()
    {
        currentQuestIndex = FindNextAvailableQuest(0);

        if (IsValidQuestIndex(currentQuestIndex) && dialogueController != null)
        {
            dialogueController.UpdateQuestData(quests[currentQuestIndex].inkJSONAsset, quests[currentQuestIndex].requiredItem, false);
        }
        else
        {
            Debug.LogWarning("No available quests to initialize or dialogueController is missing");
        }
    }

    private int FindNextAvailableQuest(int startIndex)
    {
        for (int i = startIndex; i < quests.Count; i++)
        {
            if (!quests[i].isCompleted)
            {
                return i;
            }
        }
        return -1; 
    }

    private bool IsValidQuestIndex(int index)
    {
        return index >= 0 && index < quests.Count;
    }

    public void CompleteCurrentQuest()
    {
        if (!IsValidQuestIndex(currentQuestIndex))
        {
            Debug.LogWarning("Cannot complete quest: invalid quest index " + currentQuestIndex);
            return;
        }

        if (compass != null && quests[currentQuestIndex].questMarker != null)
        {
            compass.DeleteMarker(quests[currentQuestIndex].questMarker);
        }
        quests[currentQuestIndex].isCompleted = true;

        int nextQuestIndex = FindNextAvailableQuest(currentQuestIndex + 1);

        if (nextQuestIndex != -1)
        {
            currentQuestIndex = nextQuestIndex;
            UpdateCurrentQuest();
        }
        else
        {
            Debug.Log("All quests completed!");
        }
    }

    public void UpdateCurrentQuest()
    {
        if (IsValidQuestIndex(currentQuestIndex) && dialogueController != null)
        {
            Quest currentQuest = quests[currentQuestIndex];
            dialogueController.UpdateQuestData(currentQuest.inkJSONAsset, currentQuest.requiredItem, false);
        }
        else
        {
            Debug.LogWarning("Cannot update quest: invalid index or missing dialogueController");
        }
    }

    public Quest GetCurrentQuest()
    {
        if (IsValidQuestIndex(currentQuestIndex))
        {
            return quests[currentQuestIndex];
        }
        return null;
    }

    public void CheckQuestCompletion()
    {
        if (IsValidQuestIndex(currentQuestIndex) && dialogueController != null &&
            currentQuestIndex != lastCheckedQuestIndex)
        {
            if (dialogueController.IsQuestCompleted() && !quests[currentQuestIndex].isCompleted)
            {
                lastCheckedQuestIndex = currentQuestIndex;
                CompleteCurrentQuest();
            }
        }
    }

    public void AddQuest(Quest quest)
    {
        if (quest != null && quest.inkJSONAsset != null)
        {
            quests.Add(quest);

            if (quests.Count == 1 || !IsValidQuestIndex(currentQuestIndex) || quests[currentQuestIndex].isCompleted)
            {
                InitializeFirstQuest();
            }
        }
        else
        {
            Debug.LogWarning("Cannot add null quest or quest without inkJSONAsset");
        }
    }

    public void PrintQuestStatus()
    {
        Debug.Log("=== Quest Status ===");
        Debug.Log("Current Quest Index: " + currentQuestIndex);
        for (int i = 0; i < quests.Count; i++)
        {
            string status = quests[i].isCompleted ? "COMPLETED" : "ACTIVE";
            string current = (i == currentQuestIndex) ? " <- CURRENT" : "";
            Debug.Log($"Quest {i}: {quests[i].inkJSONAsset.name} - {status}{current}");
        }
    }

    public void ResetAllQuests()
    {
        foreach (Quest quest in quests)
        {
            quest.isCompleted = false;
        }
        currentQuestIndex = 0;
        lastCheckedQuestIndex = -1;
        InitializeFirstQuest();
    }
}