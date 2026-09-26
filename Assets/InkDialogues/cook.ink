VAR met_chef = false
VAR chef_reward_given = false

=== chef_intro ===
#speaker:Chef #voice:chef_intro_1
Did you really have to use explosives? You are ruining my kitchen with all that dust! 

I know why you are here. You want to go upstairs. But you can't. The main pipe is leaking toxic gas. #voice:chef_intro_2
I took the gas valve to protect my supplies. I have it right here. #voice:chef_intro_3
I will give it to you, but you must do something for me first. The Director's dinner is tonight, and the dining room is a mess. #voice:chef_intro_4
~ met_chef = true
-> chef_questions

=== chef_questions ===
* [Who is coming to dinner?]
    The Director, the Professor, a loyal worker, and a test subject. They are strict about their table setup. If they get angry, we are both dead. #voice:chef_hint_1
    -> chef_questions

* [How should I set the table?]
    The Director always sits at the head of the table. He exclusively uses the silver knife. #voice:chef_hint_2
    As for the test subject... you must seat him as far away from the Director as possible, or he will attack him across the table. #voice:chef_hint_3
    -> chef_questions

* [Where are the items for the table?]
    Scattered. Check the dark pantry next door. You might find the old incident reports there, too. You will need them. Figure out the rest yourself. #voice:chef_hint_4
    -> chef_questions

+ [I will set the table for them.]
    Go to the dining room. Put the four items in their correct places. Don't make a mistake. #voice:chef_bye
    -> END

=== chef_waiting ===
Are you deaf? The Director is almost here! Check the pantry for the missing items and place them on the table! #speaker:Chef #voice:chef_waiting
-> END

=== chef_done ===
{ chef_reward_given:
    -> chef_leave_me
}
~ chef_reward_given = true
You actually placed everything correctly. I am impressed. #speaker:Chef #voice:chef_done_1

The valve is yours. I tossed it into that old grey metal cabinet over there. But to be honest... I have so much on my mind, I forgot which of the 12 drawers it's in. #voice:chef_done_2

There is a document somewhere around here. Find it, find the right drawer, take the valve and let me cook in peace. #give_cabinet_access #voice:chef_done_3
-> END

=== chef_leave_me ===
I gave you the valve. Let me cook in peace! #speaker:Chef #voice:chef_leave
-> END