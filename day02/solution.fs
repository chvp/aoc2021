needs ../lib.fs

: looped-part'
  { xt fd-in buffer -- }
  begin
    buffer fd-in read-single-line
  while
    buffer swap ( saddr n )
    bl str-split ( saddr1 n saddr2 n )
    to-number ( saddr1 n u )
    -rot ( u saddr1 n )
    xt execute
  repeat
  ( read-length ) drop
  buffer free throw
  fd-in close-file throw ;

: looped-part
  ( xt -- )
  next-arg fopen
  max-line chars allocate throw
  looped-part' ;

: part1-forward
  { depth pos x -- depth' pos' }
  depth pos x + ;

: part1-up
  { depth pos x -- depth' pos' }
  depth x negate + pos ;

: part1-down
  { depth pos x -- depth' pos' }
  depth x + pos ;

: part1 ( depth pos x ) { c-addr n -- depth' pos' }
        ['] drop \ Ignore unknown strings
        s" forward" ['] part1-forward
        s" up"      ['] part1-up
        s" down"    ['] part1-down
        3 c-addr n switch ;

: part2-forward
  { aim depth pos x -- aim' depth' pos' }
  aim depth aim x * + pos x + ;

: part2-up
  { aim depth pos x -- aim' depth' pos' }
  aim x negate + depth pos ;

: part2-down
  { aim depth pos x -- aim' depth' pos' }
  aim x + depth pos ;

: part2
  ( aim depth pos x ) { c-addr n -- aim' depth' pos' }
  ['] drop \ Ignore unknown strings
  s" forward" ['] part2-forward
  s" up"      ['] part2-up
  s" down"    ['] part2-down
  3 c-addr n switch ;

:noname
  next-arg 2drop
  next-arg to-number
  1 = if
    0 0 ['] part1 looped-part * . CR
  else
    0 0 0 ['] part2 looped-part * . CR drop
  then
  bye
; IS 'cold
