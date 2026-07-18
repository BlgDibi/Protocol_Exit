=== receptionist_intro ===
Welcome! You're just in time—it's not too late for you to get out of here. #voice:rec_intro_1

Not every door opens with a key. For some, you will have to rely purely on your logic. Pay close attention to the details. #voice:rec_intro_2

There are old photographs hanging on the walls. Decipher them to find the correct room number! #voice:rec_intro_3
-> receptionist_questions

=== receptionist_questions ===
* [What do these photographs mean?]
    Look closely at what is depicted in them. Nothing is there by accident. Every single picture holds a hidden value if you look hard enough. #voice:rec_hint_1
    -> receptionist_questions


* [What is the math behind it?]
    Read the pictures from left to right. Add them together, then divide. #voice:rec_hint_2
    -> receptionist_questions

* [Can you just give me the exact formula?]
    I'm afraid not. If I say too much, it puts us both in danger. You have to figure out the calculation on your own. #voice:rec_refuse
    -> receptionist_questions


+ [Got it, I'll examine the pictures now.]
Keep your eyes open and trust your logic.#voice:rec_bye
    -> END

=== told_everything ===
Leave me alone! I already told you everything I know. #voice:rec_leave_me

We shouldn't be seen talking. Just focus on the photographs on the wall and find that room number! #voice:rec_focus
-> END