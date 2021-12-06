s" ../lib.fs" included

: count-into-age-array'
  ( ...u ) { n buf -- buf }
  9 0 do
    0 buf i cells + !
  loop
  n 0 do
    cells buf + 1 swap +!
  loop
  buf
;

: count-into-age-array
  ( ...u n -- buf )
  9 cells allocate throw
  count-into-age-array'
;

: simulate-step
  { buf -- }
  buf @
  9 1 do
    buf i cells + @
    buf i 1 - cells + !
  loop
  dup buf 6 cells + +!
  buf 8 cells + !
;

: sum-buf
  { buf -- n }
  0
  9 0 do
    buf i cells + @ +
  loop
  buf free throw
;

: main
  next-arg to-number
  1 = if 80 else 256 then
  max-line cells allocate throw dup dup
  next-arg fopen
  read-single-line drop
  0 -rot
  begin
    2dup [char] , contains while
    [char] , str-split
    2swap to-number ( count s n u )
    -rot 2swap swap 1 + 2swap ( u count+1 s n )
  repeat
  to-number swap 1 +
  count-into-age-array
  swap free throw
  swap 0 do
    dup simulate-step
  loop
  sum-buf
  . CR
;

main bye
