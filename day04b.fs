\ https://adventofcode.com/2019/day/4

\ Find the password. Requirements are:

\ - It is a six-digit number.
\ - The value is within the range given in your puzzle input.
\ - Two adjacent digits are the same (like 22 in 122345).
\ - Going from left to right, the digits never decrease; they only ever
\   increase or stay the same (like 111123 or 135679).
\ - The two adjacent matching digits are not part of a larger group of
\   matching digits.

\ As with the other part, the requirements could be worded better,
\ since `111122` is considered valid because it has `22`...

353096 CONSTANT minimum
843212 CONSTANT maximum

: in-range?  ( password -- f )  minimum maximum 1+ WITHIN ;

CREATE digits 6 CELLS ALLOT
: th-digit  ( i -- addr )  CELLS digits + ;

: decompose  ( password -- )
  DUP              100000 /  0 th-digit !
  DUP   100000 MOD 10000  /  1 th-digit !
  DUP    10000 MOD 1000   /  2 th-digit !
  DUP     1000 MOD 100    /  3 th-digit !
  DUP      100 MOD 10     /  4 th-digit !
            10 MOD           5 th-digit ! ;

: doubles  ( -- n )
  0   5 0 DO I DUP th-digit @  SWAP 1+ th-digit @  = IF 1+ THEN LOOP ;

: triples  ( -- n )
  0
  4 0 DO
    I th-digit @ I 1+ th-digit @ =
    I th-digit @ I 2 + th-digit @ =
    AND IF 1+ THEN
  LOOP ;

: quadruples  ( -- n )
  0
  3 0 DO
    I th-digit @ I 1+ th-digit @ =
    I th-digit @ I 2 + th-digit @ =
    I th-digit @ I 3 + th-digit @ =
    AND AND IF 1+ THEN
  LOOP ;

: quintuples  ( -- n )
  0
  2 0 DO
    I th-digit @ I 1+ th-digit @ =
    I th-digit @ I 2 + th-digit @ =
    I th-digit @ I 3 + th-digit @ =
    I th-digit @ I 4 + th-digit @ =
    AND AND AND IF 1+ THEN
  LOOP ;

: sixtuples  ( -- n )
  0
  0 th-digit @ 1 th-digit @ =
  0 th-digit @ 2 th-digit @ =
  0 th-digit @ 3 th-digit @ =
  0 th-digit @ 4 th-digit @ =
  0 th-digit @ 5 th-digit @ =
  AND AND AND AND IF 1+ THEN ;

: not-decreasing?  ( -- f )
  5 0 DO I th-digit @ I 1+ th-digit @ > IF 0 UNLOOP EXIT THEN LOOP
  -1 ;

: extra-requirement?  ( -- f )
  sixtuples IF 0 EXIT THEN
  quintuples IF 0 EXIT THEN
  doubles triples quadruples { doubs trips quads }
  quads 1 = doubs 3 = AND IF 0 EXIT THEN
  quads 1 = doubs 4 = AND IF -1 EXIT THEN
  quads 0= doubs 1 = AND IF -1 EXIT THEN
  trips 2 = IF 0 EXIT THEN
  trips 1 = doubs 2 = AND IF 0 EXIT THEN
  trips 1 = doubs 3 = AND IF -1 EXIT THEN
  trips 0= doubs 2 = AND IF -1 EXIT THEN
  doubs 3 = IF -1 EXIT THEN
  doubs 0= IF 0 EXIT THEN
  doubs 1 = IF -1 EXIT THEN
  0 ;

: valid?  ( password -- f )
  DUP in-range? 0= IF 0 EXIT THEN
  decompose
  not-decreasing? extra-requirement? AND ;

: go  ( password -- )
  0
  maximum 1+ minimum DO I valid? IF 1+ I . THEN LOOP
  ." Found " . ." valid passwords." CR ;
