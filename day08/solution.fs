needs ../lib.fs

: is-1-4-7-8
  { n -- f }
  n 2 = n 3 = or n 4 = or n 7 = or
;

: read-input-and-count
  { fd buf }
  0
  begin
    buf buf fd read-single-line while
    [char] | str-split
    2swap 2drop swap 1 chars + swap 1 -
    3 0 do
      BL str-split
      2swap
      nip
      is-1-4-7-8 if
        rot 1+ -rot
      then
    loop
    nip
    is-1-4-7-8 if
      1+
    then
  repeat
  drop free throw
  fd close-file throw
;

: common-letters
  { s1 n1 s2 n2 }
  n1 n2 min chars allocate throw 0
  n1 0 do
    n2 0 do
      s1 j chars + c@ s2 i chars + c@ = if
        2dup s1 j chars + c@ -rot chars + c! 1+
      then
    loop
  loop
;

: minus
  { s1 n1 s2 n2 }
  n1 chars allocate throw 0
  n1 0 do
    true
    n2 0 do
      s1 j chars + c@ s2 i chars + c@ = invert and
    loop
    if
      2dup s1 i chars + c@ -rot chars + c! 1+
    then
  loop
;

: find-len
  { reads n len }
  n 2 / 0 do
    reads i 2 * 1 + cells + @ len = if
      reads i 2 * cells + @ reads i 2 * 1 + cells + @
    then
  loop
;

: find-1 2 find-len ;
: find-4 4 find-len ;
: find-7 3 find-len ;
: find-8 7 find-len ;

: cdf
  { seg-addr buf }
  seg-addr 2 chars + c@ buf c!
  seg-addr 3 chars + c@ buf 1 chars + c!
  seg-addr 5 chars + c@ buf 2 chars + c!
  buf
;

: abcdfg
  { seg-addr buf }
  seg-addr c@ buf c!
  seg-addr 1 chars + c@ buf 1 chars + c!
  seg-addr 2 chars + c@ buf 2 chars + c!
  seg-addr 3 chars + c@ buf 3 chars + c!
  seg-addr 5 chars + c@ buf 4 chars + c!
  seg-addr 6 chars + c@ buf 5 chars + c!
  buf
;
  

: figure-out'
  { seg-addr s1 n1 s4 n4 s7 n7 s8 n8 reads reads-n }
  s7 n7 s1 n1 minus drop dup c@ seg-addr c! free throw
  reads reads-n 6 find-len
  3 0 do
    s1 n1 common-letters
    1 = if
      dup c@ seg-addr 5 chars + c!
    then
    free throw
  loop
  s1 n1 seg-addr 5 chars + 1 minus drop dup c@ seg-addr 2 chars + c! free throw
  reads reads-n 5 find-len
  0 0
  3 0 do
    2swap 2dup s1 n1 common-letters
    2 = swap free throw if
      2dup s1 n1 minus
      2rot 2drop 2swap
    then
    2drop
  loop
  2dup 2dup
  s4 n4 common-letters dup' s1 n1 minus drop dup c@ seg-addr 3 chars + c! free throw free throw
  s4 n4 minus dup' s7 n7 minus drop dup c@ seg-addr 6 chars + c! free throw free throw
  drop free throw
  seg-addr 3 chars allocate throw cdf dup 3
  s4 n4 2swap minus drop dup c@ seg-addr 1 chars + c! free throw free throw
  seg-addr 6 chars allocate throw abcdfg dup 6
  s8 n8 2swap minus drop dup c@ seg-addr 4 chars + c! free throw free throw
  seg-addr
  reads free throw
;

: figure-out
  { s-addr n }
  7 chars allocate throw
  s-addr n
  9 0 do
    BL str-split
  loop
  1-
  20 to-array
  2dup find-1 2swap
  2dup find-4 2swap
  2dup find-7 2swap
  2dup find-8 2swap
  figure-out'
;

: make-digit
  { seg s n }
  n 2 = if
    [char] 1 exit
  then
  n 4 = if
    [char] 4 exit
  then
  n 3 = if
    [char] 7 exit
  then
  n 7 = if
    [char] 8 exit
  then
  s n seg 5 chars + c@ contains invert if
    [char] 2 exit
  then
  s n seg 2 chars + c@ contains s n seg 4 chars + c@ contains and if
    [char] 0 exit
  then
  s n seg 4 chars + c@ contains if
    [char] 6 exit
  then
  n 5 = s n seg 2 chars + c@ contains and if
    [char] 3 exit
  then
  n 5 = if
    [char] 5 exit
  then
  [char] 9 exit
;

: make-number
  { seg-addr s1 n1 s2 n2 s3 n3 s4 n4 }
  4 chars allocate throw dup dup dup dup dup
  seg-addr s1 n1 make-digit swap c!
  chars 1 + seg-addr s2 n2 make-digit swap c!
  chars 2 + seg-addr s3 n3 make-digit swap c!
  chars 3 + seg-addr s4 n4 make-digit swap c!
  4 to-number swap free throw
  seg-addr free throw
;

: analyze-line
  { s-addr n }
  s-addr n [char] | str-split
  swap 1 chars + swap 1 -
  2swap figure-out
  -rot
  3 0 do
    BL str-split
  loop
  make-number
;

: read-input-and-sum
  { fd buf }
  0
  begin
    buf buf fd read-single-line while
    analyze-line +
  repeat
  drop free throw
  fd close-file throw
;

:noname
  next-arg 2drop
  next-arg to-number
  1 = if
    ['] read-input-and-count
  else
    ['] read-input-and-sum
  then
  next-arg fopen
  max-line chars allocate throw
  rot execute
  . CR
  bye
; IS 'cold
