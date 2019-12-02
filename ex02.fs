\ https://adventofcode.com/2019/day/2

256 CONSTANT line-length

: array CREATE CELLS ALLOT ;
line-length array bytecode

VARIABLE input-file
VARIABLE pointer
VARIABLE input-char

: open-input  ( -- )
  s" input02.txt" R/O OPEN-FILE ABORT" error opening file" input-file ! ;
: close-input  ( -- ) input-file @ CLOSE-FILE ABORT" error closing file" ;

: digit?  ( c -- f )  [CHAR] 0 [CHAR] 9 1+ WITHIN ;
: read-digit  ( -- )
  input-char 1 input-file @ READ-FILE ABORT" Error reading digit" ;
: load-digit  ( -- )
  pointer @ @ 10 *  input-char @ [CHAR] 0 -  +   pointer @ ! ;
: skip-token  ( -- )  1 CELLS pointer +! ;

: read-bytecode  ( -- )
  bytecode pointer !
  BEGIN
    read-digit
  WHILE
    input-char @ digit? IF load-digit ELSE skip-token THEN
  REPEAT ;

: fetch-arguments  ( -- addr u1 u2 )
  pointer @ CELL+ @ \ first bytecode offset argument
  CELLS bytecode + @ \ dereference
  pointer @ 2 CELLS + @ \ second bytecode offset
  CELLS bytecode + @ \ dereference
  pointer @ 3 CELLS + @  CELLS  bytecode + \ destination *address*
  -ROT
;

: op-add  ( -- )  fetch-arguments + SWAP ! ;
: op-mult  ( -- )  fetch-arguments * SWAP ! ;
: op-halt  ( f -- f' )  ." End of program with #0: " bytecode ? CR DROP 0 ;

: cycle  ( -- f )
  -1     ( return true by default )
  pointer @ @ CASE
    1 OF op-add ENDOF
    2 OF op-mult ENDOF
    99 OF op-halt ENDOF
    ." Illegal opcode at position " HEX pointer @ bytecode - . DECIMAL CR
    DROP 0
  ENDCASE
;

: advance  ( -- )  4 CELLS pointer +! ;

: patch-program  ( -- )
  12 bytecode CELL+ !
  2 bytecode 2 CELLS + !
;

: init  ( -- )
  open-input
  read-bytecode
  close-input
  patch-program
  bytecode pointer !
;

: go  ( -- )
  init
  BEGIN cycle WHILE advance REPEAT
;
