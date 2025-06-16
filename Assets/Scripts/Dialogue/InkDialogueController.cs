using System;
using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

public class InkDialogueController : MonoBehaviour
{
    public static event Action<Story> OnCreateStory;
    public static event Action<bool> OnStartQuest;

    [SerializeField] Canvas canvas;
    [SerializeField] Button buttonPrefab;
    [SerializeField] TextMeshProUGUI textPrefab;
    [SerializeField] LayerMask interactableMask;

    [Header("Transforms")]
    [SerializeField] RectTransform dialoguePanel;
    [SerializeField] RectTransform textContainer;
    [SerializeField] RectTransform choicesContainer;

    [Header("Current Quest Data")]
    [SerializeField] TextAsset inkJSONAsset;
    [SerializeField] ItemObject requiredItem;

    bool isQuestCompleted = false;
    bool isQuestStarted = false;
    bool markerPlaced = false;

    QuestManager questManager;
    Story story;
    CameraLook cameraLook;
    Compass compass;

    bool isInteracting;

    private void Awake()
    {
        RemoveChildren();
        questManager = FindObjectOfType<QuestManager>();

        if (canvas == null)
        {
            canvas = FindObjectOfType<Canvas>();
        }

        if (cameraLook == null)
        {
            cameraLook = FindObjectOfType<CameraLook>();
        }

        if (compass == null)
        {
            compass = FindObjectOfType<Compass>();
        }
    }

    private void Start()
    {
        if (inkJSONAsset != null)
        {
            InitializeStory();
        }
    }

    private void Update()
    {
        if (InputManager.Instance.IsLeftMouseButtonPressed() && MouseWorldPosition.GetInteractable(interactableMask))
        {
            if (story != null && story.variablesState.GlobalVariableExistsWithName("startQuest"))
            {
                isInteracting = (bool)story.variablesState["startQuest"];
                isInteracting = true;

                LockCamera();
            }

            if (story != null && !story.canContinue && story.currentChoices.Count == 0 && HasRequiredItem())
            {
                StartStory();
            }

            if (HasRequiredItem())
            {
                if (story != null && story.variablesState.GlobalVariableExistsWithName("hasItem"))
                {
                    story.variablesState["hasItem"] = true;
                }
            }

            CheckAndPlaceMarker();
        }

        if (InputManager.Instance.IsRightMouseButtonPressed())
        {
            UnlockCamera();
        }

        if (isInteracting)
        {
            TurnCanvasElementsOn();
        }
        else
        {
            TurnCanvasElementsOff();
        }
    }

    private void CheckAndPlaceMarker()
    {
        if (story != null && story.variablesState.GlobalVariableExistsWithName("placeMarker"))
        {
            bool shouldPlaceMarker = (bool)story.variablesState["placeMarker"];

            if (shouldPlaceMarker && !markerPlaced && questManager != null && compass != null)
            {
                Quest currentQuest = questManager.GetCurrentQuest();
                if (currentQuest != null && currentQuest.questMarker != null)
                {
                    compass.AddMarker(currentQuest.questMarker);
                    markerPlaced = true;
                    isQuestStarted = true;
                    OnStartQuest?.Invoke(true);
                }
            }
        }
    }

    public bool HasRequiredItem()
    {
        if (requiredItem == null)
        {
            return false;
        }

        var player = FindObjectOfType<Player>();

        if (player == null || player.inventory == null)
        {
            return false;
        }

        for (int i = 0; i < player.inventory.GetSlots.Length; i++)
        {
            InventorySlot slot = player.inventory.GetSlots[i];
            if (slot.item != null && slot.item.id == requiredItem.data.id)
            {
                return true;
            }
        }

        return false;
    }

    public void TakeRequiredItem()
    {
        if (requiredItem == null) return;

        var player = FindObjectOfType<Player>();
        if (player != null && player.inventory != null)
        {
            for (int i = 0; i < player.inventory.GetSlots.Length; i++)
            {
                InventorySlot slot = player.inventory.GetSlots[i];
                if (slot.item != null && slot.item.id == requiredItem.data.id)
                {
                    player.inventory.GetSlots[i].RemoveItem();
                    var currentQuest = questManager.GetCurrentQuest();

                    if (currentQuest.rewardItem != null)
                        player.inventory.GetSlots[i].UpdateSlot(currentQuest.rewardItem.data, currentQuest.rewardAmount);

                    if (questManager != null && compass != null)
                    {
                        if (currentQuest != null && currentQuest.questMarker != null)
                        {
                            compass.DeleteMarker(currentQuest.questMarker);
                        }
                    }

                    break;
                }
            }
        }
    }

    public void UpdateQuestData(TextAsset newInkJSON, ItemObject newRequiredItem, bool addToCompass = false)
    {
        inkJSONAsset = newInkJSON;
        requiredItem = newRequiredItem;
        isQuestCompleted = false;
        isQuestStarted = false;
        markerPlaced = false;

        if (inkJSONAsset != null)
        {
            InitializeStory();
        }

        if (addToCompass && questManager != null && compass != null)
        {
            Quest currentQuest = questManager.GetCurrentQuest();
            if (currentQuest != null && currentQuest.questMarker != null)
            {
                if (story != null && story.variablesState.GlobalVariableExistsWithName("placeMarker"))
                {
                    bool shouldPlaceMarker = (bool)story.variablesState["placeMarker"];
                    if (shouldPlaceMarker)
                    {
                        compass.AddMarker(currentQuest.questMarker);
                        markerPlaced = true;
                        isQuestStarted = true;
                    }
                }
            }
        }
    }

    public bool IsQuestCompleted()
    {
        return isQuestCompleted;
    }

