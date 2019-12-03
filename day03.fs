\ https://adventofcode.com/2019/day/3

\ UTILITIES ----------------------------------------
: UNDER+  ( a b c -- a+c b )  ROT + SWAP ;
: UNDER-  ( a b c -- a+c b )  ROT SWAP - SWAP ;


\ I/O ----------------------------------------------
VARIABLE input-file

: open-input  ( -- )
  s" input03.txt" R/O OPEN-FILE ABORT" error opening file" input-file ! ;
: close-input  ( -- ) input-file @ CLOSE-FILE ABORT" error closing file" ;

: array CREATE ALLOT ;
4096 CONSTANT line-length
line-length array str

: read-wire-data  ( -- n f )
  str line-length ERASE
  str line-length input-file @ READ-LINE ABORT" error" ;


\ DIRECTIONS----------------------------------------
: get-next-direction  ( addr -- addr' )
  { ptr -- }
  BEGIN
    ptr C@ CASE
      [CHAR] U OF -1 ENDOF
      [CHAR] D OF -1 ENDOF
      [CHAR] R OF -1 ENDOF
      [CHAR] L OF -1 ENDOF
      ptr 1+ TO ptr
      0
    ENDCASE
  UNTIL
  ptr ;
: parse-number  ( addr n -- u )  2>R 0 0 2R> >NUMBER DROP ;


\ SEGMENTS -----------------------------------------
: make-segment  ( ox oy dir length -- ox oy ux uy )
  SWAP CASE
    [CHAR] U OF >R 2DUP R> + ENDOF
    [CHAR] D OF >R 2DUP R> - ENDOF
    [CHAR] R OF >R 2DUP R> UNDER+ ENDOF
    [CHAR] L OF >R 2DUP R> UNDER- ENDOF
    ." Invalid direction!" CR ABORT
  ENDCASE ;
: normalize-segment  ( ox oy ux uy -- ox' oy' ux' uy' )
  { ox oy ux uy | spare -- ox' oy' ux' uy' }
  ux ox < IF  ox TO spare  ux TO ox  spare TO ux  THEN
  uy oy < IF  oy TO spare  uy TO oy  spare TO uy  THEN
  ox oy ux uy ;

: vertical?  ( ox oy ux uy -- f )  DROP NIP = ;
: horizontal?  ( ox oy ux uy -- f ) NIP ROT DROP = ;

\ check for intersection of two normalized segments
\ ASSUMPTION: always vertical against horizontal lines
: intersect?  ( s1* s2* -- coords f | f )
  { ox1 oy1 ux1 uy1 ox2 oy2 ux2 uy2 -- }
  ox1 oy1 ux1 uy1 horizontal? IF
    ox2  ox1 ux1 1+ WITHIN
    oy1  oy2 uy2 1+ WITHIN
    AND
    IF ox2 oy1 -1 ELSE 0 THEN
  ELSE
    ox1  ox2 ux2 1+ WITHIN
    oy2  oy1 uy1 1+ WITHIN
    AND
    IF ox1 oy2 -1 ELSE 0 THEN
  THEN ;


\ SEGMENT ARRAYS (WIRES) ---------------------------
2000 CELLS 4 * ALLOCATE ABORT" Error allocating memory" CONSTANT wire1
2000 CELLS 4 * ALLOCATE ABORT" Error allocating memory" CONSTANT wire2

VARIABLE #wire1
VARIABLE #wire2

: store-segment  ( ox oy ux uy dest -- )
  { dest }
  dest 3 CELLS + !
  dest 2 CELLS + !
  dest CELL+ !
  dest ! ;
: fetch-segment  ( addr -- ox oy ux uy )
  { addr }
  addr @
  addr CELL+ @
  addr 2 CELLS + @
  addr 3 CELLS + @ ;

: print-segment  ( addr -- ) { addr } 4 0 DO addr I CELLS + ? LOOP CR ;

: build-wire-array  ( dest addr n -- )
  { dest ptr len | max x y dir count }
  ptr len + TO max
  BEGIN
    ptr max <
  WHILE
    \ read direction and length
    ptr get-next-direction TO ptr
    ptr C@ TO dir
    ptr 1+ TO ptr
    ptr 8 parse-number TO ptr DROP TO count
    \ add a new segment to the array
    x y dir count make-segment
    2DUP TO y TO x
    dest store-segment
    dest 4 CELLS + TO dest
  REPEAT
  ." Stored segments" CR ;

\ MANHATTAN DISTANCE -------------------------------
VARIABLE best-x
VARIABLE best-y
VARIABLE best-mhd

: mhd  ( x y -- n )  ABS SWAP ABS + ;

: get-nearest-crossing  ( -- )
  999999999 best-mhd !
  { | x y d }
  #wire1 @ 0 DO
    #wire2 @ 0 DO
      wire1 J CELLS 4 * + fetch-segment
      wire2 I CELLS 4 * + fetch-segment
      intersect? IF
        TO y TO x
        x y mhd  0>  x y mhd best-mhd @ <  AND IF
          x y mhd best-mhd ! y best-y ! x best-x !
          ." New best distance: " best-mhd ? CR
        THEN
      THEN
    LOOP
  LOOP ;

\ ENTRY POINT --------------------------------------
: go  ( -- )
  open-input
  read-wire-data DROP #wire1 !
  wire1 str #wire1 @ build-wire-array
  read-wire-data DROP #wire2 !
  wire2 str #wire2 @ build-wire-array
  close-input
  \ normalize segments
  #wire1 @ 0 DO
    wire1 I CELLS 4 * + fetch-segment
    normalize-segment
    wire1 I CELLS 4 * + store-segment
  LOOP
  #wire2 @ 0 DO
    wire2 I CELLS 4 * + fetch-segment
    normalize-segment
    wire2 I CELLS 4 * + store-segment
  LOOP
  get-nearest-crossing ;
