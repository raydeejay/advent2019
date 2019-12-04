\ https://adventofcode.com/2019/day/4

\ Find the password. Requirements are:

\ - It is a six-digit number.
\ - The value is within the range given in your puzzle input.
\ - Two adjacent digits are the same (like 22 in 122345).
\ - Going from left to right, the digits never decrease; they only ever
\ - increase or stay the same (like 111123 or 135679).

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

: doubles  ( password -- n )
  decompose
  0   5 0 DO I th-digit @ I 1+ th-digit @ = IF 1+ THEN LOOP ;

: not-decreasing?  ( password -- f )
  decompose
  5 0 DO I th-digit @ I 1+ th-digit @ > IF 0 UNLOOP EXIT THEN LOOP
  -1 ;

: valid?  ( password -- f )
  DUP in-range?
  OVER doubles 0> AND
  SWAP not-decreasing? AND ;

: go  ( password -- )
  0
  maximum 1+ minimum DO I valid? IF 1+ I . THEN LOOP
  ." Found " . ." valid passwords." CR ;
