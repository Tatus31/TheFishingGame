using System;
using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InkDialogueController : MonoBehaviour
{
    public static event Action<Story> OnCreateStory;

    [SerializeField][Tooltip("the JSON file of the story that is going to play")] TextAsset inkJSONAsset;
    [SerializeField] Canvas canvas;
    [SerializeField] Button buttonPrefab;
    [SerializeField] TextMeshProUGUI textPrefab;
    [SerializeField] ItemObject requiredItem;
    [SerializeField] LayerMask interactableMask;

    [Header("Transforms")]
    [SerializeField] RectTransform dialoguePanel;
    [SerializeField] RectTransform textContainer;
    [SerializeField] RectTransform choicesContainer;

    Story story;
    CameraLook cameraLook;

    bool isInteracting;

    private void Awake()
    {
        RemoveChildren();
        StartStory();

        if (canvas == null)
        {
            canvas = FindObjectOfType<Canvas>();
        }

        if (cameraLook == null)
        {
            cameraLook = FindObjectOfType<CameraLook>();
        }
    }   

    private void Update()
    {
        if (InputManager.Instance.IsLeftMouseButtonPressed() && MouseWorldPosition.GetInteractable(interactableMask))
        {
            Debug.Log("Clicked on interactable object");

            if (story.variablesState.GlobalVariableExistsWithName("startJump"))
            {
                isInteracting = (bool)story.variablesState["startJump"];
                isInteracting = true;

                LockCamera();
            }
            if (HasRequiredItem())
            {
                Debug.Log("Has required item");

                if (story.variablesState.GlobalVariableExistsWithName("hasItem"))
                {
                    story.variablesState["hasItem"] = true;
                    TakeRequiredItem();
                }
            }
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

    public bool HasRequiredItem()
    {
        var player = FindObjectOfType<Player>();

        if (player == null || player.inventory == null)
        {
            return false;
        }

        for (int i = 0; i < player.inventory.GetSlots.Length; i++)
        {
            InventorySlot slot = player.inventory.GetSlots[i];
            if (slot.item.id == requiredItem.data.id)
            {
                return true;
            }
        }

        return false;
    }

    public void TakeRequiredItem()
    {
        var player = FindObjectOfType<Player>();

        if (player != null && player.inventory != null)
        {
            for (int i = 0; i < player.inventory.GetSlots.Length; i++)
            {
                InventorySlot slot = player.inventory.GetSlots[i];
                if (slot.item.id == requiredItem.data.id)
                {
                    player.inventory.GetSlots[i].RemoveItem();
                    break;
                }
            }
        }
    }

    void StartStory()
    {
        story = new Story(inkJSONAsset.text);
        if (OnCreateStory != null)
        {
            OnCreateStory(story);
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
            Button choice = CreateChoiceView("Leave...");
            choice.onClick.AddListener(delegate
            {
                Debug.Log("Clicked leave button");
                TurnCanvasElementsOff();
                UnlockCamera();
                StartStory();
            });
        }
    }

    void OnClickChoiceButton(Choice choice)
    {
        story.ChooseChoiceIndex(choice.index);
        RefreshView();
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
        if(canvas != null)
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
