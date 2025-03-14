using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class DebugWindowEditor : EditorWindow
{
    [MenuItem("Window/UI Toolkit/DebugWindowEditor")]
    public static void ShowExample()
    {
        DebugWindowEditor wnd = GetWindow<DebugWindowEditor>();
        wnd.titleContent = new GUIContent("Debug Window");
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        VisualElement teleportSectionContainer = new VisualElement();
        teleportSectionContainer.style.marginTop = 10;
        teleportSectionContainer.style.marginBottom = 10;
        teleportSectionContainer.style.paddingTop = 5;
        teleportSectionContainer.style.paddingBottom = 5;
        teleportSectionContainer.style.paddingLeft = 5;
        teleportSectionContainer.style.paddingRight = 5;

        VisualElement teleportHeaderContainer = new VisualElement();
        teleportHeaderContainer.style.flexDirection = FlexDirection.Row;
        teleportHeaderContainer.style.justifyContent = Justify.Center;
        teleportHeaderContainer.style.marginBottom = 10;

        Label teleportSectionHeader = new Label("Player Teleportation");
        teleportSectionHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
        teleportSectionHeader.style.fontSize = 14;

        teleportHeaderContainer.Add(teleportSectionHeader);
        teleportSectionContainer.Add(teleportHeaderContainer);

        Button shipButton = new Button(TeleportToShip);
        shipButton.name = "TeleportToShipButton";
        shipButton.text = "Teleport to Ship";
        shipButton.style.marginTop = 5;
        shipButton.style.marginBottom = 5;
        teleportSectionContainer.Add(shipButton);

        Button underDeckButton = new Button(TeleportToUnderDeck);
        underDeckButton.name = "TeleportToUnderDeckButton";
        underDeckButton.text = "Teleport Under Deck";
        underDeckButton.style.marginTop = 5;
        underDeckButton.style.marginBottom = 5;
        teleportSectionContainer.Add(underDeckButton);

        Button SpawnButton = new Button(TeleportToSpawn);
        SpawnButton.name = "TeleportToSpawn";
        SpawnButton.text = "Teleport To Spawn";
        SpawnButton.style.marginTop = 5;
        SpawnButton.style.marginBottom = 5;
        teleportSectionContainer.Add(SpawnButton);

        root.Add(teleportSectionContainer);

        VisualElement divider = new VisualElement();
        divider.style.height = 2;
        divider.style.marginTop = 10;
        divider.style.marginBottom = 10;
        divider.style.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 0.7f);
        root.Add(divider);

        VisualElement shipSectionContainer = new VisualElement();
        shipSectionContainer.style.marginTop = 10;
        shipSectionContainer.style.marginBottom = 10;
        shipSectionContainer.style.paddingTop = 5;
        shipSectionContainer.style.paddingBottom = 5;
        shipSectionContainer.style.paddingLeft = 5;
        shipSectionContainer.style.paddingRight = 5;

        VisualElement shipHeaderContainer = new VisualElement();
        shipHeaderContainer.style.flexDirection = FlexDirection.Row;
        shipHeaderContainer.style.justifyContent = Justify.Center;
        shipHeaderContainer.style.marginBottom = 10;
        
        Label shipSectionHeader = new Label("Ship");
        shipSectionHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
        shipSectionHeader.style.fontSize = 14;
        
        shipHeaderContainer.Add(shipSectionHeader);
        shipSectionContainer.Add(shipHeaderContainer);

        Toggle invincibleShipToggle = new Toggle();
        invincibleShipToggle.name = "TurnShipInvincible";
        invincibleShipToggle.label = "Turn Invincible";
        invincibleShipToggle.style.marginTop = 5;
        invincibleShipToggle.style.marginBottom = 5;
        invincibleShipToggle.RegisterValueChangedCallback(evt => ToggleShipInvincibility(evt.newValue));

        shipSectionContainer.Add(invincibleShipToggle);

        VisualElement dropdownContainer = new VisualElement();
        dropdownContainer.style.flexDirection = FlexDirection.Row;
        dropdownContainer.style.marginTop = 5;
        dropdownContainer.style.marginBottom = 5;

        DropdownField holesDropdown = new DropdownField();
        holesDropdown.name = "NumberOfHolesDropdown";
        holesDropdown.label = "Number of Holes:";
        holesDropdown.choices = new List<string> { "1" };
        holesDropdown.index = 0;
        holesDropdown.style.flexGrow = 1;
        dropdownContainer.Add(holesDropdown);

        Button updateButton = new Button(() => UpdateHolesDropdown(holesDropdown));
        updateButton.name = "UpdateHolesButton";
        updateButton.text = "Update";
        updateButton.style.width = 45;
        updateButton.style.marginLeft = 10;
        updateButton.style.alignSelf = Align.FlexEnd;
        dropdownContainer.Add(updateButton);

        shipSectionContainer.Add(dropdownContainer);

        Button openHolesButton = new Button(() => OpenShipHoles(int.Parse(holesDropdown.value)));
        openHolesButton.name = "OpenHolesButton";
        openHolesButton.text = "Open Holes";
        openHolesButton.style.marginTop = 5;
        openHolesButton.style.marginBottom = 10;
        shipSectionContainer.Add(openHolesButton);

        root.Add(shipSectionContainer);
    }

    void TeleportToShip()
    {
        LeaveUnderDeck leaveUnderDeck = LeaveUnderDeck.Instance;

        if (leaveUnderDeck != null)
        {
            leaveUnderDeck.MovePlayerManually();
#if UNITY_EDITOR
            Debug.Log($"Player teleported to {leaveUnderDeck.targetPoint}.");
#endif
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogWarning($"{leaveUnderDeck} instance not found in the scene. (Are you in playmode?)");
#endif
        }
    }

    void TeleportToUnderDeck()
    {
        EnterUnderDeck enterUnderDeck = EnterUnderDeck.Instance;

        if (enterUnderDeck != null)
        {
            enterUnderDeck.MovePlayerManually();
#if UNITY_EDITOR
            Debug.Log($"Player teleported to {enterUnderDeck.targetPoint}.");
#endif
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogWarning($"{enterUnderDeck} instance not found in the scene. (Are you in playmode?)");
#endif
        }
    }

    void TeleportToSpawn()
    {
        MoveToSpawn moveToSpawn = MoveToSpawn.Instance;

        if (moveToSpawn != null)
        {
            moveToSpawn.MovePlayerManually();
#if UNITY_EDITOR
            Debug.Log($"Player teleported to {moveToSpawn.targetPoint}.");
#endif
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogWarning($"{moveToSpawn} instance not found in the scene. (Are you in playmode?)");
#endif
        }
    }

    void ToggleShipInvincibility(bool isInvincible)
    {
        ShipDamage shipDamage = ShipDamage.Instance;

        if (shipDamage != null)
        {
            shipDamage.IsInvincible = isInvincible;
#if UNITY_EDITOR
            Debug.Log($"Ship is invincible");
#endif
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogWarning($"{shipDamage} instance not found in the scene. (Are you in playmode?)");
#endif
        }
    }

    void OpenShipHoles(int numberOfHoles)
    {
        ShipRepairPoints shipRepairPoints = ShipRepairPoints.Instance;

        if (shipRepairPoints != null)
        {
            for (int i = 0; i < numberOfHoles; i++)
            {
                shipRepairPoints.SpawnHole();
            }
#if UNITY_EDITOR
            Debug.Log($"Opened {numberOfHoles} holes in the ship.");
#endif
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogWarning($"{shipRepairPoints} instance not found in the scene. (Are you in playmode?)");
#endif
        }
    }

    void UpdateHolesDropdown(DropdownField dropdown)
    {
        ShipRepairPoints shipRepairPoints = ShipRepairPoints.Instance;

        if(shipRepairPoints != null)
        {
            List<string> choices = new List<string>();
            for (int i = 1; i <= shipRepairPoints.RepairPoints.Count; i++)
            {
                choices.Add(i.ToString());
            }
            if (choices.Count == 0)
            {
                choices.Add("0");
            }

            dropdown.choices = choices;
            dropdown.index = 0;
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogWarning($"{shipRepairPoints} instance not found in the scene. (Are you in playmode?)");
#endif
        }
    }
}