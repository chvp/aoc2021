needs ../lib.fs

: get-at
  { a-addr x y -- n }
  a-addr x cells + @ y cells + @
;

: set-at
  { n a-addr x y -- }
  n a-addr x cells + @ y cells + !
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
  n cells allocate throw
  n 0 do
    buf i chars + 1 to-number over i cells + !
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

: pop-from-queue
  { queue qlen pos ym -- queue qlen' x y }
  queue pos cells + @ dup ym / swap ym mod
  pos 1 + qlen < if
    qlen pos 1 + do
      queue i cells + @ queue i 1 - cells + !
    loop
  then
  queue qlen 1 - 2swap
;

: pop-smallest
  { queue qlen dgrid ym -- queue qlen' x y }
  -2 -1
  qlen 0 do
    dup
    dgrid
    queue i cells + @ dup ym / swap ym mod
    get-at u> if
      2drop
      i
      dgrid
      queue i cells + @ dup ym / swap ym mod
      get-at
    then
  loop
  drop
  queue qlen rot ym pop-from-queue
;

: queue-contains
  { queue qlen x y ym }
  qlen 0 = if
    false exit
  then
  qlen 0 do
    queue i cells + @ x ym * y + = if
      true unloop exit
    then
  loop
  false
;

: check-unvisited
  ( queue qlen ) { dgrid x y ym -- queue qlen' }
  dgrid x y get-at -1 = if
    2dup cells + x ym * y + swap !
    1 +
  then
;

: handle-point'
  ( queue qlen ) { dgrid grid x y x' y' xm ym -- queue qlen' }
  x' 0 >= y' 0 >= x' xm < y' ym < and and and if
    dgrid x' y' ym check-unvisited
    2dup x' y' ym queue-contains if
      dgrid x y get-at grid x' y' get-at +
      dgrid x' y' get-at umin
      dgrid x' y' set-at
    then
  then
;

: handle-point
  ( queue qlen ) { x y dgrid grid xm ym -- queue qlen' }
  dgrid grid x y x 1 - y xm ym handle-point'
  dgrid grid x y x y 1 - xm ym handle-point'
  dgrid grid x y x 1 + y xm ym handle-point'
  dgrid grid x y x y 1 + xm ym handle-point'
;

: dijkstra'
  ( queue qlen ) { dgrid grid xm ym }
  begin
    dup 0 > while
    dgrid ym pop-smallest
    dgrid grid xm ym handle-point
  repeat
  drop free throw
  dgrid xm 1 - cells + @ ym 1 - cells + @
  dgrid xm free-grid
  grid xm free-grid
;

: dijkstra
  { grid xm ym xs ys }
  xm ym * cells allocate throw 0
  2dup cells + 0 swap !
  1 + ( queue qlen )
  xm cells allocate throw
  xm 0 do
    ym cells allocate throw over i cells + !
    ym 0 do
      -1 over j cells + @ i cells + !
    loop
  loop ( dgrid )
  0 over xs cells + @ ys cells + !
  grid xm ym dijkstra'
;

: grow-5
  { grid xm ym }
  xm 5 * cells allocate throw
  xm 5 * 0 do
    ym 5 * cells allocate throw over i cells + !
    ym 5 * 0 do
      grid j xm mod i ym mod get-at j xm / + i ym / + dup 9 > if
        9 -
      then
      over j i set-at
    loop
  loop
  xm 5 * ym 5 *
  grid xm free-grid
;

:noname
  next-arg 2drop
  next-arg to-number 1 = if
    next-arg fopen
    max-line chars allocate throw
    read-lines-into-arrays
    >r to-array r>
  else
    next-arg fopen
    max-line chars allocate throw
    read-lines-into-arrays
    >r to-array r>
    grow-5
  then
  0 0
  dijkstra
  . CR
  bye
; IS 'cold
