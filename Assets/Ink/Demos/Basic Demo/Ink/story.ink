VAR startQuest = false
VAR hasItem = false
VAR completeQuest = false
VAR placeMarker = false

-> check_loop

=== check_loop ===
~ startQuest = true
You hear the Trader fiddle with the "Scrap" you brought for a while...
Then all you can hear is silance and the wind once again.
-> quest_start

=== quest_start ===

[Trader]: That was perfect... Just what I was looking for. 

+ What is it exacly?
    You fight back your fear and slowly walk up to the mirror
    -> walkup
+ Walk away
-> ending

=== walkup ===
[Trader]: Oh nothing for that little brain of yours to be conserned about... but maybe if you help me again I might give you a hint.
+ Reluctantly agree to help again.
-> agree
+ Shake your head and demand information.
-> disagree

=== agree ===
~ placeMarker = true
[Trader]: I like the sound of that.  
-> quest

=== disagree ===
[Trader]: Well fine, fine... That "Scrap" you are collecting can help our little boat in more ways than you can imagine,
check whats under the deck and you should find what I'm talking about.
-> ending

== quest ==
[Trader]: Very well, again I will mark where you can find the next artifact so bring it back to me like you did last time.

+ [Give the artifact] 
    {hasItem:
        ~ completeQuest = true
        Well done, we really work well together(together).
        -> ending
    - else:
        [Trader]: Still not blind try again next time.
        -> ending
    }

=== ending ===
You decide to leave the room.
-> END
