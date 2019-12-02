INCLUDE intcode.fs

19690720 CONSTANT wanted-output
: calculate-answer  ( -- u )  100 noun @ * verb @ + ;

: go  ( -- )
  s" input02.txt" load-bytecode
  save-bytecode
  100 0 DO
    100 0 DO
      restore-bytecode
      J I patch-program
      execute-program
      output @ wanted-output = IF
        ." End of program with output #0: [" output @ .. ." ] "
        ." noun #1: [" noun @ .. ." ] "
        ." verb #2: [" verb @ .. ." ], "
        ." (100 * noun + verb): [" calculate-answer .. ." ] "
        CR UNLOOP UNLOOP EXIT
      THEN
    LOOP
  LOOP ;
