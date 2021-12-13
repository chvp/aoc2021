needs ../lib.fs

: read-coordinates
  { fd buf }
  0
  begin
    buf buf fd read-single-line invert throw dup while
    [char] , str-split
    to-number -rot to-number
    swap
    rot
    2 +
  repeat
  2drop
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

: construct-paper
  { buf len xm ym }
  ym 1 + cells allocate throw
  ym 1 + 0 do
    dup
    xm 1 + cells allocate throw
    swap
    i cells + !
  loop
  len 2 / 0 do
    dup
    1 swap
    buf i 2 * 1 + cells + @ cells + @
    buf i 2 * cells + @ cells + !
  loop
  xm 1 + ym 1 +
  buf free throw
;

: horizontal-fold
  { l paper xm ym }
  ym 0 do
    paper i cells + @
    xm l 1 + do
      dup dup i cells + @
      swap l i l - - cells + +!
    loop
    drop
  loop
  paper l ym
;

: vertical-fold
  { l paper xm ym }
  xm 0 do
    ym l 1 + do
      paper i cells + @ j cells + @
      paper l i l - - cells + @ j cells + +!
    loop
  loop
  
  paper xm l
;

: do-fold
  { paper xm ym s n }
  s n BL str-split 2nip
  BL str-split 2nip
  [char] = str-split
  to-number
  nip
  swap c@
  [char] x = if
    paper xm ym horizontal-fold
  else
    paper xm ym vertical-fold
  then
;

: fold-part1
  { fd buf paper xm ym }
  paper xm ym
  buf buf fd read-single-line invert throw
  do-fold
  buf free throw
  fd close-file throw
;

: count-dots
  { paper xm ym }
  0
  ym 0 do
    paper i cells + @
    xm 0 do
      dup i cells + @ 0 > if
        swap 1+ swap
      then
    loop
    drop
  loop
;

: fold-part2
  { fd buf paper xm ym }
  paper xm ym
  begin
    buf buf fd read-single-line while
    do-fold
  repeat
  drop free throw
  fd close-file throw
;

: print-paper
  { paper xm ym }
  xm chars allocate throw
  ym 0 do
    paper i cells + @
    xm 0 do
      2dup
      i cells + @
      0 > if
        [char] #
      else
        BL
      then
      swap i chars + c!
    loop
    drop
    dup
    xm type CR
  loop
;

:noname
  next-arg 2drop
  next-arg to-number
  1 = if
    next-arg fopen
    max-line chars allocate throw
    2dup
    read-coordinates
    2dup
    2max
    construct-paper
    fold-part1
    count-dots
    . CR
  else
    next-arg fopen
    max-line chars allocate throw
    2dup
    read-coordinates
    2dup
    2max
    construct-paper
    fold-part2
    print-paper
  then
  bye
; IS 'cold
