VAR trying_alone = false
VAR got_reward = false
EXTERNAL is_scale_solved()

=== npc_intro ===
{ 
    - got_reward: 
        -> npc_done
    - is_scale_solved(): 
        -> npc_solved
    - trying_alone: 
        -> npc_unsolved_again
    - else: 
        -> npc_unsolved_first
}

=== npc_unsolved_first ===
Oh, another person looking at this strange machine. I've been trying to fix it for hours... #voice:npc_intro_01
-> unsolved_choices

=== npc_unsolved_again ===
Ohhh you again? Still trying to figure it out? #voice:npc_again_01
-> unsolved_choices

=== unsolved_choices ===
+ [What is this machine?] 
    -> ask_machine
+ [I will figure it out myself, thanks.] 
    ~ trying_alone = true
    -> npc_end

=== ask_machine ===
It's a very sensitive weight scale. It needs exact numbers. You can't just put all the things on it; it needs a perfect balance. #voice:npc_machine_info
+ [Any tips on how to balance it?] 
    -> ask_hint
+ [I see. I'll try to handle it.] 
    ~ trying_alone = true
    -> npc_end

=== ask_hint ===
Well, observe the boxes around us. Their colors are the secret. #voice:npc_hint_01
The red crates are the heaviest of all. But red alone won't work. You need to mix that weight with something bright... Together, they might reach the perfect number. #voice:npc_hint_02
+ [I'll remember that.] 
    ~ trying_alone = true
    -> npc_end

=== npc_solved ===
// Ez a rész csak akkor jön be, ha az is_scale_solved() C# függvény TRUE-t ad vissza
~ got_reward = true
Wow... The scale is perfectly balanced! You actually did it. #voice:npc_solved_01
+ [So, what happens now?]
    The path is clear now. Oh, and take this as a thank you! #spawn_green_key #voice:npc_solved_02
    -> npc_end

=== npc_done ===
// Ez jön be, ha már megkaptad a kulcsot
I told you everything I know. The path is clear now. #voice:npc_done_01
-> DONE

=== npc_end ===
Good luck! #voice:npc_bye
-> DONE