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

: check-stack-part2
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

: count-paths-part2
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
      dupe depth check-stack-part2 if
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

: count-paths-part1
  { nodes n depth -- u }
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
      depth check-stack-part1 if
        nodes n depth 1 + recurse ( count s-addr n n-addr n u )
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
    next-arg fopen
    read-graph
    0 count-paths-part1
  else
    false
    next-arg fopen
    read-graph
    0 count-paths-part2
  then
  . CR
  2drop
  bye
; IS 'cold
