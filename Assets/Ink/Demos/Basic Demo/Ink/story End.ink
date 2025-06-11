VAR startQuest = false
VAR hasItem = false
VAR completeQuest = false
VAR placeMarker = false

-> check_loop

=== check_loop ===
~ startQuest = true
You hear the Trader laugh maniacally behind the counter. 
-> quest_start

=== quest_start ===

[Trader]: Finally it's almost done!

+ What is?
    You ask feeling goosebumps rising on your skin.
    -> walkup

=== walkup ===
[Trader]: The ship is! All it needs now is the last artifact and everyting will work like it used to.
-> agree

=== agree ===
~ placeMarker = true
[Trader]: Just find the last piece and we can leave this hellhole together.  
+Can we really?
-> quest

== quest ==
[Trader]: Of course! Did I ever lie to you, now get to it and we can finally be done with this place forever.

+ [Give the artifact] 
    {hasItem:
        ~ completeQuest = true
        Well done, we really work well together(together).
        -> endingtrue
    - else:
        [Trader]: I don't have the time to be playing with you now.
        -> ending
    }

=== ending ===
You decide to leave the room.
-> END

=== endingtrue ===
-> END