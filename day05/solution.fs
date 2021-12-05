s" ../lib.fs" included

: parse-line
  { n line-buf length -- x1 y1 x2 y2 n+4 }
  line-buf length [char] , str-split
  2swap to-number -rot
  BL str-split
  2swap to-number -rot
  BL str-split
  2swap 2drop
  [char] , str-split
  2swap to-number -rot
  to-number
  n 4 +
;

: read-input
  { fd line-buf -- n-buf len }
  0 begin
    line-buf dup fd read-single-line while
    parse-line
  repeat
  drop free throw
  to-array
;

: 2max
  { buf len }
  0 0
  len 2 / 0 do
    swap dup buf i 2 * cells + @ < if
      drop buf i 2 * cells + @
    then
    swap
    dup buf i 2 * 1 + cells + @ < if
      drop buf i 2 * 1 + cells + @
    then
  loop
;

: draw-line'
  { grid xm x1 y1 x2 y2 xd yd -- }
  x1 y1
  begin
    1 pick x2 = invert 1 pick y2 = invert or while
    2dup
    xm * + cells grid + 1 swap +!
    yd + swap xd + swap
  repeat
  xm * + cells grid + 1 swap +!
;

: draw-line
  { grid xm x1 y1 x2 y2 -- }
  grid xm x1 y1 x2 y2
  x1 x2 = if
    0
  else
    x1 x2 < if
      1
    else
      -1
    then
  then
  y1 y2 = if
    0
  else
    y1 y2 < if
      1
    else
      -1
    then
  then
  draw-line'
;

: draw-line-part1
  { grid xm x1 y1 x2 y2 -- }
  x1 x2 = y1 y2 = or if
    grid xm x1 y1 x2 y2 draw-line
  then
;

: fill-grid'
  { xt grid buf len xm ym -- }
  len 4 / 0 do
    grid xm
    buf i 4 * cells + @
    buf i 4 * 1 + cells + @
    buf i 4 * 2 + cells + @
    buf i 4 * 3 + cells + @
    xt execute
  loop
;

: fill-grid
  { buf len xm ym -- buf' len' }
  xm ym * cells allocate throw dup swap'
  buf len xm ym fill-grid'
  xm ym *
;

: count>2
  { buf len -- num }
  0
  len 0 do
    buf i cells + @ 1 > if
      1+
    then
  loop
  buf free throw
;

: main
  next-arg to-number
  1 = if
    ['] draw-line-part1
  else
    ['] draw-line
  then
  next-arg fopen
  max-line cells allocate throw
  read-input
  2dup 2max
  fill-grid
  count>2
  . CR
;

main bye
