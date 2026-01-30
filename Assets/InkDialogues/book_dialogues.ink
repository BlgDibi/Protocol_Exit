// Változó: annyit jegyzünk meg, hogy elolvasta-e a naplót
VAR read_diary = false

// A KÖNYV (The Book)
=== book_interaction ===
It is a dusty journal left on the shelf. 
It looks old.

// CSAK AKKOR LÁTSZIK, HA MÉG NEM OLVASTUK
+ {not read_diary} [Read it] 
    -> read_entry

// CSAK AKKOR LÁTSZIK, HA MÁR ELOLVASTUK
+ {read_diary} [Move it aside]
    You push the book slightly to the left.
    There is a writing on the wall!
    #move_book
    -> DONE

// EZ MINDIG LÁTSZIK
+ [Close it] 
    -> DONE
    
= read_entry
Day 6... 
I am still trapped here. The doors won't open.
If you are reading this... be careful.
Look closely at everything.
Keep your eyes open.

// Játékos elolvasta - átállítjuk a kapcsolót
~ read_diary = true 

+ [Close the book]
 I need to look around.
    -> DONE