﻿using System;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// This is a super bare bones example of how to play and display a ink story in Unity.
public class BasicInkExample : MonoBehaviour
{
    public static event Action<Story> OnCreateStory;
    bool isInteracting;

    [SerializeField] Player player;
    [SerializeField] LayerMask interactableMask;
    [SerializeField] ItemObject requiredItem;
    CameraLook cameraLook;

    void Awake()
    {
        // Remove the default message
        RemoveChildren();
        StartStory();

        cameraLook = FindObjectOfType<CameraLook>();
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

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                cameraLook.enabled = false;
            }
            if (player != null && HasRequiredItem())
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
            isInteracting = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            cameraLook.enabled = true;
        }

        if (isInteracting)
        {
            canvas.gameObject.SetActive(true);
        }
        else
        {
            canvas.gameObject.SetActive(false);
        }
    }

    bool HasRequiredItem()
    {
        if (player == null || player.inventory == null)
            return false;

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

    void TakeRequiredItem()
    {
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

    // Creates a new Story object with the compiled story which we can then play!
    void StartStory()
    {
        story = new Story(inkJSONAsset.text);
        if (OnCreateStory != null)
        {
            OnCreateStory(story);
            OnCreateStory?.Invoke(story);
        }

        RefreshView();
    }

    // This is the main function called every time the story changes. It does a few things:
    // Destroys all the old content and choices.
    // Continues over all the lines of text, then displays all the choices. If there are no choices, the story is finished!
    void RefreshView()
    {
        // Remove all the UI on screen
        RemoveChildren();

        // Read all the content until we can't continue any more
        while (story.canContinue)
        {
            // Continue gets the next line of the story
            string text = story.Continue();
            // This removes any white space from the text.
            text = text.Trim();
            // Display the text on screen!
            CreateContentView(text);
        }
        // Display all the choices, if there are any!
        if (story.currentChoices.Count > 0)
        {
            for (int i = 0; i < story.currentChoices.Count; i++)
            {
                Choice choice = story.currentChoices[i];
                Button button = CreateChoiceView(choice.text.Trim());
                // Tell the button what to do when we press it
                button.onClick.AddListener(delegate {
                    OnClickChoiceButton(choice);
                });
            }
        }
        // If we've read all the content and there's no choices, the story is finished!
        else
        {
            Button choice = CreateChoiceView("Leave...");
            choice.onClick.AddListener(delegate
            {
                canvas.gameObject.SetActive(false);
                isInteracting = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                cameraLook.enabled = true;
                StartStory();
            });
        }
    }
    // When we click the choice button, tell the story to choose that choice!
    void OnClickChoiceButton(Choice choice)
    {
        story.ChooseChoiceIndex(choice.index);
        RefreshView();
    }
    // Creates a textbox showing the the line of text
    void CreateContentView(string text)
    {
        TextMeshProUGUI storyText = Instantiate(textPrefab) as TextMeshProUGUI;
        storyText.text = text;
        storyText.transform.SetParent(canvas.transform, false);
    }
    // Creates a button showing the choice text
    Button CreateChoiceView(string text)
    {
        // Creates the button from a prefab
        Button choice = Instantiate(buttonPrefab) as Button;
        choice.transform.SetParent(canvas.transform, false);
        // Gets the text from the button prefab
        TextMeshProUGUI choiceText = choice.GetComponentInChildren<TextMeshProUGUI>();
        choiceText.text = text;
        // Make the button expand to fit the text
        HorizontalLayoutGroup layoutGroup = choice.GetComponent<HorizontalLayoutGroup>();
        layoutGroup.childForceExpandHeight = false;
        return choice;
    }
    // Destroys all the children of this gameobject (all the UI)
    void RemoveChildren()
    {
        int childCount = canvas.transform.childCount;
        for (int i = childCount - 1; i >= 0; --i)
        {
            Destroy(canvas.transform.GetChild(i).gameObject);
        }
    }
    [SerializeField]
    private TextAsset inkJSONAsset = null;
    public Story story;
    [SerializeField]
    private Canvas canvas = null;
    // UI Prefabs
    [SerializeField]
    private TextMeshProUGUI textPrefab = null;
    [SerializeField]
    private Button buttonPrefab = null;
}