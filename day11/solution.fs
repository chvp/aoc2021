needs ../lib.fs

: get-at
  { a-addr x y -- n }
  a-addr x cells + @ y chars + c@
;

: set-at
  { n a-addr x y -- }
  n a-addr x cells + @ y chars + c!
;

: add-at
  { n a-addr x y -- }
  a-addr x y get-at
  n +
  a-addr x y set-at
;

: free-grid
  { a-addr x -- }
  x 0 do
    a-addr i cells + @ free throw
  loop
  a-addr free throw
;

: read-line-into-array
  { buf n -- a-addr n }
  n chars allocate throw
  n 0 do
    buf i chars + 1 to-number over i chars + c!
  loop
  n
;

: read-lines-into-arrays
  { fd buf }
  0 0
  begin
    buf buf fd read-single-line while
    read-line-into-array
    rot drop
    swap' swap 1+ swap
  repeat
  drop free throw
  fd close-file throw
;

: add-one
  { addr x y }
  x 0 do
    y 0 do
      1 addr j i add-at
      addr j i get-at 9 > if
        2dup j -rot 2 * chars + c!
        2dup i -rot 2 * 1 + chars + c!
        1 +
      then
    loop
  loop
;

: flash-for-coord
  ( q-buf n ) { j i addr x y }
  j -1 > i -1 > and j x < and i y < and if
    1 addr j i add-at
    addr j i get-at 10 = if
      2dup j -rot 2 * chars + c!
      2dup i -rot 2 * 1 + chars + c!
      1 +
    then
  then
;

: handle-flash
  ( q-buf n ) { j i addr x y }
  j 1 - i 1 - addr x y flash-for-coord
  j 1 - i addr x y flash-for-coord
  j 1 - i 1 + addr x y flash-for-coord
  j i 1 - addr x y flash-for-coord
  j i 1 + addr x y flash-for-coord
  j 1 + i 1 - addr x y flash-for-coord
  j 1 + i addr x y flash-for-coord
  j 1 + i 1 + addr x y flash-for-coord
;

: handle-flashes
  ( q-buf n ) { addr x y }
  0
  begin
    2dup > while
    swap >r
    2dup 2 * 1 + chars + c@ >r
    2dup 2 * chars + c@ r>
    rot r> swap >r -rot
    addr x y handle-flash
    r> 1+
  repeat
  drop
;

: reset-flashed
  { addr x y }
  x 0 do
    y 0 do
      addr j i get-at 9 > if
        0 addr j i set-at
      then
    loop
  loop
;

: simulate-step
  { q-buf addr x y }
  q-buf 0 addr x y add-one
  addr x y handle-flashes
  addr x y reset-flashed
  nip
;

: simulate-octopi-100-steps
  { addr x y }
  0
  x y * 2 * chars allocate throw
  100 0 do
    dup addr x y simulate-step swap' + swap
  loop
  free throw
  addr x free-grid
;

: simulate-octopi-until-all-flashed
  { addr x y }
  1
  x y * 2 * chars allocate throw
  begin
    dup addr x y simulate-step x y * < while
    swap 1 + swap
  repeat
  free throw
  addr x free-grid
;

:noname
  next-arg 2drop
  next-arg to-number
  1 = if
    ['] simulate-octopi-100-steps
  else
    ['] simulate-octopi-until-all-flashed
  then
  next-arg fopen
  max-line chars allocate throw
  read-lines-into-arrays
  >r to-array r>
  3 pick execute
  . CR
  drop
  bye
; IS 'cold
