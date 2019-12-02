\ https://adventofcode.com/2019/day/1#part2

128 CONSTANT line-length

: UNDER+  ( a b c -- a+c b )  ROT + SWAP ;
: array CREATE ALLOT ;
line-length array str

VARIABLE input-file
VARIABLE total

: open-input  ( -- )
  s" input01.txt" R/O OPEN-FILE ABORT" error opening file" input-file ! ;
: close-input  ( -- ) input-file @ CLOSE-FILE ABORT" error closing file" ;

: read-number  ( -- n f )
  str line-length input-file @ READ-LINE ABORT" error" ;
: parse-number  ( n -- u )  >R 0 0 str R> >NUMBER 2DROP DROP ;

: mass>fuel  ( u -- u' )
  0 SWAP
  BEGIN 3 / 2- ?DUP 0> WHILE DUP UNDER+ REPEAT ;

: go  ( -- )  
  open-input
  0 total !
  BEGIN read-number WHILE parse-number mass>fuel total +! REPEAT
  ." Total fuel needed is: " total ? CR
  close-input ;
