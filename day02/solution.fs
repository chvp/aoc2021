s" ../lib.fs" included

: looped-part' { xt fd-in buffer -- }
    begin
        buffer fd-in read-single-line
    while
            buffer swap ( saddr n )
            c-space str-split ( saddr1 n saddr2 n )
            s>number? invert throw d>s ( saddr1 n u )
            -rot ( u saddr1 n )
            xt execute
    repeat
    ( read-length ) drop
    buffer free throw
    fd-in close-file throw ;

: looped-part ( xt -- )
    next-arg fopen
    max-line cells allocate throw
    looped-part' ;

: part1 { depth pos x c-addr n -- depth' pos' }
    c-addr n s" forward" str= if
        depth pos x +
    else
        x
        c-addr n s" up" str= if
            negate
        then
        depth + pos
    then ;

: part2  { aim depth pos x c-addr n -- aim' depth' pos' }
    c-addr n s" forward" str= if
        aim depth aim x * + pos x +
    else
        x
        c-addr n s" up" str= if
            negate
        then
        aim + depth pos
    then ;

: main
    next-arg s>number? invert throw d>s
    1 = if
        0 0 ['] part1 looped-part * . CR
    else
        0 0 0 ['] part2 looped-part * . CR drop
    then ;

main bye


