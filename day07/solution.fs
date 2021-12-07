s" ../lib.fs" included

: part1-dist - abs ;

: part2-dist
  part1-dist dup 1 + * 2 /
;

: min-fuel
  { xt a-addr n max }
  -1
  max 0 do
    a-addr i cells + @
    0
    n 0 do
      a-addr i cells + @
      2 pick xt execute +
    loop
    nip
    umin
  loop
;

: main
  next-arg to-number
  1 = if
    ['] part1-dist
  else
    ['] part2-dist
  then
  max-line chars allocate throw dup dup
  next-arg fopen
  read-single-line drop
  comma-line-to-numbers
  to-array
  rot free throw
  2dup array-max
  min-fuel
  . CR
;

main bye
