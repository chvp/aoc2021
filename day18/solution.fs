needs ../lib.fs

: type-number
  { number }
  [char] [ emit
  number cell+ @ 0 = if
    number 3 cells + @ 1 .r
  else
    number cell+ @ recurse
  then
  [char] , emit
  number 2 cells + @ 0 = if
    number 4 cells + @ 1 .r
  else
    number 2 cells + @ recurse
  then
  [char] ] emit
;

defer read-number

: read-number-or-value
  ( arr buf' ) { offset -- arr buf' }
  dup c@ [char] [ = if
    read-number ( arr number buf' )
    -rot
    2dup ! \ Store parent pointer in child
    over 1 offset + cells + !
    swap ( arr buf' )
  else
    dup 1 to-number swap 1+ ( arr u buf' )
    -rot
    over 3 offset + cells + !
    0 over 1 offset + cells + ! \ Make sure child pointer is 0
    swap ( arr buf' )
  then
;

\ Numbers are arrays of 5 elements:
\ 1) Pointer to parent (or 0 if non-existent)
\ 2) Pointer to left-child (or 0 if non-existent, 4 should be filled in this case)
\ 3) Pointer to right-child (or 0 if non-existent, 5 should be filled in this case)
\ 4) Left value (only valid if there is no left child)
\ 5) Right value (only valid if there is no right child)
:noname
  ( buf -- number buf' )
  1+ \ skip [
  5 cells allocate throw
  0 over !
  swap
  0 read-number-or-value
  1+ \ skip ,
  1 read-number-or-value
  1+ \ skip ]
; IS read-number

: read-numbers
  { fd buf }
  0
  begin
    buf fd read-single-line while
    drop \ If all numbers are well-formed, we don't need the length
    buf read-number drop
    swap 1+
  repeat
  drop
  buf free throw
  fd close-file throw
  to-array
;

: bubble-down-right
  { number val -- }
  number 2 cells + @ 0 = if
    val number 4 cells + +!
  else
    number 2 cells + @ val recurse
  then
;

: bubble-up-left
  { number val -- }
  number @ 0 = if
    exit
  then
  number @ cell+ @ number = if
    number @ val recurse
  else
    number @ cell+ @ 0 = if
      val number @ 3 cells + +!
    else
      number @ cell+ @ val bubble-down-right
    then
  then
;

: bubble-down-left
  { number val -- }
  number cell+ @ 0 = if
    val number 3 cells + +!
  else
    number cell+ @ val recurse
  then
;

: bubble-up-right
  { number val -- }
  number @ 0 = if
    exit
  then
  number @ 2 cells + @ number = if
    number @ val recurse
  else
    number @ 2 cells + @ 0 = if
      val number @ 4 cells + +!
    else
      number @ 2 cells + @ val bubble-down-left
    then
  then
;

: do-explode
  { number }
  number number 3 cells + @ bubble-up-left
  number number 4 cells + @ bubble-up-right
;

: explode
  { number depth -- f }
  number cell+ @ 0 = invert if
    number cell+ @ depth 1 + recurse if
      true
    else
      number 2 cells + @ 0 = invert if
        number 2 cells + @ depth 1 + recurse
      else
        false
      then
    then
  else
    number 2 cells + @ 0 = invert if
      number 2 cells + @ depth 1 + recurse
    else
      depth 4 = if
        number do-explode
        number @ cell+ @ number = if
          0 number @ cell+ !
          0 number @ 3 cells + !
        else
          0 number @ 2 cells + !
          0 number @ 4 cells + !
        then
        number free throw
        true
      else
        false
      then
    then
  then
;

: do-split
  { number offset -- }
  5 cells allocate throw
  dup number 1 offset + cells + !
  number over !
  0 over cell+ !
  0 over 2 cells + !
  number 3 offset + cells + @ 2/ over 3 cells + !
  number 3 offset + cells + @ 1+ 2/ over 4 cells + !
  drop
;

: split
  { number }
  number cell+ @ 0 = invert if
    number cell+ @ recurse if
      true
    else
      number 2 cells + @ 0 = invert if
        number 2 cells + @ recurse
      else
        number 4 cells + @ 9 > if
          number 1 do-split
          true
        else
          false
        then
      then
    then
  else
    number 3 cells + @ 9 > if
      number 0 do-split
      true
    else
      number 2 cells + @ 0 = invert if
        number 2 cells + @ recurse
      else
        number 4 cells + @ 9 > if
          number 1 do-split
          true
        else
          false
        then
      then
    then
  then
;

: reduce
  { number }
  true
  begin
  while
    number 0 explode if
      true
    else
      number split if
        true
      else
        false
      then
    then
  repeat
;

: add
  { n1 n2 -- n3 }
  5 cells allocate throw
  0 over !
  dup n1 !
  dup n2 !
  n1 over cell+ !
  n2 over 2 cells + !
  dup reduce
;

: magnitude
  { number }
  number cell+ @ 0 = if
    number 3 cells + @
  else
    number cell+ @ recurse
  then
  3 *
  number 2 cells + @ 0 = if
    number 4 cells + @
  else
    number 2 cells + @ recurse
  then
  number free throw
  2 *
  +
;

: add-all-find-magnitude
  { arr len -- u }
  arr @
  len 1 do
    arr i cells + @
    add
  loop
  arr free throw
  magnitude
;

: copy-number
  { number }
  5 cells allocate throw
  0 over !
  number cell+ @ 0 = if
    0 over cell+ !
    number 3 cells + @ over 3 cells + !
  else
    number cell+ @ recurse over cell+ !
    dup dup cell+ @ !
  then
  number 2 cells + @ 0 = if
    0 over 2 cells + !
    number 4 cells + @ over 4 cells + !
  else
    number 2 cells + @ recurse over 2 cells + !
    dup dup 2 cells + @ !
  then
;

: find-largest-magnitude
  { arr len -- u }
  0
  len 0 do
    len 0 do
      j i = invert if
        arr j cells + @ copy-number
        arr i cells + @ copy-number
        add magnitude
        max
      then
    loop
  loop
;

:noname
  next-arg 2drop
  next-arg to-number 1 = if
    ['] add-all-find-magnitude
  else
    ['] find-largest-magnitude
  then
  next-arg fopen
  max-line chars allocate throw
  read-numbers
  2 pick execute
  . CR
  drop
  bye
; IS 'cold
