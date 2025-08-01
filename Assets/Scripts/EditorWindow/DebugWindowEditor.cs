﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
public class DebugWindowEditor : EditorWindow
{
    TextField statValueField;
    TextField detectionValueField;
    string selectedStat;

    float detectionValue;

    [MenuItem("Window/DebugWindow")]
    public static void ShowExample()
    {
#if UNITY_EDITOR
        DebugWindowEditor wnd = GetWindow<DebugWindowEditor>();
        wnd.titleContent = new GUIContent("Debug Window");
#endif
    }

    public void CreateGUI()
    {
#if UNITY_EDITOR
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
        invincibleShipToggle.label = "God Mode: ";
        invincibleShipToggle.style.marginTop = 5;
        invincibleShipToggle.style.marginBottom = 15;
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

        VisualElement statsContainer = new VisualElement();
        statsContainer.style.flexDirection = FlexDirection.Row;
        statsContainer.style.marginTop = 15;
        statsContainer.style.marginBottom = 2;

        DropdownField statsDropdown = new DropdownField();
        statsDropdown.name = "SelectStatDropdown";
        statsDropdown.label = "Select Stat:";
        statsDropdown.style.marginTop = 2;
        statsDropdown.style.marginBottom = 2;
        statsDropdown.style.marginLeft = 1;
        statsDropdown.style.unityTextAlign = TextAnchor.MiddleCenter;

        List<string> statOptions = Enum.GetNames(typeof(Stats)).ToList();
        statsDropdown.choices = statOptions;
        statsDropdown.index = 0;
        selectedStat = statsDropdown.choices[0];

        statsDropdown.RegisterValueChangedCallback(evt =>
        {
            selectedStat = evt.newValue;
            GetStatValue(evt.newValue);
        });
        statsContainer.Add(statsDropdown);

        VisualElement newValueContainer = new VisualElement();
        newValueContainer.style.flexDirection = FlexDirection.Row;
        newValueContainer.style.marginTop = 2;
        newValueContainer.style.marginBottom = 2;

        statValueField = new TextField("New Value:");
        statValueField.style.flexGrow = 1;
        statValueField.style.width = 200;
        statValueField.style.minWidth = 150;
        statValueField.style.marginLeft = 1;
        statValueField.style.unityTextAlign = TextAnchor.MiddleCenter;
        newValueContainer.Add(statValueField);

        statsContainer.Add(newValueContainer);

        Button applyStatButton = new Button(() => SetStatValue(selectedStat, statValueField.value));
        applyStatButton.name = "ApplyStatButton";
        applyStatButton.text = "Apply";
        applyStatButton.style.marginTop = 2;
        applyStatButton.style.marginBottom = 2;
        applyStatButton.style.marginLeft = 1;
        applyStatButton.style.width = 50;
        applyStatButton.style.minWidth = 25;
        statsContainer.Add(applyStatButton);

        shipSectionContainer.Add(statsContainer);

        Button shipRespawnButton = new Button(RespawnShip);
        shipRespawnButton.name = "RespawnShip";
        shipRespawnButton.text = "Respawn Ship";
        shipRespawnButton.style.marginTop = 15;
        shipRespawnButton.style.marginBottom = 5;
        shipSectionContainer.Add(shipRespawnButton);

        Button shipTransportButton = new Button(MoveShipToPosition);
        shipTransportButton.name = "shipTransportButton";
        shipTransportButton.text = "Move Ship To Water";
        shipTransportButton.style.marginTop = 15;
        shipTransportButton.style.marginBottom = 5;
        shipSectionContainer.Add(shipTransportButton);

        VisualElement detectionContainer = new VisualElement();
        detectionContainer.style.flexDirection = FlexDirection.Row;
        detectionContainer.style.marginTop = 2;
        detectionContainer.style.marginBottom = 2;

        detectionValueField = new TextField("New Detection Value:");
        detectionValueField.style.flexGrow = 1;
        detectionValueField.style.width = 100;
        detectionValueField.style.minWidth = 75;
        detectionValueField.style.marginLeft = 1;
        detectionValueField.style.unityTextAlign = TextAnchor.MiddleCenter;
        detectionContainer.Add(detectionValueField);

        Button changeDetectionValue = new Button(SetDetectionValue);
        changeDetectionValue.name = "ChangeDetectionValue";
        changeDetectionValue.text = "Change";
        changeDetectionValue.style.marginTop = 2;
        changeDetectionValue.style.marginBottom = 2;
        changeDetectionValue.style.marginLeft = 1;
        changeDetectionValue.style.width = 100;
        changeDetectionValue.style.minWidth = 75;
        detectionContainer.Add(changeDetectionValue);

        Toggle detectionValueStatic = new Toggle();
        detectionValueStatic.name = "KeepDetectionStatic";
        detectionValueStatic.label = "Keep Detection Static: ";
        detectionValueStatic.style.marginTop = 5;
        detectionValueStatic.style.marginBottom = 15;
        detectionValueStatic.RegisterValueChangedCallback(evt => KeepDetectionValueStatic(evt.newValue));
        detectionContainer.Add(detectionValueStatic);

        VisualElement speedContainer = new VisualElement();
        speedContainer.style.flexDirection = FlexDirection.Row;
        speedContainer.style.marginTop = 10;
        speedContainer.style.marginBottom = 2;
        speedContainer.style.alignSelf = Align.Center;

        TextField moveValueField = new TextField("Speed:");
        moveValueField.style.flexGrow = 1;
        moveValueField.style.width = 40;
        moveValueField.style.minWidth = 35;
        moveValueField.style.unityTextAlign = TextAnchor.MiddleLeft;
        speedContainer.Add(moveValueField);

        Button increaseSpeedLevel = new Button(IncreaseSpeedLevel);
        increaseSpeedLevel.name = "IncreaseSpeedLevel";
        increaseSpeedLevel.text = "Increase";
        increaseSpeedLevel.style.marginTop = 2;
        increaseSpeedLevel.style.marginLeft = 1;
        increaseSpeedLevel.style.width = 100;
        increaseSpeedLevel.style.minWidth = 75;
        increaseSpeedLevel.style.unityTextAlign = TextAnchor.MiddleCenter;
        speedContainer.Add(increaseSpeedLevel);

        Button decreaseSpeedLevel = new Button(DecreaseSpeedLevel);
        decreaseSpeedLevel.name = "DecreaseSpeedLevel";
        decreaseSpeedLevel.text = "Decrease";
        decreaseSpeedLevel.style.marginTop = 2;
        decreaseSpeedLevel.style.marginBottom = 2;
        decreaseSpeedLevel.style.marginLeft = 1;
        decreaseSpeedLevel.style.width = 100;
        decreaseSpeedLevel.style.minWidth = 75;
        decreaseSpeedLevel.style.unityTextAlign = TextAnchor.MiddleCenter;
        speedContainer.Add(decreaseSpeedLevel);

        shipSectionContainer.Add(detectionContainer);
        shipSectionContainer.Add(speedContainer);

        root.Add(shipSectionContainer);
        root.Add(detectionContainer);
        root.Add(speedContainer);

        VisualElement toolSectionContainer = new VisualElement();
        toolSectionContainer.style.marginTop = 10;
        toolSectionContainer.style.marginBottom = 10;
        toolSectionContainer.style.paddingTop = 5;
        toolSectionContainer.style.paddingBottom = 5;
        toolSectionContainer.style.paddingLeft = 5;
        toolSectionContainer.style.paddingRight = 5;

        VisualElement divider2 = new VisualElement();
        divider2.style.height = 2;
        divider2.style.marginTop = 10;
        divider2.style.marginBottom = 10;
        divider2.style.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 0.7f);
        root.Add(divider2);

