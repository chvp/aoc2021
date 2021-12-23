needs ../lib.fs
needs ../heap.fs

: read-letter
  { state mul x y }
  state mul 8 * 1 + cells + @ 0 = if
    x state mul 8 * 1 + cells + !
    y state mul 8 * 2 + cells + !
  else
  state mul 8 * 3 + cells + @ 0 = if
    x state mul 8 * 3 + cells + !
    y state mul 8 * 4 + cells + !
  else
  state mul 8 * 5 + cells + @ 0 = if
    x state mul 8 * 5 + cells + !
    y state mul 8 * 6 + cells + !
  else
    x state mul 8 * 7 + cells + !
    y state mul 8 * 8 + cells + !
  then
  then
  then
  state
;

: read-initial-state-part1
  { fd buf }
  33 cells allocate throw
  33 0 do
    0 over i cells + !
  loop
  0 over !
  buf fd read-single-line invert throw drop
  buf fd read-single-line invert throw drop
  buf fd read-single-line invert throw drop
  buf 3 chars + c@ [char] A - 2 1 read-letter
  buf 5 chars + c@ [char] A - 4 1 read-letter
  buf 7 chars + c@ [char] A - 6 1 read-letter
  buf 9 chars + c@ [char] A - 8 1 read-letter
  buf fd read-single-line invert throw drop
  buf 3 chars + c@ [char] A - 2 2 read-letter
  buf 5 chars + c@ [char] A - 4 2 read-letter
  buf 7 chars + c@ [char] A - 6 2 read-letter
  buf 9 chars + c@ [char] A - 8 2 read-letter
  0 2 3 read-letter
  1 4 3 read-letter
  2 6 3 read-letter
  3 8 3 read-letter
  0 2 4 read-letter
  1 4 4 read-letter
  2 6 4 read-letter
  3 8 4 read-letter
  buf free throw
  fd close-file throw
;

: read-initial-state-part2
  { fd buf }
  33 cells allocate throw
  33 0 do
    0 over i cells + !
  loop
  0 over !
  buf fd read-single-line invert throw drop
  buf fd read-single-line invert throw drop
  buf fd read-single-line invert throw drop
  buf 3 chars + c@ [char] A - 2 1 read-letter
  buf 5 chars + c@ [char] A - 4 1 read-letter
  buf 7 chars + c@ [char] A - 6 1 read-letter
  buf 9 chars + c@ [char] A - 8 1 read-letter
  3 2 2 read-letter
  2 4 2 read-letter
  1 6 2 read-letter
  0 8 2 read-letter
  3 2 3 read-letter
  1 4 3 read-letter
  0 6 3 read-letter
  2 8 3 read-letter
  buf fd read-single-line invert throw drop
  buf 3 chars + c@ [char] A - 2 4 read-letter
  buf 5 chars + c@ [char] A - 4 4 read-letter
  buf 7 chars + c@ [char] A - 6 4 read-letter
  buf 9 chars + c@ [char] A - 8 4 read-letter
  buf free throw
  fd close-file throw
;

: copy-state
  { extracost old }
  33 cells allocate throw
  33 0 do
    old i cells + @ over i cells + !
  loop
  extracost over +!
;

: type-state
  { state }
  33 0 do
    state i cells + @ .
  loop
  CR
;

: xpos
  { i state }
  state i 2 * 1 + cells + @
;

: ypos
  { i state }
  state i 2 * 2 + cells + @
;

: get-cost
  { state }
  state @
;

: destination
  { num }
  num 4 / 1+ 2 *
;

: others
  { num }
  num 4 / 4 * num = invert if num 4 / 4 * then
  num 4 / 4 * 1 + num = invert if num 4 / 4 * 1 + then
  num 4 / 4 * 2 + num = invert if num 4 / 4 * 2 + then
  num 4 / 4 * 3 + num = invert if num 4 / 4 * 3 + then
;

: is-finished
  { state }
  state 1 cells + @ 2 =
  state 3 cells + @ 2 = and
  state 5 cells + @ 2 = and
  state 7 cells + @ 2 = and
  state 9 cells + @ 4 = and
  state 11 cells + @ 4 = and
  state 13 cells + @ 4 = and
  state 15 cells + @ 4 = and
  state 17 cells + @ 6 = and
  state 19 cells + @ 6 = and
  state 21 cells + @ 6 = and
  state 23 cells + @ 6 = and
  state 25 cells + @ 8 = and
  state 27 cells + @ 8 = and
  state 29 cells + @ 8 = and
  state 31 cells + @ 8 = and
;

: between
  { val a b }
  val a >= val b <= and
  val b >= val a <= and
  or
