VAR met_janitor = false

EXTERNAL check_condition(conditionName)
=== function check_condition(conditionName) ===
~ return false

=== mechanic_intro ===

{ check_condition("HasC4") == true: -> mechanic_leave_me }

{ check_condition("HasDetonator") == true: -> mechanic_done }

Another fresh face... Wonderful. #voice:janitor_intro_1

Look at this. If you are planning to use this C4 to clear the upper corridor, you are going to earn it first. #voice:janitor_intro_2

I happened to drop the detonator somewhere inside that green storage room. Pitch black. And I am certainly not stepping foot in there. #voice:janitor_intro_3

Go retrieve it from the dark. Bring it back to me in one piece, and maybe-just maybe-I will hand over the explosive. #voice:janitor_intro_4
~ met_janitor = true
-> mechanic_questions

=== mechanic_questions ===
* [Where exactly inside that room?]
    Where? Somewhere among the shadows and the dust. It is a maze in there, not a gift shop. Open your eyes. Or rather, let the light do the work. #voice:janitor_hint_1
    -> mechanic_questions

+ [Fine. I will find your detonator.]
    Tick-tock. Make it quick and watch your step in the dark. #voice:janitor_bye
    -> END

=== mechanic_waiting ===
I told you to find the detonator. Stop wasting my time and get it.#speaker:janitor #voice:janitor_waiting
-> END

=== mechanic_done ===
You actually made it back... Fascinating. Here is your precious C4. Do whatever you want with it, just get out of my sight. #voice:janitor_reward #spawn_c4
-> END

=== mechanic_leave_me ===
I already gave you what you wanted.  #voice:janitor_leave
-> END