        VisualElement toolHeaderContainer = new VisualElement();
        toolHeaderContainer.style.flexDirection = FlexDirection.Row;
        toolHeaderContainer.style.justifyContent = Justify.Center;
        toolHeaderContainer.style.marginBottom = 10;

        Label toolSectionHeader = new Label("Tool Selection");
        toolSectionHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
        toolSectionHeader.style.fontSize = 14;

        toolHeaderContainer.Add(toolSectionHeader);
        toolSectionContainer.Add(toolHeaderContainer);

        VisualElement toolGridContainer = new VisualElement();
        toolGridContainer.style.flexDirection = FlexDirection.Row;
        toolGridContainer.style.flexWrap = Wrap.Wrap;
        toolGridContainer.style.justifyContent = Justify.Center;

        CreateToolButton(toolGridContainer, "EmptyHands", "Empty Hands");
        CreateToolButton(toolGridContainer, "Harpoon", "Harpoon");
        CreateToolButton(toolGridContainer, "FireExtinguisher", "Fire Extinguisher");
        CreateToolButton(toolGridContainer, "Wrench", "Wrench");

        root.Add(toolSectionContainer);
        toolSectionContainer.Add(toolGridContainer);

        VisualElement divider3 = new VisualElement();
        divider3.style.height = 2;
        divider3.style.marginTop = 10;
        divider3.style.marginBottom = 10;
        divider3.style.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 0.7f);
        root.Add(divider3);

        VisualElement monsterSectionContainer = new VisualElement();
        monsterSectionContainer.style.marginTop = 10;
        monsterSectionContainer.style.marginBottom = 10;
        monsterSectionContainer.style.paddingTop = 5;
        monsterSectionContainer.style.paddingBottom = 5;
        monsterSectionContainer.style.paddingLeft = 5;
        monsterSectionContainer.style.paddingRight = 5;

        VisualElement monsterHeaderContainer = new VisualElement();
        monsterHeaderContainer.style.flexDirection = FlexDirection.Row;
        monsterHeaderContainer.style.justifyContent = Justify.Center;
        monsterHeaderContainer.style.marginBottom = 10;

        Label monsterSectionHeader = new Label("Monster Controls");
        monsterSectionHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
        monsterSectionHeader.style.fontSize = 14;

        monsterHeaderContainer.Add(monsterSectionHeader);
        monsterSectionContainer.Add(monsterHeaderContainer);

        Button idleStateButton = new Button(() => SetMonsterState("Idle"));
        idleStateButton.name = "IdleStateButton";
        idleStateButton.text = "Idle State";
        idleStateButton.style.marginTop = 5;
        idleStateButton.style.marginBottom = 5;
        monsterSectionContainer.Add(idleStateButton);

        Button stalkingStateButton = new Button(() => SetMonsterState("Stalking"));
        stalkingStateButton.name = "StalkingStateButton";
        stalkingStateButton.text = "Stalking State";
        stalkingStateButton.style.marginTop = 5;
        stalkingStateButton.style.marginBottom = 5;
        monsterSectionContainer.Add(stalkingStateButton);

        Button attackingStateButton = new Button(() => SetMonsterState("Attacking"));
        attackingStateButton.name = "AttackingStateButton";
        attackingStateButton.text = "Attacking State";
        attackingStateButton.style.marginTop = 5;
        attackingStateButton.style.marginBottom = 5;
        monsterSectionContainer.Add(attackingStateButton);

        root.Add(monsterSectionContainer);
