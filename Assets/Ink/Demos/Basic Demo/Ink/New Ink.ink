VAR startQuest = false
VAR hasItem = false
VAR completeQuest = false
VAR placeMarker = false

-> check_loop

=== check_loop ===
~ startQuest = true
You walk up to a strange warped one way mirror and behind it you see.... 
nothing no reflection no siluete 
but for some inexplicable reason you know that behind that steined glass is something beckoning you to talk to it...   
-> quest_start

=== quest_start ===

[???]: Come here do not be afraid. 

+ Lisen to the voice.
    You fight back your fear and slowly walk up to the mirror.
    -> walkup
+ Walk away.
-> ending

=== walkup ===
[???]: Ah Finally someone new.... Oh but before I forget let me introduce myself, you can call me The Trader.

[Trader]: Or at least thats what everyone used to call me here back in the day but I'm loosing track of whats important here. 
Are you willing to help me out a bit in here, obviously not for free mind you.
+ Nod your head and tell him you agree.
-> agree
+ Shake your head and tell him you disagree.
-> disagree

=== agree ===
~ placeMarker = true
[Trader]: well I didn't expect you to be so willing to help. 
-> quest

=== disagree ===
[Trader]: well then suit yourself... I know you will be back.
-> ending

== quest ==
[Trader]: Well then all I need of you is to just collect a bunch of scrap for me... nothing dangerous at least for you,
 mind you the wildlife here seems to dislike my boat with a burning passion for whatever reason so make sure to patch it every now and then.

+ [Give the artifact] 
    {hasItem:
        ~ completeQuest = true
        Well finally... Took you long enough but still here's a treat for a job well done.
        -> ending
    - else:
        [Trader]: Nice try but I'm not blind at least yet.
        -> ending
    }

=== ending ===
You decide to leave the room.
-> END
