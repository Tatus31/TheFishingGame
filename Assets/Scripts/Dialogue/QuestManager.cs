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
    [Tooltip("The item reward ammount")]
    public int rewardAmount = 1;
    [Tooltip("Whether this quest has been completed")]
    public bool isCompleted = false;
    [Tooltip("The marker for the compass for this quest")]
    public Marker QuestMarker;
}

public class QuestManager : MonoBehaviour
{
    [SerializeField] List<Quest> quests = new List<Quest>();
    InkDialogueController dialogueController;
    Compass compass;
    int currentQuestIndex = 0;

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

        InkDialogueController.OnStartQuest += InkDialogueController_OnStartQuest;

        InitializeFirstQuest();
    }

    private void LateUpdate()
    {
        CheckQuestCompletion();
    }

    private void InkDialogueController_OnStartQuest(bool shouldAddMarker)
    {
        if (shouldAddMarker && compass != null && quests.Count > currentQuestIndex && quests[currentQuestIndex].QuestMarker != null)
        {
            Debug.Log("Adding marker to compass for quest: " + quests[currentQuestIndex].inkJSONAsset.name);
            compass.AddMarker(quests[currentQuestIndex].QuestMarker);
        }
    }

    void InitializeFirstQuest()
    {
        if (quests.Count > 0 && dialogueController != null)
        {
            dialogueController.UpdateQuestData(quests[0].inkJSONAsset, quests[0].requiredItem, false);
        }
        else
        {
            Debug.LogWarning("No quests have been added to the QuestManager or dialogueController is missing");
        }
    }

    public void CompleteCurrentQuest()
    {
        if (currentQuestIndex < quests.Count)
        {
            if (compass != null && quests[currentQuestIndex].QuestMarker != null)
            {
                compass.DeleteMarker(quests[currentQuestIndex].QuestMarker);
            }

            quests[currentQuestIndex].isCompleted = true;

            if (currentQuestIndex < quests.Count - 1)
            {
                currentQuestIndex++;
            }
        }
    }

    public void UpdateCurrentQuest()
    {
        if (currentQuestIndex < quests.Count && dialogueController != null)
        {
            Quest currentQuest = quests[currentQuestIndex];
            dialogueController.UpdateQuestData(currentQuest.inkJSONAsset, currentQuest.requiredItem, false);
        }
    }

    public Quest GetCurrentQuest()
    {
        if (currentQuestIndex < quests.Count)
        {
            return quests[currentQuestIndex];
        }
        return null;
    }

    public void CheckQuestCompletion()
    {
        if (currentQuestIndex < quests.Count && dialogueController != null)
        {
            if (dialogueController.IsQuestCompleted() && !quests[currentQuestIndex].isCompleted)
            {
                CompleteCurrentQuest();
            }
        }
    }

    public void AddQuest(Quest quest)
    {
        quests.Add(quest);
    }
}