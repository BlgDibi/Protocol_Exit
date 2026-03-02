EXTERNAL is_scale_solved()

=== npc_intro ===
{ is_scale_solved():
    -> npc_solved
- else:
    -> npc_unsolved
}

=== npc_unsolved ===
Oh, another person looking at this strange machine. I've been trying to fix it for hours... #play_audio

* [What is this machine?] -> ask_machine
* [I will figure it out myself, thanks.] -> npc_end

=== ask_machine ===
It's a very sensitive weight scale. It needs exact numbers. You can't just put all the things on it; it needs a perfect balance.

* [Any tips on how to balance it?] -> ask_hint
* [I see. I'll try to handle it.] -> npc_end

=== ask_hint ===
Well, observe the boxes around us. Their colors are the secret. 
The red crates are the heaviest of all. But red alone won't work. You need to mix that weight with something bright... Together, they might reach the perfect number.
* [I'll remember that.] -> npc_end

=== npc_solved ===
Wow... The scale is perfectly balanced! You actually did it. #play_audio
* [So, what happens now?]
    The path is clear now. Oh, and take this as a thank you! #spawn_green_key
    -> npc_end

=== npc_end ===
Good luck!
-> DONE