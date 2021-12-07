needs ../lib.fs

: count-increasers
  { base length }
  0
  base @
  0
  begin
    1+
    dup length < while
    dup cells base + @ ( count prev i cur )
    rot ( count i cur prev )
    dup' ( count i cur cur prev )
    > if
      ( count i cur )
      rot 1+ -rot
    then
    swap
  repeat
  2drop ;
: count-sliding-increasers
  { base length }
  0
  base @ base cell+ @ + base cell+ cell+ @ +
  0
  begin
    dup length 3 - < while
    ( count prev i )
    2dup dup cells base + @ swap ( count prev i prev arr[i] i )
    3 + cells base + @ ( count prev i prev arr[i] arr[i+3] )
    rot + swap - ( count prev i cur )
    rot dup' ( count i cur cur prev )
    > if
      ( count i cur )
      rot 1+ -rot
    then
    swap
    1+
  repeat
  2drop ;

:noname
  next-arg 2drop
  next-arg to-number
  next-arg read-file-into-numbers

  rot 1 = if
    count-increasers
  else
    count-sliding-increasers
  then
  . CR
  bye
; IS 'cold
