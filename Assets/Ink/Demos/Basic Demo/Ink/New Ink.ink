VAR startJump = false
VAR hasItem = false
VAR completeQuest = false

-> check_loop

=== check_loop ===
~ startJump = true
BEHOLD! THE POWER OF AN ANGEL!
-> jump1

=== jump1 ===
Machine, I know you're here. I can smell the insolent stench of your bloodstained hands. 
I await you down below. Come to me.

+ ...
    nikon noises. 
    -> astonished
+ [Give the required item] 
    {hasItem:
        ~ completeQuest = true
        Ah, I see you have what I seek. Very well...
        -> ending
    - else:
        You don't have what I need. Find it and return to me.
        -> jump1
    }

=== astonished ===
Limbo, Lust, all gone... With Gluttony soon to follow. 
Your kind know nothing but hunger; purged all life on the upper layers, and yet they remain unsatiated... As do you.
You've taken everything from me, machine. And now all that remains is PERFECT HATRED.
-> nod

=== nod ===
get parried bitch.
+ [3 dollar shrimp special]
    -> ending

=== ending ===
Twice!? Beaten by an object... Twice! I've only known the taste of victory, but this taste... 
Is Is this my blood? Haha I've never known such... Such... relief..?
I- I need some time to think... We will meet again, machine. May your woes be many... and your days few.
-> END