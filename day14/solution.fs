needs ../lib.fs

: pair-to-num
  { s-addr }
  s-addr c@ [char] A - 26 * s-addr char+ c@ [char] A - +
;

: 2pair-to-3num
  { s-addr n1 c-addr n2 }
  s-addr c@ [char] A - 26 * s-addr char+ c@ [char] A - +
  s-addr c@ [char] A - 26 * c-addr c@ [char] A - +
  c-addr c@ [char] A - 26 * s-addr char+ c@ [char] A - +
;

: read-polymer
  { polybuf fd }
  max-line chars allocate throw
  dup fd read-single-line invert throw 
  2dup 1 - chars + c@ [char] A - >r
  1 - 0 do
    polybuf over i chars + pair-to-num cells + 1 swap +!
  loop
  dup fd read-single-line invert throw throw free throw
  r> polybuf fd
;

: read-rules
  { fd }
  0 >r
  max-line chars allocate throw
  begin
    dup fd read-single-line while
    dup' 2dup 1 - chars + 1
    2pair-to-3num
    r> 3 + >r
    rot'
  repeat
  drop free throw
  fd close-file throw
  r> to-array 3 /
;

: simulate-step
  { polybuf rules nr }
  26 26 * cells allocate throw
  26 26 * 0 do
    dup i cells + 0 swap !
  loop
  nr 0 do
    dup
    polybuf rules i 3 * cells + @ cells + @ swap 2dup
    rules i 3 * 1 + cells + @ cells + +!
    rules i 3 * 2 + cells + @ cells + +!
  loop
  polybuf free throw
  rules nr
;

: simulate-steps
  { num lastc polybuf rules nr }
  polybuf rules nr
  num 0 do
    simulate-step
  loop
  drop free throw
  lastc swap
;

: count-chars
  { lastc polybuf }
  26 cells allocate throw
  26 0 do
    dup i cells + 0 swap !
  loop
  dup lastc cells + 1 swap !
  26 0 do
    26 0 do
      polybuf j 26 * i + cells + @ over j cells + +!
    loop
  loop
  polybuf free throw
;

:noname
  next-arg 2drop
  next-arg to-number
  1 = if 10 else 40 then
  26 26 * cells allocate throw
  26 26 * 0 do
    dup i cells + 0 swap !
  loop
  next-arg fopen
  read-polymer
  read-rules
  simulate-steps
  count-chars
  dup 26 2dup
  array-max -rot
  array-min>0
  - . CR
  free throw
  bye
; IS 'cold
