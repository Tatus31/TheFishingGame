using Cinemachine;
using Game;
using UnityEngine;

public static class Events
{
   public static OnEnteredInteraction onEnteredInteraction = new OnEnteredInteraction();
}

public class OnEnteredInteraction : GameEvent
{
    public bool CanShowPanel = true;
}

