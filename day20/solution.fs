needs ../lib.fs

: read-algorithm
  { fd buf }
  buf fd read-single-line invert throw drop
  buf fd
;

: chomp-line
  { fd }
  max-line chars allocate throw
  dup fd read-single-line invert throw drop
  free throw
  fd
;

: read-image
  { fd }
  0
  0 >r
  begin
    max-line chars allocate throw
    dup fd read-single-line while
    r> max >r
    swap 1+
  repeat
  drop free throw
  to-array r>
;

: set-at
  { val im j i }
  val im j cells + @ i chars + c!
;

: get-pixel
  { im j i h w step alg }
  j 0 < i 0 < or j h >= or i w >= or if
    alg c@ [char] # = if step 2 mod else 0 then
  else
    im j cells + @ i chars + c@ [char] # = if 1 else 0 then
  then
;


: get-at
  { im j i h w step alg }
  im j 1 - i 1 - h w step alg get-pixel 256 *
  im j 1 - i h w step alg get-pixel 128 * +
  im j 1 - i 1 + h w step alg get-pixel 64 * +
  im j i 1 - h w step alg get-pixel 32 * +
  im j i h w step alg get-pixel 16 * +
  im j i 1 + h w step alg get-pixel 8 * +
  im j 1 + i 1 - h w step alg get-pixel 4 * +
  im j 1 + i h w step alg get-pixel 2 * +
  im j 1 + i 1 + h w step alg get-pixel +
  chars alg + c@
;


: do-step
  { im h w im' h' w' step alg -- im' h' w' }
  h' 0 do
    w' 0 do
      im j 1 - i 1 - h w step alg get-at im' j i set-at
    loop
  loop
  im' h' w'
;

: enhance-image
  { steps step alg im h w }
  im h w
  steps 0 do
    1 pick 2 + cells allocate throw
    2 pick 2 + 0 do
      1 pick 2 + cells allocate throw over i cells + !
    loop
    2 pick 2 +
    2 pick 2 +
    i alg do-step
  loop
;

: count-on
  { im h w }
  0
  h 0 do
    w 0 do
      im j cells + @ i chars + c@ [char] # = if
        1+
      then
    loop
  loop
;

:noname
  next-arg 2drop
  next-arg to-number 1 = if 2 else 50 then 0
  next-arg fopen
  max-line chars allocate throw
  read-algorithm
  chomp-line
  read-image
  enhance-image
  count-on
  . CR
  bye
; IS 'cold
