needs ../lib.fs

: is-upper
  { c }
  c [char] A >= c [char] Z <= and
;

: check-stack-part1
  { s-addr n depth -- s-addr n f }
  s-addr n s" start" str= if
    s-addr n false exit
  then
  s-addr c@ is-upper if
    s-addr n true exit
  then
  depth 1 > if
    depth 1 do
      i 3 * 1 + pick
      i 3 * 1 + pick
      s-addr n str= if
        unloop s-addr n false exit
      then
    loop
  then
  s-addr n true
;

: has-small-dup
  { depth -- f }
  depth 1 > if
    depth 1 - 0 do
      i 3 * 1 + pick c@ is-upper invert if
        depth i 1 + do
          j 3 * 1 + pick
          j 3 * 1 + pick
          i 3 * 3 + pick
          i 3 * 3 + pick
          str= if
            true unloop unloop exit
          then
        loop
      then
    loop
  then
  false
;

: check-stack-part2
  { s-addr n depth -- s-addr n f }
  s-addr n s" start" str= if
    s-addr n false exit
  then
  depth has-small-dup invert if
    s-addr n true exit
  then
  s-addr n depth check-stack-part1
;

: count-paths
  { check-stack nodes n depth -- u }
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
      depth check-stack execute if
        check-stack nodes n depth 1 + recurse ( count s-addr n n-addr n u )
        tuck'''
        >r >r >r >r + r> r> r> r>
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
  1 = if
    ['] check-stack-part1
  else
    ['] check-stack-part2
  then
  next-arg fopen
  read-graph
  0 count-paths
  . CR
  2drop
  bye
; IS 'cold
