VAR startQuest = false
VAR hasItem = false
VAR completeQuest = false

-> check_loop

=== check_loop ===
~ startQuest = true
You there—traveler! A relic of old lies hidden in the ruins.
Will you find it for me?
-> quest_start

=== quest_start ===
I sense the artifact's presence... dusty, ancient, powerful. 
Bring it to me. Only then shall your path be clear.

+ ...
    wind rustles the grass.
    -> ponder
+ [Give the artifact] 
    {hasItem:
        ~ completeQuest = true
        You have it... The relic of the First Flame. Thank you.
        -> ending
    - else:
        You don’t have it yet. Search the ruins—time fades it fast.
        -> quest_start
    }

=== ponder ===
The old world crumbled under its own weight. Kingdoms lost, gods silenced...
And yet we cling to echoes.
You seek purpose in ruins, and I in what remains of faith.
-> agree

=== agree ===
Then take this moment—and guard it well.
+ [Nod and hold out the relic]
    -> ending

=== ending ===
It is done. The relic is safe once more. 
Strange... I feel lighter. As if time itself loosened its grip.
Go now, traveler. Others may yet need your hand.
-> END
