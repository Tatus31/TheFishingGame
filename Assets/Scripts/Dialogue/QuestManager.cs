using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

[System.Serializable]
public class Quest
{
    [Tooltip("The JSON file containing the Ink dialogue for this quest")]
    public TextAsset inkJSONAsset;

    [Tooltip("The item required to complete this quest")]
    public ItemObject requiredItem;

    [Tooltip("Whether this quest has been completed")]
    public bool isCompleted = false;
}

public class QuestManager : MonoBehaviour
{
    [SerializeField] List<Quest> quests = new List<Quest>();
    [SerializeField] InkDialogueController dialogueController;

    int currentQuestIndex = 0;

    private void Start()
    {
        if (dialogueController == null)
        {
            dialogueController = FindObjectOfType<InkDialogueController>();
        }

        InitializeFirstQuest();
    }

    void InitializeFirstQuest()
    {
        if (quests.Count > 0 && dialogueController != null)
        {
            dialogueController.UpdateQuestData(quests[0].inkJSONAsset, quests[0].requiredItem);
        }
        else
        {
            Debug.LogWarning("No quests have been added to the QuestManager or dialogueController is missing!");
        }
    }

    public void CompleteCurrentQuest()
    {
        if (currentQuestIndex < quests.Count)
        {
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
            dialogueController.UpdateQuestData(currentQuest.inkJSONAsset, currentQuest.requiredItem);
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