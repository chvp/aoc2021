needs ../lib.fs

\ Numbers are arrays of 3 elements:
\ 1) Pointer to parent (or 0 if non-existent)
\ 2) > 0: Pointer to left-child, else: -value
\ 3) > 0: Pointer to right-child, else: -value
: allocate-number 3 cells allocate throw ;
: parent @ ;
: has-parent @ 0 = invert ;
: set-parent ! ;
: left-child cell+ @ ;
: has-left-child left-child 0 > ;
: set-left-child cell+ ! ;
: left-value left-child negate ;
: set-left-value swap negate swap set-left-child ;
: right-child 2 cells + @ ;
: set-right-child 2 cells + ! ;
: has-right-child right-child 0 > ;
: right-value right-child negate ;
: set-right-value swap negate swap set-right-child ;

: type-number
  { number }
  [char] [ emit
  number has-left-child if
    number left-child recurse
  else
    number left-value 1 .r
  then
  [char] , emit
  number has-right-child if
    number right-child recurse
  else
    number right-value 1 .r
  then
  [char] ] emit
;

: copy-number
  { number }
  allocate-number
  0 over set-parent
  number has-left-child if
    number left-child recurse over set-left-child
    dup dup left-child set-parent
  else
    number left-value over set-left-value
  then
  number has-right-child if
    number right-child recurse over set-right-child
    dup dup right-child set-parent
  else
    number right-value over set-right-value
  then
;

: free-number
  { number }
  number has-left-child if
    number left-child recurse
  then
  number has-right-child if
    number right-child recurse
  then
  number free throw
;

defer 'read-number

: read-number-or-value
  ( arr buf' ) { set-child set-value -- arr buf' }
  dup c@ [char] [ = if
    'read-number ( arr number buf' )
    -rot
    2dup set-parent
    over set-child execute
    swap ( arr buf' )
  else
    dup 1 to-number swap 1+ ( arr u buf' )
    -rot
    over set-value execute
    swap ( arr buf' )
  then
;

: read-number
  ( buf -- number buf' )
  1+ \ skip [
  allocate-number
  0 over set-parent
  swap
  ['] set-left-child ['] set-left-value read-number-or-value
  1+ \ skip ,
  ['] set-right-child ['] set-right-value read-number-or-value
  1+ \ skip ]
;
' read-number IS 'read-number

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
  number has-right-child if
    number right-child val recurse
  else
    val number right-value + number set-right-value
  then
;

: bubble-up-left
  { number val -- }
  number has-parent invert if
    exit
  then
  number parent left-child number = if
    number parent val recurse
  else
    number parent has-left-child if
      number parent left-child val bubble-down-right
    else
      val number parent left-value + number parent set-left-value
    then
  then
;

: bubble-down-left
  { number val -- }
  number has-left-child if
    number left-child val recurse
  else
    val number left-value + number set-left-value
  then
;

: bubble-up-right
  { number val -- }
  number has-parent invert if
    exit
  then
  number parent right-child number = if
    number parent val recurse
  else
    number parent has-right-child if
      number parent right-child val bubble-down-left
    else
      val number parent right-value + number parent set-right-value
    then
  then
;

: do-explode
  { number }
  number number left-value bubble-up-left
  number number right-value bubble-up-right
;

: explode
  { number depth -- f }
  number has-left-child if
    number left-child depth 1 + recurse if
      true
    else
      number has-right-child if
        number right-child depth 1 + recurse
      else
        false
      then
    then
  else
    number has-right-child if
      number right-child depth 1 + recurse
    else
      depth 4 = if
        number do-explode
        number parent left-child number = if
          0 number parent set-left-value
        else
          0 number parent set-right-value
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
  { number val set-child -- }
  allocate-number
  dup number set-child execute
  number over set-parent
  val 2/ over set-left-value
  val 1+ 2/ swap set-right-value
;

: split
  { number }
  number has-left-child if
    number left-child recurse if
      true
    else
      number has-right-child if
        number right-child recurse
      else
        number right-value 9 > if
          number number right-value ['] set-right-child do-split
          true
        else
          false
        then
      then
    then
  else
    number left-value 9 > if
      number number left-value ['] set-left-child do-split
      true
    else
      number has-right-child if
        number right-child recurse
      else
        number right-value 9 > if
          number number right-value ['] set-right-child do-split
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
  allocate-number
  0 over set-parent
  dup n1 set-parent
  dup n2 set-parent
  n1 over set-left-child
  n2 over set-right-child
  dup reduce
;

: magnitude
  { number }
  number has-left-child if
    number left-child recurse
  else
    number left-value
  then
  3 *
  number has-right-child if
    number right-child recurse
  else
    number right-value
  then
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
  dup magnitude swap free-number
  arr free throw
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
  len 0 do
    arr i cells + @ free-number
  loop
  arr free throw
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
