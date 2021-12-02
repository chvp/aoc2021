\ \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
\ \\\ Stack management
\ \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

: 3dup { a b c -- a b c a b c }
    a b c a b c ;
: 3swap { a b c d e f -- d e f a b c }
    d e f a b c ;
: 3drop { d e f -- }
;
: dup' { a b -- a a b }
    a a b ;
: swap' { a b c -- b a c }
    b a c ;

\ \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
\ \\\ File reading & parsing
\ \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

32 Constant c-space
4096 Constant max-line
: fopen r/o open-file throw ;

: read-single-line { buffer fd-in -- saddr n eof }
    buffer max-line fd-in read-line throw ;

: read-file-into-numbers' { fd-in -- ... n }
    0
    max-line cells allocate throw
    begin
        ( buffer ) dup
        ( buffer ) fd-in read-single-line
    while
            ( buffer read-length ) dup' s>number? invert throw d>s ( buffer number )
            ( nread buffer number ) rot 1+ rot ( number nread+1 buffer )
    repeat
    ( read-length ) drop
    fd-in close-file throw
    ( buffer ) free throw ;
: to-array { length -- a-addr length }
    length cells allocate throw
    length 0 u+do
        ( base-addr ) dup length cells + i cells - 1 cells - ( base-addr cur-addr )
        ( number base-addr cur-addr ) swap' !
    loop
    length ;
: read-file-into-numbers ( s u -- a-addr length )
    fopen read-file-into-numbers' to-array ;


\ \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
\ \\\ String functions
\ \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

: str-split { s n c -- s1 n1 s2 n2 }
    c pad c!
    s n pad 1 search invert throw ( s2' n2' )
    1 - swap 1 chars + swap ( s2 n2 )
    s n 2 pick - 1 - 2swap ;
