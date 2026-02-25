VAR read_diary = false

=== book_interaction ===
// Ha először nézel rá, ezt írja, ha már olvastad, akkor csak visszatér ide
{not read_diary: It is a dusty journal left on the shelf. It looks old. | What do you want to do?}

+ {not read_diary} [Read it] 
    -> read_entry

+ {read_diary} [Move it aside]
    You push the book slightly to the left.
    #move_book
    -> DONE

+ [Close it] 
    -> DONE
    
= read_entry
Day 6... 
I am still trapped here. The doors won't open.
If you are reading this... be careful.
Look closely at everything.
Keep your eyes open.

~ read_diary = true 

+ [Continue...] 
    -> book_interaction
    
    
    
