using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolBoxActivation : MonoBehaviour
{
    
   
    Player player;
    ItemObject item;


    private void Update()
    {
        Activate();
    }

    private void Awake()
    {
        
    }

    private void Start()
    {
        player = GetComponent<Player>();
    }

    

    public void Activate()
    {

        for (int i = 0; i < player.equipment.GetSlots.Length; i++)
        {
            var itemType = player.equipment.GetSlots[i];

            for (int j = 0; j < itemType.AllowedItems.Length; j++)
            {

                if (itemType.AllowedItems[j] == ItemType.fireExtinguisher)
                {
                    if (itemType.item.id == -1)
                    {
                        return;
                    }

                    if (itemType.item.id == 12)
                    {
                        if (Input.GetKey(KeyCode.Mouse1))
                        {
                            Debug.Log("Naprawiam");
                        }
                        else
                        {
                            Debug.Log("Dupa nie dziala");
                        }
                    }

                }
            }
        }

    }

    
}
