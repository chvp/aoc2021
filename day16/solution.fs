needs ../lib.fs

: nop ;
: zero [char] 0 ;
: one [char] 1 ;

: hex0 zero zero zero zero ;
: hex1 zero zero zero one ;
: hex2 zero zero one zero ;
: hex3 zero zero one one ;
: hex4 zero one zero zero ;
: hex5 zero one zero one ;
: hex6 zero one one zero ;
: hex7 zero one one one ;
: hex8 one zero zero zero ;
: hex9 one zero zero one ;
: hexA one zero one zero ;
: hexB one zero one one ;
: hexC one one zero zero ;
: hexD one one zero one ;
: hexE one one one zero ;
: hexF one one one one ;

: hex2binary
  { buf n -- buf' n' }
  n 4 * chars allocate throw
  0
  n 0 do
    ['] hex0
    s" 0" ['] hex0
    s" 1" ['] hex1
    s" 2" ['] hex2
    s" 3" ['] hex3
    s" 4" ['] hex4
    s" 5" ['] hex5
    s" 6" ['] hex6
    s" 7" ['] hex7
    s" 8" ['] hex8
    s" 9" ['] hex9
    s" A" ['] hexA
    s" B" ['] hexB
    s" C" ['] hexC
    s" D" ['] hexD
    s" E" ['] hexE
    s" F" ['] hexF
    16 buf i chars + 1 switch
    5 pick 5 pick 3 + chars + c!
    4 pick 4 pick 2 + chars + c!
    3 pick 3 pick 1 + chars + c!
    2 pick 2 pick chars + c!
    4 +
  loop
;

: parse-literal
  { version typ buf -- packet buf' }
  0
  begin
    dup 5 * chars buf + c@ [char] 1 = while
    1+
  repeat
  1+
  dup 4 * chars allocate throw ( len nbuf )
  over 0 do
    buf i 5 * 1 + chars + c@ over i 4 * chars + c!
    buf i 5 * 2 + chars + c@ over i 4 * 1 + chars + c!
    buf i 5 * 3 + chars + c@ over i 4 * 2 + chars + c!
    buf i 5 * 4 + chars + c@ over i 4 * 3 + chars + c!
  loop
  over 4 * dup' to-number swap free throw ( len n )
  3 cells allocate throw ( len n packet )
  version over ! ( len n packet )
  typ over cell+ ! ( len n packet )
  swap over 2 cells + ! ( len packet )
  swap 5 * chars buf + ( packet buf )
;

: create-operator
  ( ...ps ) { buf version typ count -- packet buf }
  count 3 + cells allocate throw
  version over !
  typ over cell+ !
  count over 2 cells + !
  count 0 do
    ( ... ps p packet )
    swap ( ... ps packet p )
    over ( ... ps packet p packet )
    3 count + cells + i cells - 1 cells - ! ( ... ps packet )
  loop
  buf
;

defer parse-packet

: parse-operator-length
  { version typ length buf -- packet buf' }
  0 >r
  buf
  begin
    dup buf length chars + < while
    parse-packet
    r> 1+ >r
  repeat
  version typ r> create-operator
;

: parse-operator-count
  { version typ count buf -- packet buf' }
  buf
  count 0 do
    parse-packet
  loop
  version typ count create-operator
;

: sum-versions-recursive
  { packet -- sum }
  packet @
  packet cell+ @ 4 = invert if
    packet 2 cells + @ 0 do
      packet 3 i + cells + @ recurse +
    loop
  then
  packet free throw
;

: recursive-packet-value
  { packet }

  packet cell+ @ 0 = if
    0
    packet 2 cells + @ 0 do
      packet 3 i + cells + @ recurse +
    loop
  then

  packet cell+ @ 1 = if
    1
    packet 2 cells + @ 0 do
      packet 3 i + cells + @ recurse *
    loop
  then

  packet cell+ @ 2 = if
    -1
    packet 2 cells + @ 0 do
      packet 3 i + cells + @ recurse umin
    loop
  then

  packet cell+ @ 3 = if
    0
    packet 2 cells + @ 0 do
      packet 3 i + cells + @ recurse max
    loop
  then

  packet cell+ @ 4 = if
    packet 2 cells + @
  then

  packet cell+ @ 5 = if
    packet 3 cells + @ recurse
    packet 4 cells + @ recurse
    > if 1 else 0 then
  then

  packet cell+ @ 6 = if
    packet 3 cells + @ recurse
    packet 4 cells + @ recurse
    < if 1 else 0 then
  then

  packet cell+ @ 7 = if
    packet 3 cells + @ recurse
    packet 4 cells + @ recurse
    = if 1 else 0 then
  then
  
  packet free throw
;

:noname
  { buf -- packet buf' }
  buf 3 to-number
  buf 3 chars + 3 to-number
  dup 4 = if
    buf 6 chars + parse-literal
  else
    buf 6 chars + c@ [char] 0 = if
      buf 7 chars + 15 to-number buf 22 chars + parse-operator-length
    else
      buf 7 chars + 11 to-number buf 18 chars + parse-operator-count
    then
  then
; IS parse-packet

:noname
  next-arg 2drop
  next-arg to-number 1 = if
    ['] sum-versions-recursive
  else
    ['] recursive-packet-value
  then
  max-line chars allocate throw
  next-arg fopen
  2dup dup'
  read-single-line invert throw
  rot close-file throw
  hex2binary
  rot free throw
  drop
  dup
  ['] parse-packet 2 base-execute drop
  swap free throw
  swap execute
  . CR
  bye
; IS 'cold
