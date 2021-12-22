needs ../lib.fs

: parse-line
  ( s n -- f xmin xmax ymin ymax zmin zmax )
  BL str-split
  2swap s" on" str= -rot
  swap 2 chars + swap 2 - \ Skip x=
  [char] . str-split
  2swap to-number -rot
  swap 1 chars + swap 1 - \ Skip .
  [char] , str-split
  2swap to-number -rot
  swap 2 chars + swap 2 - \ Skip y=
  [char] . str-split
  2swap to-number -rot
  swap 1 chars + swap 1 - \ Skip .
  [char] , str-split
  2swap to-number -rot
  swap 2 chars + swap 2 - \ Skip y=
  [char] . str-split
  2swap to-number -rot
  swap 1 chars + swap 1 - \ Skip .
  to-number
;

: handle-input
  { destination fd buf xt }
  begin
    buf buf fd read-single-line while
    parse-line
    destination xt execute
  repeat
  drop free throw
  fd close-file throw
  destination
;

: fill-cuboid
  { f xmin xmax ymin ymax zmin zmax grid }
  xmax 50 + 100 min 1 + xmin 50 + 0 max +do
    ymax 50 + 100 min 1 + ymin 50 + 0 max +do
      zmax 50 + 100 min 1 + zmin 50 + 0 max +do
        f grid k 100 * j + 100 * i + cells + !
      loop
    loop
  loop
;

: count-true
  { grid }
  0
  101 101 101 * * 0 do
    grid i cells + @ if
      1+
    then
  loop
;

: store-on-stack
  { f xmin xmax ymin ymax zmin zmax list }
  xmin list list @ 7 * 1 + cells + !
  xmax list list @ 7 * 2 + cells + !
  ymin list list @ 7 * 3 + cells + !
  ymax list list @ 7 * 4 + cells + !
  zmin list list @ 7 * 5 + cells + !
  zmax list list @ 7 * 6 + cells + !
  f    list list @ 7 * 7 + cells + !
  1 list +!
;

:noname
  next-arg 2drop
  next-arg to-number 1 = if
    101 101 101 * * cells allocate throw
    101 101 101 * * 0 do
      false over i cells + !
    loop
    next-arg fopen
    max-line chars allocate throw
    ['] fill-cuboid handle-input
    count-true
  else
    500 7 * cells allocate throw
    0 over !
    next-arg fopen
    max-line chars allocate throw
    ['] store-on-stack handle-input
    dup @
    swap 1 cells + swap
  then
  . CR
  bye
; IS 'cold
