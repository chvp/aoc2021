needs ../lib.fs

: read-ranges
  { fd buf -- xmin xmax ymin ymax }
  buf buf fd read-single-line invert throw
  swap 15 + swap 15 -
  [char] . str-split
  2swap to-number -rot
  swap 1 + swap 1 -
  [char] , str-split
  2swap to-number -rot
  swap 3 + swap 3 -
  [char] . str-split
  2swap to-number -rot
  swap 1 + swap 1 -
  to-number
  buf free throw
  fd close-file throw
;

: partial-y-sum
  { vel time }
  time time * negate
  time
  +
  2/
  time vel *
  +
;

: partial-x-sum
  { vel time }
  vel vel time min partial-y-sum
;

: hits-yrange
  { vel ymin ymax }
  vel 2 * 2 +
  begin
    vel over partial-y-sum ymin >= while
    vel over partial-y-sum ymax <= if
      drop true exit
    then
    1 +
  repeat
  drop false
;

: hits-xrange
  { vel xmin xmax }
  xmin vel /
  begin
    vel over partial-x-sum xmax <= over vel <= and while
    vel over partial-x-sum xmin >= if
      drop true exit
    then
    1 +
  repeat
  drop false
;

: find-highest-yvel
  { xmin xmax ymin ymax }
  ymin 1+ negate
;

: find-lowest-xvel
  { xmin xmax ymin ymax }
  1
  begin
    dup xmin xmax hits-xrange invert while
    1 +
  repeat
;

: calculate-highest-y
  find-highest-yvel
  dup partial-y-sum
;

: hits-range
  { xvel yvel xmin xmax ymin ymax }
  yvel 2 * 2 +
  xmin xvel /
  1
  max max >r
  begin
    xvel r@ partial-x-sum xmax <= yvel r@ partial-y-sum ymin >= and while
    xvel r@ partial-x-sum xmin >= yvel r@ partial-y-sum ymax <= and if
      r> drop true exit
    then
    r> 1 + >r
  repeat
  r> drop
  false
;
    
: count-velocities
  { xmin xmax ymin ymax }
  xmin xmax ymin ymax find-highest-yvel 1 +
  0
  xmax 1+ xmin xmax ymin ymax find-lowest-xvel do
    over ymin do
      j i xmin xmax ymin ymax hits-range if
        1+
      then
    loop
  loop
  nip
;

:noname
  next-arg 2drop
  next-arg to-number 1 = if
    ['] calculate-highest-y
  else
    ['] count-velocities
  then
  next-arg fopen
  max-line chars allocate throw
  read-ranges
  4 pick execute
  . CR
  drop
  bye
; IS 'cold
