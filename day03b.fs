\ https://adventofcode.com/2019/day/3#part2

\ UTILITIES ----------------------------------------
: UNDER+  ( a b c -- a+c b )  ROT + SWAP ;
: UNDER-  ( a b c -- a+c b )  ROT SWAP - SWAP ;
: array CREATE ALLOT ;


\ I/O ----------------------------------------------
VARIABLE input-file

: open-input  ( -- )
  s" input03.txt" R/O OPEN-FILE ABORT" error opening file" input-file ! ;
: close-input  ( -- ) input-file @ CLOSE-FILE ABORT" error closing file" ;

4096 CONSTANT line-length
line-length array str

: read-wire-data  ( -- n f )
  str line-length ERASE
  str line-length input-file @ READ-LINE ABORT" error" DROP ;


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
: parse-number  ( addr n -- u addr' )  2>R 0 0 2R> >NUMBER DROP NIP ;


\ SEGMENTS -----------------------------------------
5 CONSTANT /segment

: ox  ( segment -- n )  ;
: oy  ( segment -- n )  CELL+ ;
: ux  ( segment -- n )  2 CELLS + ;
: uy  ( segment -- n )  3 CELLS + ;
: run-length  ( segment -- n )  4 CELLS + ;

: vertical?  ( segment -- f ) DUP ox @ SWAP ux @ = ;
: horizontal?  ( segment -- f ) DUP oy @ SWAP uy @ = ;

: make-segment  ( ox oy dir length -- ox oy ux uy )
  SWAP CASE
    [CHAR] U OF >R 2DUP R> + ENDOF
    [CHAR] D OF >R 2DUP R> - ENDOF
    [CHAR] R OF >R 2DUP R> UNDER+ ENDOF
    [CHAR] L OF >R 2DUP R> UNDER- ENDOF
    ." Invalid direction!" CR ABORT
  ENDCASE ;

: normalize-segment  ( segment -- ox oy ux uy )
  { s }
  s ox @ s ux @ 2DUP > IF SWAP THEN
  s oy @ s uy @ 2DUP > IF SWAP THEN
  ROT SWAP s run-length ;

: store-segment  ( ox oy ux uy acc dest -- )
  { dest }
  dest run-length !  dest uy !  dest ux !  dest oy !  dest ox ! ;
: fetch-segment  ( addr -- ox oy ux uy )
  { addr }
  addr ox @  addr oy @  addr ux @  addr uy @  addr run-length @ ;

: print-segment  ( addr -- )
  { addr }
  /segment 0 DO addr I CELLS + ? LOOP CR ;

: intersects-vertically?  ( sv sh -- f )
  { sv sh -- f }
  sv ox @  sh ox @ sh ux @ 1+ WITHIN
  sh oy @  sv oy @ sv uy @ 1+ WITHIN
  AND ;
: intersects-horizontally?  ( sv sh -- f )
  { sv sh -- f }
  sv ox @  sh ox @ sh ux @ 1+ WITHIN
  sh oy @  sv oy @ sv uy @ 1+ WITHIN
  AND ;

\ check for intersection of two normalized segments
\ ASSUMPTION: always vertical against horizontal lines
: intersect?  ( s1 s2 -- coords f | f )
  { s1 s2 }
  s1 horizontal? IF
    s2 s1 intersects-vertically? IF s2 ox @ s1 oy @ -1 ELSE 0 THEN
  ELSE
    s1 s2 intersects-horizontally? IF s1 ox @ s2 oy @ -1 ELSE 0 THEN
  THEN ;


\ SEGMENT ARRAYS (WIRES) ---------------------------
: segments  ( n -- n' )  /segment * CELLS ;
: segment+  ( addr -- addr' )  1 segments + ;
: th-segment  ( addr i -- addr' )  /segment * CELLS + ;

2000 segments ALLOCATE ABORT" Error allocating" CONSTANT wire1
2000 segments ALLOCATE ABORT" Error allocating" CONSTANT wire2
2000 segments ALLOCATE ABORT" Error allocating" CONSTANT wire1-normalized
2000 segments ALLOCATE ABORT" Error allocating" CONSTANT wire2-normalized

VARIABLE #wire1
VARIABLE #wire2

: build-wire-array  ( dest addr n -- )
  { dest ptr len | max x y dir count acc }
  ptr len + TO max
  BEGIN
    ptr max <
  WHILE
    \ read direction and length
    ptr get-next-direction TO ptr
    ptr C@ TO dir
    ptr 1+ TO ptr
    ptr 8 parse-number TO ptr TO count
    \ add a new segment to the array
    x y dir count make-segment  2DUP TO y TO x
    acc dest store-segment
    count acc + TO acc  dest segment+ TO dest
  REPEAT
  ." Stored segments" CR ;


\ MANHATTAN DISTANCE -------------------------------
VARIABLE best-x
VARIABLE best-y
VARIABLE best-mhd
VARIABLE best-steps

: mhd  ( x y -- n )  ABS SWAP ABS + ;
: better-mhd?  ( x y -- f )  2DUP mhd  0>  -ROT mhd best-mhd @ <  AND ;

: steps  ( segment x y -- n )
  { segment x y }
  segment run-length @
  segment ox @ x - abs +
  segment oy @ y - abs + ;

: shorter-route?  ( s1 s2 x y -- f )
  { s1 s2 x y }
  s1 x y steps  s2 x y steps +  best-steps @ < ;

\ normalized wires are only needed to calculate the intersections
: normalize-wire  ( addr dest count -- )
  { addr dest count }
  count 0 DO
    addr I th-segment
    normalize-segment
    dest I th-segment store-segment
  LOOP ;

: get-nearest-crossing  ( -- )
  999999999 DUP best-mhd ! best-steps !
  { | x y }
  #wire1 @ 0 DO
    #wire2 @ 0 DO
      wire1-normalized J th-segment
      wire2-normalized I th-segment
      intersect? IF
        TO y TO x
        x y better-mhd?
        wire1 J th-segment wire2 I th-segment
        { s1 s2 }
        s1 s2 x y shorter-route? AND IF
          x y mhd best-mhd !
          s1 x y steps s2 x y steps + best-steps !
          y best-y ! x best-x !
          ." New best distance: " best-mhd ? CR
        THEN
      THEN
    LOOP
  LOOP ;


\ ENTRY POINT --------------------------------------
: go  ( -- )
  open-input
  read-wire-data #wire1 !
  wire1 str #wire1 @ build-wire-array
  read-wire-data #wire2 !
  wire2 str #wire2 @ build-wire-array
  close-input
  wire1 wire1-normalized #wire1 @ normalize-wire
  wire2 wire2-normalized #wire2 @ normalize-wire
  get-nearest-crossing ;
