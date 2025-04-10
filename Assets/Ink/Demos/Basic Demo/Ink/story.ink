VAR startQuest = false
VAR hasItem = false
VAR completeQuest = false

-> check_loop

=== check_loop ===
~ startQuest = true
I looked at Monsieur Fogg... and I could contain myself no longer.
'What is the purpose of our journey, Monsieur?'
'A wager,' he replied.
-> wager_response

=== wager_response ===
'A wager!'[] I returned.
He nodded.
+ ['But surely that is foolishness!']
    'A most serious matter then!'
    He nodded again.
    -> wager_questions
+ ['A most serious matter then!']
    He nodded again.
    -> wager_questions

=== wager_questions ===
+ ['But can we win?']
    'That is what we will endeavour to find out,' he answered.
    -> wager_questions
+ ['A modest wager, I trust?']
    'Twenty thousand pounds,' he replied, quite flatly.
    -> wager_questions
+ [Give the required item]
    {hasItem:
        ~ completeQuest = true
        Ah, I see you have what I seek. Very well...
        -> ending
    - else:
        You don't have what I need. Find it and return to me.
        -> wager_questions
    }
+ [Say nothing further]
    I asked nothing further of him then[.], and after a final, polite cough, he offered nothing more to me. <>
    'Ah[.'],' I replied, uncertain what I thought.
    After that, <>
    we passed the day in silence.
    -> END

=== ending ===
Twice!? Beaten by an object... Twice! I've only known the taste of victory, but this taste... 
Is Is this my blood? Haha I've never known such... Such... relief..?
I- I need some time to think... We will meet again, machine. May your woes be many... and your days few.
-> END