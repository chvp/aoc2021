needs ../lib.fs

: get-at
  { a-addr x y -- n }
  a-addr x cells + @ y chars + c@
;

: contains-coordinates
  { a-addr n x y }
  n 0 > if
    n 0 do
      a-addr i 2 * chars + c@ x = a-addr i 2 * 1 + chars + c@ y = and if
        true unloop exit
      then
    loop
  then
  false
;

: try-add-coord
  { a-addr b-addr bn j' i' j i }
  2dup j' i' contains-coordinates invert
  b-addr bn j' i' contains-coordinates invert and
  a-addr j' i' get-at a-addr j i get-at > and
  a-addr j' i' get-at 9 < and
  if
    2dup j' -rot 2 * chars + c!
    2dup i' -rot 2 * 1 + chars + c!
    1 +
  then
;

: flood-step'
  { b-addr bn q-addr qn a-addr x y j i }
  j b-addr bn 2 * chars + c!
  i b-addr bn 2 * 1 + chars + c!
  b-addr bn 1 +
  q-addr qn
  j 0 > if
    a-addr b-addr bn j 1 - i j i try-add-coord
  then
  i 0 > if
    a-addr b-addr bn j i 1 - j i try-add-coord
  then
  j x 1 - < if
    a-addr b-addr bn j 1 + i j i try-add-coord
  then
  i y 1 - < if
    a-addr b-addr bn j i 1 + j i try-add-coord
  then
;

: flood-step
  { b-addr bn q-addr qn a-addr x y }
  b-addr bn q-addr qn 1 - a-addr x y
  q-addr qn 1 - 2 * chars + c@
  q-addr qn 1 - 2 * 1 + chars + c@
  flood-step'
;

: flood-basin'
  { a-addr x y }
  begin
    dup 0 > while
    a-addr x y flood-step
  repeat
  2drop
  nip
;

: flood-basin
  { a-addr x y j i q-addr b-addr -- size }
  j q-addr c! i q-addr char+ c!
  b-addr 0 q-addr 1 a-addr x y flood-basin'
;

: get-basin-sizes
  { a-addr x y q-addr b-addr -- ... n }
  0
  x 0 do
    y 0 do
      true
      j 0 > if
        a-addr j 1 - i get-at a-addr j i get-at > and
      then
      i 0 > if
        a-addr j i 1 - get-at a-addr j i get-at > and
      then
      j x 1 - < if
        a-addr j 1 + i get-at a-addr j i get-at > and
      then
      i y 1 - < if
        a-addr j i 1 + get-at a-addr j i get-at > and
      then
      if
        1 +
        a-addr x y j i q-addr b-addr flood-basin
        swap
      then
    loop
  loop
  q-addr free throw
  b-addr free throw
;

: array-3max
  { a-addr n -- n1 n2 n3 }
  0 0 0
  n 0 do
    2 pick a-addr i cells + @ < if
      rot drop a-addr i cells + @
      1 pick 1 pick > if
        swap
      then
      2 pick 2 pick > if
        swap'
      then
    then
  loop
  a-addr free throw
;

: free-grid
  { a-addr x -- }
  x 0 do
    a-addr i cells + @ free throw
  loop
  a-addr free throw
;

: multiply-three-largest-basins
  { a-addr x y -- n }
  a-addr x y
  x y * 2 * chars allocate throw
  x y * 2 * chars allocate throw
  get-basin-sizes to-array
  array-3max * *
  a-addr x free-grid
;

: sum-local-minima+1
  { a-addr x y -- n }
  0
  x 0 do
    y 0 do
      true
      j 0 > if
        a-addr j 1 - i get-at a-addr j i get-at > and
      then
      i 0 > if
        a-addr j i 1 - get-at a-addr j i get-at > and
      then
      j x 1 - < if
        a-addr j 1 + i get-at a-addr j i get-at > and
      then
      i y 1 - < if
        a-addr j i 1 + get-at a-addr j i get-at > and
      then
      if
        a-addr j i get-at 1 + +
      then
    loop
  loop
  a-addr x free-grid
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

:noname
  next-arg 2drop
  next-arg to-number
  1 = if
    ['] sum-local-minima+1
  else
    ['] multiply-three-largest-basins
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
