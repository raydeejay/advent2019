\ https://adventofcode.com/2019/day/2#part2

\ UTILITIES --------------------------------------------
: ..  ( u -- )  0 <# #S #> TYPE ;
: array  ( "name" size -- )  CREATE CELLS ALLOT ;
: IS   ( "name" -- )  ' , ; \ assist in deferring words
: digit?  ( c -- f )  [CHAR] 0 [CHAR] 9 1+ WITHIN ;


\ MEMORY -----------------------------------------------
4096 CONSTANT memory-size
memory-size array bytecode
memory-size array bytecode-backup

: save-bytecode  ( -- )  bytecode bytecode-backup memory-size MOVE ;
: restore-bytecode  ( -- )  bytecode-backup bytecode memory-size MOVE ;

bytecode CONSTANT output
bytecode CELL+ CONSTANT noun
bytecode 2 CELLS + CONSTANT verb


\ I/O --------------------------------------------------
VARIABLE input-file
VARIABLE pointer
VARIABLE input-char

: open-input  ( filename -- )
  R/O OPEN-FILE ABORT" error opening file" input-file ! ;
: close-input  ( -- )  input-file @ CLOSE-FILE ABORT" error closing file" ;

: read-digit  ( -- )
  input-char 1 input-file @ READ-FILE ABORT" Error reading digit" ;
: load-digit  ( -- )
  pointer @ @ 10 *              \ get current value * 10
  input-char @ [CHAR] 0 -  +    \ add the input digit
  pointer @ ! ;                 \ store it back
: skip-non-digit  ( -- )  1 CELLS pointer +! ;

: clear-bytecode  ( -- ) bytecode memory-size CELLS ERASE ;
: read-bytecode  ( -- )
  BEGIN read-digit
  WHILE
    input-char @ digit? IF
      load-digit
    ELSE skip-non-digit
    THEN
  REPEAT ;
: load-bytecode  ( filename -- )
  clear-bytecode
  bytecode pointer !
  open-input
  read-bytecode
  close-input
  bytecode pointer ! ;


\ OPCODES ----------------------------------------------
: fetch-arguments  ( -- addr u1 u2 )
  pointer @ CELL+ @             \ first bytecode offset argument
  CELLS bytecode + @            \ dereference
  pointer @ 2 CELLS + @         \ second bytecode offset
  CELLS bytecode + @            \ dereference
  pointer @ 3 CELLS + @  CELLS bytecode + \ destination *address*
  -ROT ;

: op-add      ( -- )       fetch-arguments + SWAP ! ;
: op-mult     ( -- )       fetch-arguments * SWAP ! ;
: op-halt     ( f -- f' )  DROP 0 ;
: op-illegal  ( f -- f' )
  ." Illegal opcode #" HEX pointer @ @ DECIMAL
  ."  at position #" HEX pointer @ bytecode - . DECIMAL CR
  op-halt ;

2 CELLS CONSTANT /opcode         \ bytes in table for each key
CREATE opcodes                   \ opcode table
 1 , IS op-add
 2 , IS op-mult
99 , IS op-halt
 0 , IS op-illegal
HERE /opcode - CONSTANT 'nomatch \ addr of no-match function

: 'function ( opcode -- addr-of-match )
  { | matched }
  'nomatch TO matched
  'nomatch opcodes DO
    DUP I @ = IF
      I TO matched LEAVE
    THEN
  /opcode +LOOP
  DROP matched ;


\ EXECUTION --------------------------------------------
: cycle  ( -- f )  -1  pointer @ @ 'function CELL+ @ EXECUTE ;
: advance  ( -- )  4 CELLS pointer +! ;
: patch-program  ( noun verb -- )  verb ! noun ! ;
: execute-program  ( -- )
  bytecode pointer !
  BEGIN cycle WHILE advance REPEAT ;
