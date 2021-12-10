needs ../lib.fs

: is-opener
  { c }
  c [char] ( =
  c [char] [ =
  c [char] < =
  c [char] { =
  or or or
;

: to-opener
  { c }
  c [char] ) = if [char] ( exit then
  c [char] ] = if [char] [ exit then
  c [char] } = if [char] { exit then
  c [char] > = if [char] < exit then
  1 throw
;

: to-corrupted-score
  { c }
  c [char] ) = if 3 exit then
  c [char] ] = if 57 exit then
  c [char] } = if 1197 exit then
  c [char] > = if 25137 exit then
  1 throw
;

: to-incomplete-score
  { c }
  c [char] ( = if 1 exit then
  c [char] [ = if 2 exit then
  c [char] { = if 3 exit then
  c [char] < = if 4 exit then
  1 throw
;

: drop-until-sentinel
  begin -1 = invert while repeat
;

: incomplete-score
  { s n }
  -1 \ Sentinel value
  n 0 do
    s i chars + c@
    is-opener if
      s i chars + c@
    else
      s i chars + c@ to-opener = invert if
        drop-until-sentinel
        0 unloop exit
      then
    then
  loop
  0
  begin
    swap dup -1 = invert while
    to-incomplete-score swap 5 * +
  repeat
  drop
;

: count-incomplete-points
  { fd buf }
  0
  begin
    buf buf fd read-single-line while
    incomplete-score
    dup 0 = invert if
      swap 1+ 0
    then
    drop
  repeat
  drop free throw
  fd close-file throw
  to-array
  2dup array-sort
  dup' 2 / cells + @
  swap free throw
;

: corrupted-score
  { s n }
  -1 \ Sentinel value
  n 0 do
    s i chars + c@
    is-opener if
      s i chars + c@
    else
      s i chars + c@ to-opener = invert if
        drop-until-sentinel
        s i chars + c@ to-corrupted-score unloop exit
      then
    then
  loop
  drop-until-sentinel
  0
;

: count-corrupted-points
  { fd buf }
  0
  begin
    buf buf fd read-single-line while
    corrupted-score +
  repeat
  drop free throw
  fd close-file throw
;

:noname
  next-arg 2drop
  next-arg to-number
  1 = if
    ['] count-corrupted-points
  else
    ['] count-incomplete-points
  then
  next-arg fopen
  max-line chars allocate throw
  rot execute
  . CR
  bye
; IS 'cold
