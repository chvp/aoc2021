needs ../lib.fs

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

:noname
  next-arg 2drop
  next-arg to-number
  1 = if 80 else 256 then
  max-line chars allocate throw dup dup
  next-arg fopen
  read-single-line drop
  comma-line-to-numbers
  count-into-age-array
  swap free throw
  swap 0 do
    dup simulate-step
  loop
  sum-buf
  . CR
  bye
; IS 'cold
