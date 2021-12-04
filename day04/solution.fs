s" ../lib.fs" included

: read-board'
  { board buffer fd }
  5 0 do
    buffer dup fd read-single-line drop ( buffer length )
    BL trim-front
    4 0 do
      ( buffer length ) BL str-split ( s n rest rl )
      2swap to-number board j 5 * i + 2 * cells + !
      false board j 5 * i + 2 * 1 + cells + !
      BL trim-front
    loop
    to-number board i 5 * 4 + 2 * cells + !
    false board i 5 * 4 + 2 * 1 + cells + !
  loop
;

: read-board
  { fd -- addr-a }
  50 cells allocate throw
  max-line cells allocate throw
  2dup
  fd read-board'
  free throw
  ;

: board-won
  { addr -- f }
  5 0 do
    true
    5 0 do
      addr j 5 * i + 2 * 1 + cells + @ and
    loop
    if
      true unloop exit
    then
    true
    5 0 do
      addr i 5 * j + 2 * 1 + cells + @ and
    loop
    if
      true unloop exit
    then
  loop
  false
;

: any-board-won
  { b-buf len -- u f }
  len 0 do
    b-buf i cells + @ board-won if
      i true unloop exit
    then
  loop
  -1 false
;

: sum-not-crossed-out
  { addr -- }
  0
  25 0 do
    addr i 2 * 1 + cells + @ invert if
      addr i 2 * cells + @ +
    then
  loop
;

: cross-out
  { addr u -- }
  25 0 do
    addr i 2 * cells + @ u = if
      true addr i 2 * 1 + cells + !
    then
  loop
;

: cross-out-all
  { b-buf len u -- }
  len 0 do
    b-buf i cells + @ u cross-out
  loop
;

: call-out-number
  { n-buf nlen b-buf blen -- n n-buf' nlen' b-buf blen }
  n-buf nlen 2dup
  [char] , contains if
    [char] , str-split
  then
  2swap to-number
  b-buf blen 2 pick cross-out-all ( n-buf' nlen' n )
  -rot b-buf blen ( n n-buf' nlen' b-buf blen )
;

: part1
  0 tuck'''
  begin
    2dup any-board-won invert while
    drop
    nip'''
    call-out-number
  repeat
  nip cells + @ sum-not-crossed-out
  -rot 2drop
  * . CR
;

: remove-won-boards
  { b-buf len -- b-buf' len' }
  len cells allocate throw 0 ( b-buf' len' )
  len 0 do
    b-buf i cells + @ board-won invert if
      2dup cells +
      b-buf i cells + @ swap !
      1 +
    else
      b-buf i cells + @ free throw
    then
  loop
  b-buf free throw
;

: part2
  begin
    dup 1 > while
    call-out-number
    nip'''
    remove-won-boards
  repeat
  part1
;

: read-boards
  { ws-buffer fd -- b-buf len }
  0
  begin
    ws-buffer fd read-single-line nip while
    1+
    fd read-board
    swap
  repeat
  to-array
;

: read-input
  { fd -- n-buf len b-buf len }
  max-line cells allocate throw
  dup fd read-single-line drop
  1 cells allocate throw
  dup fd read-boards
  fd close-file throw
  rot free throw
;

: main
  next-arg to-number
  next-arg fopen
  read-input
  ( part n-buf len b-buf len )
  4 pick 1 = if
    part1
  else
    part2
  then
  drop
;

main bye
