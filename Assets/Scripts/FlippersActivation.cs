using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class FlippersActivation : MonoBehaviour
{

   
    Player player;
    ItemObject item;

    

    // Update is called once per frame
    void Update()
    {
        Activate();
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

                    if (itemType.item.id == 13)
                    {
                        Debug.Log("Zalozylem Flippersy!");
                    }
                    else
                    {
                        Debug.Log("Nie zalozylem Flippersow");
                    }

                }
            }
        }

    }
}
