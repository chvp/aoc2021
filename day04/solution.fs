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
  dup'
  fd read-board'
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

: part1
  0 >r
  begin
    2dup any-board-won invert while
    r>
    2drop
    2swap
    2dup [char] , contains if
      [char] , str-split
    then
    2swap to-number >r r@ ( b-buf len n-buf len u )
    2swap' 2dup' ( n-buf len b-buf len b-buf len u )
    cross-out-all
  repeat
  nip cells + @ sum-not-crossed-out
  r> * . CR
;

: remove-won-boards
  { b-buf len -- b-buf' len' }
  len cells allocate throw 0 ( b-buf' len' )
  len 0 do
    b-buf i cells + @ board-won invert if
      2dup cells +
      b-buf i cells + @ swap !
      1 +
    then
  loop
  b-buf free throw
;

: part2
  0 >r
  begin
    dup 1 > while
    r> drop
    2swap
    2dup [char] , contains if
      [char] , str-split
    then
    2swap to-number >r r@ ( b-buf len n-buf len u )
    2swap' 2dup' ( n-buf len b-buf len b-buf len u )
    cross-out-all
    remove-won-boards
  repeat
  r> drop
  part1
;

: read-boards
  { ws-buffer fd }
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
  fd read-boards
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
