needs ../lib.fs

: convert-line
  { h w s n -- a h w }
  n cells allocate throw
  n 0 do
    s i chars + c@ over i cells + !
  loop
  h 1 + n
;

: read-grid
  { fd buf }
  0 0
  begin
    buf buf fd read-single-line while
    convert-line
  repeat
  drop free throw
  >r to-array r>
;

: step-horizontal
  { grid h w -- grid' h w f }
  false
  h cells allocate throw
  h 0 do
    w cells allocate throw over i cells + !
    w 0 do
      grid j cells + @ i cells + @ over j cells + @ i cells + !
    loop
    w 0 do
      grid j cells + @ i cells + @ [char] > = grid j cells + @ i 1+ w mod cells + @ [char] . = and if
        swap true or swap
        [char] . over j cells + @ i cells + !
        [char] > over j cells + @ i 1 + w mod cells + !
      then
    loop
    grid i cells + @ free throw
  loop
  grid free throw
  swap
  h swap
  w swap
;

: step-vertical
  { grid h w -- grid' h w f }
  h cells allocate throw
  h 0 do
    w cells allocate throw over i cells + !
    w 0 do
      grid j cells + @ i cells + @ over j cells + @ i cells + !
    loop
  loop
  false swap
  h 0 do
    w 0 do
      grid j cells + @ i cells + @ [char] v = grid j 1 + h mod cells + @ i cells + @ [char] . = and if
        swap true or swap
        [char] . over j cells + @ i cells + !
        [char] v over j 1 + h mod cells + @ i cells + !
      then
    loop
  loop
  h 0 do
    grid i cells + @ free throw
  loop
  grid free throw
  swap
  h swap
  w swap
;

: do-step
  ( grid h w -- grid' h w f )
  step-horizontal >r
  step-vertical r> or
;

: draw-grid
  { grid h w }
  h 0 do
    w 0 do
      grid j cells + @ i cells + @ emit
    loop
    CR
  loop
  grid h w
;

: count-steps-until-stopped
  0 true
  begin while
    >r
    do-step
    r> 1+ swap
  repeat
;

:noname
  next-arg 2drop
  next-arg to-number drop
  next-arg fopen
  max-line chars allocate throw
  read-grid
  count-steps-until-stopped
  . CR
  drop
  0 do
    dup i cells + @ free throw
  loop
  free throw
  bye
; is 'cold