;

: try-up-to
  { stepcost num state heap dest }
  16 0 do
    i state ypos 0 =
    i state xpos dest num state xpos between
    and if unloop exit then
  loop
  stepcost
  stepcost num state xpos dest - abs * +
  state copy-state
  dest over num 2 * 1 + cells + !
  0 over num 2 * 2 + cells + !
  heap heap-add
;

: try-move
  { stepcost num state heap }
  num state ypos 0 = if
    16 0 do
      i num = invert
      i state ypos 0 = i state xpos num destination num state xpos between and
      and if unloop exit then
      i 4 / num 4 / = invert
      i state xpos num destination =
      and if unloop exit then
    loop
    num others state xpos num destination = if 1 else 0 then
    -rot state xpos num destination = if 1 else 0 then
    swap state xpos num destination = if 1 else 0 then + +
    dup 3 = if
      stepcost
      stepcost num state xpos num destination - abs * +
      state copy-state
      num destination over num 2 * 1 + cells + !
      1 over num 2 * 2 + cells + !
      heap heap-add
    then
    dup 2 = if
      stepcost 2*
      stepcost num state xpos num destination - abs * +
      state copy-state
      num destination over num 2 * 1 + cells + !
      2 over num 2 * 2 + cells + !
      heap heap-add
    then
    dup 1 = if
      stepcost 3 *
      stepcost num state xpos num destination - abs * +
      state copy-state
      num destination over num 2 * 1 + cells + !
      3 over num 2 * 2 + cells + !
      heap heap-add
    then
    0 = if
      stepcost 4 *
      stepcost num state xpos num destination - abs * +
      state copy-state
      num destination over num 2 * 1 + cells + !
      4 over num 2 * 2 + cells + !
      heap heap-add
    then
    exit
  then
  num state ypos 1 = if
    num state xpos num destination = if
      num others state xpos num destination = if 1 else 0 then
      -rot state xpos num destination = if 1 else 0 then
      swap state xpos num destination = if 1 else 0 then + +
      3 = if exit then
    then
    11 0 do
      i 2 = invert i 4 = invert i 6 = invert i 8 = invert and and and if
        stepcost num state heap i try-up-to
      then
    loop
    exit
  then
  num state ypos 2 = if
    num state xpos num destination = if
      num others state xpos num destination = if 1 else 0 then
      -rot state xpos num destination = if 1 else 0 then
      swap state xpos num destination = if 1 else 0 then + +
      2 = if exit then
    then
    16 0 do
      i state xpos num state xpos = i state ypos num state ypos < and if
        unloop exit
      then
    loop
    stepcost state copy-state
    11 0 do
      i 2 = invert i 4 = invert i 6 = invert i 8 = invert and and and if
        stepcost num 2 pick heap i try-up-to
      then
    loop
    free throw
    exit
  then
  num state ypos 3 = if
    num state xpos num destination = if
      num others state xpos num destination = if 1 else 0 then
      -rot state xpos num destination = if 1 else 0 then
      swap state xpos num destination = if 1 else 0 then + +
      1 = if exit then
    then
    16 0 do
      i state xpos num state xpos = i state ypos num state ypos < and if
        unloop exit
      then
    loop
    stepcost 2 * state copy-state
    11 0 do
      i 2 = invert i 4 = invert i 6 = invert i 8 = invert and and and if
        stepcost num 2 pick heap i try-up-to
      then
    loop
    free throw
    exit
  then
  num state ypos 4 = if
    num state xpos num destination = if exit then
    16 0 do
      i state xpos num state xpos = i state ypos num state ypos < and if
        unloop exit
      then
    loop
    stepcost 3 * state copy-state
    11 0 do
      i 2 = invert i 4 = invert i 6 = invert i 8 = invert and and and if
        stepcost num 2 pick heap i try-up-to
      then
    loop
    free throw
    exit
  then
;

: add-possible-moves
  { heap state }
  state type-state
  16 0 do
    10 i 4 / pow i state heap try-move
  loop
;

: free-state free throw ;

: find-cheapest
  { state }
  ['] get-cost heap-init >r
  state r@ heap-add
  begin
    r@ heap-pop >r r@ is-finished invert while
    2r@ add-possible-moves
    r> free throw
  repeat
  r@ get-cost
  r> free throw
  ['] free-state r> heap-free
;

:noname
  next-arg 2drop
  next-arg to-number 1 = if
    ['] read-initial-state-part1
  else
    ['] read-initial-state-part2
  then
  next-arg fopen
  max-line chars allocate throw
  rot execute
  find-cheapest
  . CR
  bye
; IS 'cold
