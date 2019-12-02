INCLUDE intcode.fs

: go  ( -- )
  s" input02.txt" load-bytecode
  12 2 patch-program
  execute-program
  ." End of program with output #0: [" output @ .. ." ] " CR ;