#endif
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
        ChangeWaterLevelUnderDeck waterLevelUnderDeck = ChangeWaterLevelUnderDeck.Instance;

        if (shipDamage != null)
        {
            shipDamage.IsInvincible = isInvincible;
            waterLevelUnderDeck.IsInvincible = isInvincible;

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

        if (shipRepairPoints != null)
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

    void GetStatValue(string stat)
    {
        Ship ship = Ship.Instance;
        if (ship != null)
        {
            Stats selectedStat = (Stats)Enum.Parse(typeof(Stats), stat);
            int statValue = ship.GetModifiedStatValue(selectedStat);
        }
    }

    void SetDetectionValue()
    {
        if (string.IsNullOrEmpty(detectionValueField.value))
        {
#if UNITY_EDITOR
            Debug.LogWarning("Enter a value for detection");
#endif
            return;
        }

        if (!float.TryParse(detectionValueField.value, out float newValue))
        {
#if UNITY_EDITOR
            Debug.LogWarning("Enter a valid numeric value");
#endif
            return;
        }

        DetectionManager detectionManager = DetectionManager.Instance;
        if (detectionManager != null)
        {
#if UNITY_EDITOR
            Debug.Log($"Detection value set to {newValue}");
#endif
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogWarning($"DetectionManager instance not found in the scene. (Are you in playmode?)");
#endif
        }
    }

    void KeepDetectionValueStatic(bool isStatic)
    {
        DetectionManager detectionManager = DetectionManager.Instance;
        if (detectionManager != null)
        {
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogWarning($"DetectionManager instance not found in the scene. (Are you in playmode?)");
#endif
        }
    }

    void SetStatValue(string stat, string valueText)
    {
        if (string.IsNullOrEmpty(valueText))
        {
#if UNITY_EDITOR
            Debug.LogWarning("enter a value for the stat");
#endif
            return;
        }

        if (!int.TryParse(valueText, out int newValue))
        {
#if UNITY_EDITOR
            Debug.LogWarning("enter a valid value type");
#endif
            return;
        }

        if (int.Parse(valueText) < 0)
        {
#if UNITY_EDITOR
            Debug.LogWarning("enter a positive value");
#endif
            return;
        }

        Ship ship = Ship.Instance;
        if (ship != null)
        {
            Stats selectedStat = (Stats)Enum.Parse(typeof(Stats), stat);

            ship.UpdateAttribute(selectedStat, newValue);
            GetStatValue(stat);
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogWarning($"{ship} instance not found in the scene. (Are you in playmode?)");
#endif
        }
    }

    void RespawnShip()
    {
        RespawnShip respawnShip = global::RespawnShip.Instance;

        if (respawnShip != null)
        {
            respawnShip.RespawnShipManually();
#if UNITY_EDITOR
            Debug.Log($"teleported to {respawnShip.targetPoint}.");
#endif
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogWarning($"{respawnShip} instance not found in the scene. (Are you in playmode?)");
#endif
        }
    }

    void MoveShipToPosition()
    {
        MoveShipToLocation moveShipToLocation = MoveShipToLocation.Instance;

        if (moveShipToLocation != null)
        {
            moveShipToLocation.RespawnShipManually();
#if UNITY_EDITOR
            Debug.Log($"teleported to {moveShipToLocation.targetPoint}.");
#endif
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogWarning($"{moveShipToLocation} instance not found in the scene. (Are you in playmode?)");
#endif
        }
    }

    void CreateToolButton(VisualElement container, string toolName, string displayName)
    {
        VisualElement toolButtonContainer = new VisualElement();
        toolButtonContainer.style.width = 80;
        toolButtonContainer.style.height = 100;
        toolButtonContainer.style.marginLeft = 5;
        toolButtonContainer.style.marginRight = 5;
        toolButtonContainer.style.marginTop = 5;
        toolButtonContainer.style.marginBottom = 5;
        toolButtonContainer.style.alignItems = Align.Center;

        Button toolButton = new Button(() => EquipSelectedTool(toolName));
        toolButton.name = $"{toolName}Button";

        Image toolImage = new Image();
        toolImage.style.width = 64;
        toolImage.style.height = 64;
        toolImage.style.marginBottom = 5;

        Texture2D spriteTexture = LoadToolSprite(toolName);
        if (spriteTexture != null)
        {
            toolImage.image = spriteTexture;
        }
        else
        {
            toolImage.style.backgroundColor = GetToolColor(toolName);
        }

        toolButton.Add(toolImage);
        toolButtonContainer.Add(toolButton);

        Label toolLabel = new Label(displayName);
        toolLabel.style.fontSize = 10;
        toolLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
        toolButtonContainer.Add(toolLabel);

        container.Add(toolButtonContainer);
    }

    Texture2D LoadToolSprite(string toolName)
    {
        string path = $"Assets/Textures/Sprites/WIP/{toolName}.png";
        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

        if (texture == null)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"no sprite {toolName} at {path}");
#endif
        }

        return texture;
    }

    Color GetToolColor(string toolName)
    {
        switch (toolName)
        {
            case "Harpoon": return new Color(0.8f, 0.2f, 0.2f);
            case "EmptyHands": return new Color(0.8f, 0.8f, 0.8f);
            case "FireExtinguisher": return new Color(0.2f, 0.2f, 0.8f);
            case "Wrench": return new Color(0.8f, 0.8f, 0.2f);
            default: return Color.gray;
        }
    }

    void EquipSelectedTool(string toolName)
    {
        InteractionManager interactionManager = InteractionManager.Instance;

        if (interactionManager != null)
        {
            InteractionManager.EquipedTool selectedTool = (InteractionManager.EquipedTool)Enum.Parse(
                typeof(InteractionManager.EquipedTool), toolName);

            interactionManager.EquipTool(selectedTool);
#if UNITY_EDITOR
            Debug.Log($"equipped {toolName}");
#endif
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogWarning($"{interactionManager} instance not found in the scene. (Are you in playmode?)");
#endif
        }
    }

    void SetMonsterState(string stateName)
    {
        //MonsterLargeStateMachine monsterStateMachine = MonsterLargeStateMachine.Instance;

//        if (monsterStateMachine != null)
//        {
//            switch (stateName)
//            {
//                case "Idle":
//                    monsterStateMachine.SwitchState(monsterStateMachine.IdleState);
//                    break;
//                case "Stalking":
//                    monsterStateMachine.SwitchState(monsterStateMachine.StalkingState);
//                    break;
//                case "Attacking":
//                    monsterStateMachine.SwitchState(monsterStateMachine.AttackingState);
//                    break;
//                default:
//                    Debug.LogWarning($"no state with name {stateName}");
//                    return;
//            }

//#if UNITY_EDITOR
//            Debug.Log($"monster state changed to {stateName}");
//#endif
//        }
//        else
//        {
//#if UNITY_EDITOR
//            Debug.LogWarning($"{monsterStateMachine} instance not found in the scene. (Are you in playmode?)");
//#endif
//        }
    }

    void IncreaseSpeedLevel()
    {
        ShipMovement shipMovement = FindObjectOfType<ShipMovement>();

        if (shipMovement != null)
        {
            shipMovement.IncreaseSpeedLevel();
            shipMovement.IsControllingShip = true;
#if UNITY_EDITOR
            Debug.Log($"Ship speed level increased to {shipMovement.CurrentSpeedLevel}");
#endif
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogWarning($"{shipMovement} component not found in the scene. (Are you in playmode?)");
#endif
        }
    }

    void DecreaseSpeedLevel()
    {
        ShipMovement shipMovement = FindObjectOfType<ShipMovement>();

        if (shipMovement != null)
        {
            shipMovement.DecreaseSpeedLevel();
            shipMovement.IsControllingShip = true;
#if UNITY_EDITOR
            Debug.Log($"Ship speed level decreased to {shipMovement.CurrentSpeedLevel}");
#endif
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogWarning($"{shipMovement} component not found in the scene. (Are you in playmode?)");
#endif
        }
    }
}

#endif