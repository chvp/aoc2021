s" ../lib.fs" included

: find-2020-sum { base length }
  0 0
  begin
    2dup
    cells base + @
    swap
    cells base + @
  2dup + 2020 = invert while
    2drop
    1+
    dup length >= if
      drop
      1+
      0
    then
  repeat
  2swap 2drop ;
: find-2020-triple-sum { base length }
  0 0 0
  begin
    3dup
    cells base + @
    swap
    cells base + @
    rot
    cells base + @
  3dup + + 2020 = invert while
    3drop
    1+
    dup length >= if
      drop
      1+
      dup length >= if
        drop
        1+
        0
      then
      0
    then
  repeat
  3swap 3drop ;
: part1
  s" input" read-file-into-numbers find-2020-sum * . ;
: part2
  s" input" read-file-into-numbers find-2020-triple-sum * * . ;

s" Part 1: " type part1 CR
s" Part 2: " type part2 CR
bye
