using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Timeline.Actions.MenuPriority;

public class Player : MonoBehaviour
{
    public static event EventHandler<bool> OnMonsterHuntingPlayer;

    public InventoryObject inventory;
    public InventoryObject equipment;

    public PlayerAttributes[] playerAttributes;

    [SerializeField] Transform monsterHeadTransform;
    [SerializeField] float minDistanceToPlayer = 10f;

    bool isSwimming = false;
    bool isHuntingPlayer = false;

    Compass compass;

    private void Start()
    {
        PlayerMovement.Instance.OnPlayerSwimmingChange += PlayerMovement_OnPlayerSwimmingChange;

        if (compass == null)
        {
            compass = FindObjectOfType<Compass>();
        }

        for (int i = 0; i < playerAttributes.Length; i++)
        {
            playerAttributes[i].SetParent(this);
        }
        for (int i = 0; i < equipment.GetSlots.Length; i++)
        {
            equipment.GetSlots[i].OnBeforeUpdate += OnRemoveItem;
            equipment.GetSlots[i].OnAfterUpdate += OnAddItem;
        }
    }

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, monsterHeadTransform.position);
        if (isSwimming && distance <= minDistanceToPlayer)
        {
            Debug.Log($"Player is swimming and within {minDistanceToPlayer} units of the monster head.");

            isHuntingPlayer = true;
            OnMonsterHuntingPlayer?.Invoke(this, isHuntingPlayer);
            MonsterStateMachine.Instance.SwitchState(MonsterStateMachine.Instance.AttackingPlayerState);
        }
        else
        {
            isHuntingPlayer = false;
            OnMonsterHuntingPlayer?.Invoke(this, isHuntingPlayer);
            MonsterStateMachine.Instance.SwitchState(MonsterStateMachine.Instance.IdleState);
        }
    }

    private void PlayerMovement_OnPlayerSwimmingChange(object sender, bool e)
    {
        isSwimming = e;
    }

    public void OnRemoveItem(InventorySlot slot)
    {
        if (slot.ItemObject == null)
            return;

        switch (slot.parent.inventory.type)
        {
            case InterfaceType.Inventory:
                break;
            case InterfaceType.Equipment:
                for (int i = 0; i < slot.item.stats.Length; i++)
                {
                    for (int j = 0; j < playerAttributes.Length; j++)
                    {
                        if (playerAttributes[j].type == slot.item.stats[i].stats)
                            playerAttributes[j].value.RemoveModifier(slot.item.stats[i]);
                    }
                }

                break;
            default:
                break;
        }
    }

    public void OnAddItem(InventorySlot slot)
    {
        if (slot.ItemObject == null)
            return;

        switch (slot.parent.inventory.type)
        {
            case InterfaceType.Inventory:
                break;
            case InterfaceType.Equipment:
                for (int i = 0; i < slot.item.stats.Length; i++)
                {
                    for (int j = 0; j < playerAttributes.Length; j++)
                    {
                        if (playerAttributes[j].type == slot.item.stats[i].stats)
                            playerAttributes[j].value.AddModifier(slot.item.stats[i]);
                    }
                }
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var item = other.GetComponent<ItemPhysical>();
        if (item)
        {
            Item _item = new Item(item.item);
            inventory.AddItem(_item, 1, _item.weight);
            var marker = other.GetComponent<Marker>();
            compass.DeleteMarker(marker);
            Destroy(other.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        var coll = collision.gameObject.GetComponent<MonsterStateMachine>();

        if (coll)
        {
            Debug.Log("Monster collided with player");

        }
    }

    public void AttributeModified(PlayerAttributes attribute)
    {
#if UNITY_EDITOR
        Debug.Log($"{attribute.type} changed to {attribute.value.ModifiedValue} points");
#endif
    }

    private void OnApplicationQuit()
    {
        inventory.inventoryContainer.Slots = new InventorySlot[24];
    }
}
