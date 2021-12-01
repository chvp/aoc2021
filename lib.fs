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

4096 Constant max-line
: fopen r/o open-file throw ;
: read-file-into-numbers' { fd-in -- ... n }
    0
    max-line cells allocate throw
    begin
        ( buffer ) dup
        ( buffer ) max-line fd-in read-line throw
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
