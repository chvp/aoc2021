needs ../lib.fs

: is-upper
  { c }
  c [char] A >= c [char] Z <= and
;

: check-stack
  { s-addr n dupe depth -- s-addr n dupe f }
  s-addr n s" start" str= if
    s-addr n dupe false exit
  then
  s-addr c@ is-upper if
    s-addr n dupe true exit
  then
  depth 1 > if
    depth 1 do
      i 3 * 1 + pick
      i 3 * 1 + pick
      s-addr n str= if
        dupe if
          unloop s-addr n dupe false exit
        else
          unloop s-addr n true true exit
        then
      then
    loop
  then
  s-addr n dupe true
;

: count-paths
  { dupe nodes n depth -- u }
  2dup s" end" str= if
    1 exit
  then
  0 -rot ( count s-addr n )
  n 0 do
    2dup ( count s-addr n s-addr n )
    nodes i 4 * cells + @
    nodes i 4 * 1 + cells + @ ( count s-addr n s-addr n n-addr n )
    str= if
      nodes i 4 * 2 + cells + @
      nodes i 4 * 3 + cells + @ ( count s-addr n n-addr n )
      dupe depth check-stack if
        nodes n depth 1 + recurse ( count s-addr n n-addr n u )
        tuck'''
        >r >r >r >r + r> r> r> r>
      else
        drop
      then
      2drop
    then
  loop
  rot
;

: read-graph
  { fd -- addr n }
  0 >r
  begin
    max-line chars allocate throw
    dup fd read-single-line while
    [char] - str-split
    2over 2over 2swap
    r> 8 + >r
  repeat
  drop free throw
  fd close-file throw
  r> to-array 4 /
;

:noname
  next-arg 2drop
  s" start"
  next-arg to-number
  1 = if true else false then
  next-arg fopen
  read-graph
  0 count-paths
  . CR
  2drop
  bye
; IS 'cold
