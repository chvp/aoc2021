needs ../lib.fs

: read-positions
  { fd buf }
  buf buf fd read-single-line invert throw
  28 - swap 28 chars + swap to-number
  buf buf fd read-single-line invert throw
  28 - swap 28 chars + swap to-number
  buf free throw
  fd close-file throw
;

: mod1
  { num limit }
  num
  begin
    dup limit > while
    limit -
  repeat
;

: simulate-game
  { p1 p2 }
  p1 0
  p2 0
  0 >r
  begin
    dup 1000 < while
    2swap
    0 r> 1+ dup -rot 100 mod1 + swap
    1+ dup -rot 100 mod1 + swap
    1+ dup -rot 100 mod1 + swap
    >r
    rot + 10 mod1
    dup -rot +
  repeat
  swap drop
  rot drop
  swap
  r>
  * swap drop
;

: get-p1-addr
  { mem p1 p2 s1 s2 }
  p1 1 - 10 *
  p2 1 - + 21 *
  s1 + 21 *
  s2 + 2 *
  cells mem +
;

: get-p2-addr
  ( mem p1 p2 s1 s2 )
  get-p1-addr cell+
;

: get-wins-at
  { mem p1 s1 p2 s2 }
  s2 21 >= if
    0 1 exit
  then
  mem p1 p2 s1 s2 get-p1-addr @ -1 = if
    0 0
    4 1 do
      4 1 do
        4 1 do
          mem p2 s2 p1 i j k + + + 10 mod1 s1 p1 i j k + + + 10 mod1 + recurse
          rot + -rot + swap
        loop
      loop
    loop
    mem p1 p2 s1 s2 get-p1-addr !
    mem p1 p2 s1 s2 get-p2-addr !
  then
  mem p1 p2 s1 s2 get-p1-addr @
  mem p1 p2 s1 s2 get-p2-addr @
;

: simulate-universes
  { p1 p2 }
  10 10 * 21 * 21 * 2 * cells allocate throw
  dup
  10 10 * 21 * 21 * 2 * 0 do
    -1 over i cells + !
  loop
  p1 0 p2 0 get-wins-at
  max
  swap free throw
;

:noname
  next-arg 2drop
  next-arg to-number 1 = if
    ['] simulate-game
  else
    ['] simulate-universes
  then
  next-arg fopen
  max-line chars allocate throw
  read-positions
  2 pick execute
  . CR
  drop
  bye
; IS 'cold
