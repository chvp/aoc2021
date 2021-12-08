\ \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
\ \\\ Stack management
\ \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

: 3dup
  { a b c -- a b c a b c }
  a b c a b c ;

: 3swap
  { a b c d e f -- d e f a b c }
  d e f a b c ;

: 3drop
  { d e f -- }
;

: dup'
  { a b -- a a b }
  a a b ;

: 2dup'
  { a b c -- a b a b c }
  a b a b c ;

: rot'
  { a b c d -- b c d a }
  b c d a ;

: nip''
  { a b c d -- b c d }
  b c d ;

: nip'''
  { a b c d e -- b c d e }
  b c d e ;

: swap'
  { a b c -- b a c }
  b a c ;

: 2swap'
  { a b c d e -- c d a b e }
  c d a b e ;

: 2swap''
  { a b c d e f -- c d a b e f }
  c d a b e f ;

: tuck'''
  { a b c d e -- e a b c d }
  e a b c d ;

\ \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
\ \\\ String functions
\ \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

: str-split
  { s n c -- s1 n1 s2 n2 }
  c pad c!
  s n pad 1 search invert throw ( s2' n2' )
  1 - swap 1 chars + swap ( s2 n2 )
  s n 2 pick - 1 - 2swap ;

: contains
  { s n c -- f }
  c pad c!
  s n pad 1 search -rot 2drop ;

: trim-front
  ( s n ) { c -- s' n' }
  begin
    swap dup c@ c = while
    1 chars +
    swap
    1 -
  repeat
  swap
;

: to-number s>number? invert throw d>s ;

\ \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
\ \\\ Array functions
\ \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

: store-at
  { base i n -- }
  n base i cells + ! ;

: copy-array
  { base n -- base' n }
  n cells allocate throw
  n 0 do
    base i cells + @
    over i cells + !
  loop
  n ;

: array-max
  { a-addr n }
  0
  n 0 do
    a-addr i cells + @
    max
  loop
;

\ \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
\ \\\ File reading & parsing
\ \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

4096 Constant max-line
: fopen r/o open-file throw ;

: read-single-line
  { buffer fd-in -- saddr n eof }
  buffer max-line fd-in read-line throw ;

: comma-line-to-numbers
  ( s-addr u -- ... n )
  0 -rot
  begin
    2dup [char] , contains while
    [char] , str-split
    2swap to-number ( count s n u )
    -rot 2swap swap 1 + 2swap ( u count+1 s n )
  repeat
  to-number swap 1 +
;

: read-file-into-numbers'
  { fd-in -- ... n }
  0
  max-line chars allocate throw
  begin
    ( buffer ) dup
    ( buffer ) fd-in read-single-line
  while
    ( buffer read-length ) dup' to-number ( buffer number )
    ( nread buffer number ) rot 1+ rot ( number nread+1 buffer )
  repeat
  ( read-length ) drop
  fd-in close-file throw
  ( buffer ) free throw ;

: to-array
  { length -- a-addr length }
  length cells allocate throw
  length 0 do
    ( base-addr ) dup length cells + i cells - 1 cells - ( base-addr cur-addr )
    ( number base-addr cur-addr ) swap' !
  loop
  length ;

: read-file-into-numbers
  ( s u -- a-addr length )
  fopen read-file-into-numbers' to-array ;


\ \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
\ \\\ Control structures
\ \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

\ All XTs need to have the same stack signature. It is up to the
\ caller to make sure that the stack beneath the switch arguments
\ can be used by the XTs.
: switch ( xt0 s1 u1 xt1 .. sn un xtn n c-addr u -- )
  2 pick 0 do
    i 1 + 3 * 2 + pick
    i 1 + 3 * 2 + pick
    i 1 + 3 * 2 + pick ( c-addr u si ui xti )
    tuck'''
    2over
    str= if
      ( s1 u1 xt1 .. sn un xtn n xt1 c-addr u )
      2drop
      swap
      3 * 1 + 0 do
        nip
      loop
      execute
      unloop exit
    else
      rot
      drop
    then
  loop
  2drop
  3 * 0 do
    drop
  loop
  execute ;