    void InitializeStory()
    {
        if (inkJSONAsset == null)
        {
            return;
        }

        story = new Story(inkJSONAsset.text);
        if (OnCreateStory != null)
        {
            OnCreateStory(story);
        }

        RefreshView();
    }

    void StartStory()
    {
        if (inkJSONAsset == null)
        {
            return;
        }

        story = new Story(inkJSONAsset.text);
        if (OnCreateStory != null)
        {
            OnCreateStory(story);
        }

        if (HasRequiredItem() && story.variablesState.GlobalVariableExistsWithName("hasItem"))
        {
            story.variablesState["hasItem"] = true;
        }

        RefreshView();
    }

    void RefreshView()
    {
        RemoveChildren();

        while (story.canContinue)
        {
            string text = story.Continue();
            text = text.Trim();

            CreateContentView(text);
        }

        CheckAndPlaceMarker();

        if (story.currentChoices.Count > 0)
        {
            for (int i = 0; i < story.currentChoices.Count; i++)
            {
                Choice choice = story.currentChoices[i];
                Button button = CreateChoiceView(choice.text.Trim());

                int choiceIndex = i;
                button.onClick.AddListener(delegate {
                    OnClickChoiceButton(story.currentChoices[choiceIndex]);
                });
            }
        }
        else
        {
            if (HasRequiredItem() && story.variablesState.GlobalVariableExistsWithName("completeQuest"))
            {
                bool shouldComplete = (bool)story.variablesState["completeQuest"];
                if (shouldComplete && !isQuestCompleted)
                {
                    isQuestCompleted = true;
                    TakeRequiredItem();
                }
            }

            Button choice = CreateChoiceView("Leave...");
            choice.onClick.AddListener(delegate
            {
                TurnCanvasElementsOff();
                UnlockCamera();

                if (isQuestCompleted)
                {
                    StartCoroutine(LoadNextQuestAfterDelay(0.5f));
                }
                else
                {
                    StartStory();
                }
            });
        }
    }

    void OnClickChoiceButton(Choice choice)
    {
        story.ChooseChoiceIndex(choice.index);
        RefreshView();

        if (HasRequiredItem() && story.variablesState.GlobalVariableExistsWithName("completeQuest"))
        {
            bool shouldComplete = (bool)story.variablesState["completeQuest"];
            if (shouldComplete)
            {
                isQuestCompleted = true;
                TakeRequiredItem();
            }
        }
    }

    void CreateContentView(string text)
    {
        TextMeshProUGUI storyText = Instantiate(textPrefab, textContainer);
        storyText.text = text;
    }

    Button CreateChoiceView(string text)
    {
        Button choice = Instantiate(buttonPrefab, choicesContainer);

        TextMeshProUGUI choiceText = choice.GetComponentInChildren<TextMeshProUGUI>();
        choiceText.text = text;

        HorizontalLayoutGroup layoutGroup = choice.GetComponent<HorizontalLayoutGroup>();
        if (layoutGroup != null)
        {
            layoutGroup.childForceExpandHeight = false;
        }

        return choice;
    }

    public void CompleteCurrentQuest()
    {
        if (questManager != null)
        {
            questManager.CompleteCurrentQuest();
            if (questManager.GetCurrentQuest() != null)
            {
                UpdateQuestData(questManager.GetCurrentQuest().inkJSONAsset, questManager.GetCurrentQuest().requiredItem);
                compass.DeleteMarker(questManager.GetCurrentQuest().questMarker);
            }
        }
    }

    IEnumerator LoadNextQuestAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (questManager != null)
        {
            Quest currentQuest = questManager.GetCurrentQuest();
            if (currentQuest != null && currentQuest.questMarker != null && compass != null)
            {
                compass.DeleteMarker(currentQuest.questMarker);
            }

            questManager.CompleteCurrentQuest();

            var nextQuest = questManager.GetCurrentQuest();
            if (nextQuest != null)
            {
                UpdateQuestData(nextQuest.inkJSONAsset, nextQuest.requiredItem, false);
            }
        }

        isQuestCompleted = false;
        markerPlaced = false; 
    }

    void RemoveChildren()
    {
        if (textContainer != null)
        {
            int childCount = textContainer.childCount;
            for (int i = childCount - 1; i >= 0; --i)
            {
                Destroy(textContainer.GetChild(i).gameObject);
            }
        }

        if (choicesContainer != null)
        {
            int childCount = choicesContainer.childCount;
            for (int i = childCount - 1; i >= 0; --i)
            {
                Destroy(choicesContainer.GetChild(i).gameObject);
            }
        }
    }

    void TurnCanvasElementsOn()
    {
        if (canvas != null)
        {
            canvas.gameObject.SetActive(true);
        }
        if (dialoguePanel != null)
        {
            dialoguePanel.gameObject.SetActive(true);
        }
        if (textContainer != null)
        {
            textContainer.gameObject.SetActive(true);
        }
        if (choicesContainer != null)
        {
            choicesContainer.gameObject.SetActive(true);
        }
    }

    void TurnCanvasElementsOff()
    {
        if (canvas != null)
        {
            canvas.gameObject.SetActive(false);
        }
        if (dialoguePanel != null)
        {
            dialoguePanel.gameObject.SetActive(false);
        }
        if (textContainer != null)
        {
            textContainer.gameObject.SetActive(false);
        }
        if (choicesContainer != null)
        {
            choicesContainer.gameObject.SetActive(false);
        }
    }

    void LockCamera()
    {
        if (cameraLook != null)
        {
            isInteracting = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            cameraLook.enabled = false;
        }
    }

    void UnlockCamera()
    {
        if (cameraLook != null)
        {
            isInteracting = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            cameraLook.enabled = true;
        }
    }
}