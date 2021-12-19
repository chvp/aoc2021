needs ../lib.fs

create rotation-matrices
  1 , 0 , 0 , 0 , 1 , 0 , 0 , 0 , 1 ,
  0 , -1 , 0 , 1 , 0 , 0 , 0 , 0 , 1 ,
  -1 , 0 , 0 , 0 , -1 , 0 , 0 , 0 , 1 ,
  0 , 1 , 0 , -1 , 0 , 0 , 0 , 0 , 1 ,
  0 , 0 , 1 , 0 , 1 , 0 , -1 , 0 , 0 ,
  0 , -1 , 0 , 0 , 0 , 1 , -1 , 0 , 0 ,
  0 , 0 , -1 , 0 , -1 , 0 , -1 , 0 , 0 ,
  0 , 1 , 0 , 0 , 0 , -1 , -1 , 0 , 0 ,
  -1 , 0 , 0 , 0 , 1 , 0 , 0 , 0 , -1 ,
  0 , -1 , 0 , -1 , 0 , 0 , 0 , 0 , -1 ,
  1 , 0 , 0 , 0 , -1 , 0 , 0 , 0 , -1 ,
  0 , 1 , 0 , 1 , 0 , 0 , 0 , 0 , -1 ,
  0 , 0 , -1 , 0 , 1 , 0 , 1 , 0 , 0 ,
  0 , -1 , 0 , 0 , 0 , -1 , 1 , 0 , 0 ,
  0 , 0 , 1 , 0 , -1 , 0 , 1 , 0 , 0 ,
  0 , 1 , 0 , 0 , 0 , 1 , 1 , 0 , 0 ,
  1 , 0 , 0 , 0 , 0 , -1 , 0 , 1 , 0 ,
  0 , 0 , 1 , 1 , 0 , 0 , 0 , 1 , 0 ,
  -1 , 0 , 0 , 0 , 0 , 1 , 0 , 1 , 0 ,
  0 , 0 , -1 , -1 , 0 , 0 , 0 , 1 , 0 ,
  -1 , 0 , 0 , 0 , 0 , -1 , 0 , -1 , 0 ,
  0 , 0 , 1 , -1 , 0 , 0 , 0 , -1 , 0 ,
  1 , 0 , 0 , 0 , 0 , 1 , 0 , -1 , 0 ,
  0 , 0 , -1 , 1 , 0 , 0 , 0 , -1 , 0 ,

: read-scanner
  { fd buf -- beacons n }
  0
  begin
    buf buf fd read-single-line drop dup 0 > while
    [char] , str-split
    2swap to-number -rot
    [char] , str-split
    2swap to-number -rot
    to-number
    rot'
    3 +
  repeat
  2drop
  to-array 3 /
;

: read-scanners
  { fd buf -- scanners n }
  0
  begin
    buf fd read-single-line while
    drop
    fd buf read-scanner
    rot 2 +
  repeat
  drop buf free throw
  fd close-file throw
  to-array 2/
;

: total-beacons
  { scanners n -- u }
  0
  n 0 do
    scanners i 2 * 1 + cells + @ +
  loop
;

: xyz
  { scanner i }
  scanner i 3 * cells + @
  scanner i 3 * 1 + cells + @
  scanner i 3 * 2 + cells + @
;

: diff
  { x1 y1 z1 x2 y2 z2 }
  x1 x2 -
  y1 y2 -
  z1 z2 -
;

: gfr
  { r i j }
  rotation-matrices r 9 * i 3 * j + + cells + @
;

: rotate
  { x y z r -- x' y' z' }
  x r 0 0 gfr * y r 1 0 gfr * z r 2 0 gfr * + +
  x r 0 1 gfr * y r 1 1 gfr * z r 2 1 gfr * + +
  x r 0 2 gfr * y r 1 2 gfr * z r 2 2 gfr * + +
;

: scanners-overlap-given-dr?
  { s1 n1 s2 n2 xd yd zd r -- f }
  0
  n1 0 do
    n2 0 do
      s1 j xyz s2 i xyz r rotate diff zd = -rot yd = swap xd = and and if
        1+
      then
    loop
  loop
  12 >=
;

: scanners-overlap?
  { s1 n1 s2 n2 -- f }
  24 0 do
    n1 0 do
      n2 11 - 0 do
        s1 n1 s2 n2 s1 j xyz s2 i xyz k rotate diff k scanners-overlap-given-dr? if
          s2 n2 s1 j xyz s2 i xyz k rotate diff k true unloop unloop unloop exit
        then
      loop
    loop
  loop
  false
;

: all
  { arr n }
  n 0 do
    arr i cells + @ invert if
      false unloop exit
    then
  loop
  true
;

: contains-beacon
  { x y z beacons n -- f}
  n 0 +do
    beacons i 3 * cells + @ x =
    beacons i 3 * 1 + cells + @ y =
    beacons i 3 * 2 + cells + @ z =
    and and if
      true unloop exit
    then
  loop
  false
;

: add-to-final
  { final-beacons n1 beacons n2 xd yd zd r -- xd yd zd final-beacons n2 }
  n1
  n2 0 do
    beacons i xyz r rotate rot xd + rot yd + rot zd +
    3dup final-beacons n1 contains-beacon invert if
      3dup
      final-beacons 7 pick 3 * 2 + cells + !
      final-beacons 6 pick 3 * 1 + cells + !
      final-beacons 5 pick 3 * cells + !
      rot' 1+ -rot'
    then
    beacons i 3 * 2 + cells + !
    beacons i 3 * 1 + cells + !
    beacons i 3 * cells + !
  loop
  final-beacons swap
  xd yd zd
;

: canonicalize-beacons'
  { scanners ns final-beacons nb processed processed' locations }
  final-beacons nb
  begin
    processed ns all invert while
    ns 0 do
      processed i cells + @ processed' i cells + @ invert and if
        ns 0 do
          j i = invert processed i cells + @ invert and if
            j . i .
            scanners j 2 * cells + @ scanners j 2 * 1 + cells + @
            scanners i 2 * cells + @ scanners i 2 * 1 + cells + @
            scanners-overlap? if
              s" found" type
              add-to-final
              locations i 3 * 2 + cells + !
              locations i 3 * 1 + cells + !
              locations i 3 * cells + !
              true processed i cells + !
            then
            CR
          then
        loop
        true processed' i cells + !
      then
    loop
  repeat
  locations ns 2swap
;

: canonicalize-beacons
  { scanners n }
  scanners n
  scanners n total-beacons 3 * cells allocate throw 0
  scanners @ scanners cell+ @ 0 0 0 0 add-to-final 3drop
  n cells allocate throw
  true over !
  n 1 do
    false over i cells + !
  loop
  n cells allocate throw
  n 0 do
    false over i cells + !
  loop
  n 3 * cells allocate throw
  0 over !
  0 over cell+ !
  0 over 2 cells + !
  canonicalize-beacons'
;

: max-distance
  { locations n }
  0
  n 0 do
    n 0 do
      locations i xyz
      locations j xyz
      diff
      rot abs rot abs rot abs
      + +
      max
    loop
  loop
;

:noname
  next-arg 2drop
  next-arg to-number drop
  next-arg fopen
  max-line chars allocate throw
  read-scanners
  canonicalize-beacons
  . CR free throw
  max-distance
  . CR
  bye
; IS 'cold

