needs ../lib.fs

: bit-at
  { n u -- u }
  n
  u 0 > if
    u 0 do
      2 /
    loop
  then
  2 mod
;

: count-ones-at-bit
  { base n u -- u }
  0
  n 0 do
    base i cells + @
    u bit-at if
      1 +
    then
  loop ;

: construct-numbers
  { base n u -- gamma epsilon }
  0 ( acc )
  u ( counter )
  begin
    1 -
    dup 0 >= while
    swap 2 * swap
    dup
    base swap
    n swap ( acc counter base n counter )
    count-ones-at-bit ( acc counter bits )
    n 2 / > if
      swap 1 + swap
    then
  repeat
  drop
  dup ( acc acc )
  0
  u 0 do
    2 *
    1 +
  loop
  xor
  base free throw ;

: highest-bit-set
  ( n -- u )
  0
  begin
    1+
    swap
    2 /
    dup while
    swap
  repeat
  drop ;

: find-highest-bit-set
  { base n -- u }
  0
  n 0 do
    base i cells + @ highest-bit-set
    2dup < if
      swap
    then
    drop
  loop ;

: part1
  ( base n u -- n )
  construct-numbers *
;

: seperate
  { base n u -- base1 n1 base2 n2 }
  n cells allocate throw
  0
  n cells allocate throw
  0
  n 0 do
    base i cells + @
    u 1 - bit-at if
      2swap
      2dup base i cells + @ store-at
      1+
      2swap
    else
      2dup base i cells + @ store-at
      1+
    then
  loop ;

: seperate-until-1
  ( base n u ) { xt -- n }
  begin
    over 1 > while
    3dup seperate ( base n u base1 n1 base2 n2 )
    2 pick 1 pick xt execute invert if
      2swap
    then
    drop
    free throw ( base n u new-base new-n )
    rot 1 - ( base n new-base new-n u' )
    nip''
    rot' free throw
  repeat
  2drop
  dup @
  swap free throw ;

: part2
  { base n u -- n }
  base n copy-array u ['] < seperate-until-1
  base n u ['] >= seperate-until-1
  *
;

:noname
  next-arg 2drop
  next-arg to-number
  next-arg 2 base ! read-file-into-numbers 10 base !
  2dup find-highest-bit-set
  3 pick 1 = if
    part1
  else
    part2
  then
  . CR
  bye
; IS 'cold

