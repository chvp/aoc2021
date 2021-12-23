needs ../heap.fs

: nop ;

: test1
  ['] nop heap-init { heap }
  2 heap heap-add
  7 heap heap-add
  4 heap heap-add
  8 heap heap-add
  9 heap heap-add
  3 heap heap-add
  1 heap heap-add
  6 heap heap-add
  5 heap heap-add
  10 heap heap-add
  heap heap-size assert( 10 = )
  heap heap-pop assert( 1 = )
  heap heap-pop assert( 2 = )
  heap heap-pop assert( 3 = )
  heap heap-pop assert( 4 = )
  heap heap-pop assert( 5 = )
  heap heap-pop assert( 6 = )
  heap heap-pop assert( 7 = )
  heap heap-pop assert( 8 = )
  heap heap-pop assert( 9 = )
  heap heap-pop assert( 10 = )
  heap heap-size assert( 0 = )
;

: test2
  ['] nop heap-init { heap }
  2 heap heap-add
  7 heap heap-add
  4 heap heap-add
  heap heap-pop assert( 2 = )
  8 heap heap-add
  heap heap-pop assert( 4 = )
  9 heap heap-add
  3 heap heap-add
  heap heap-pop assert( 3 = )
  1 heap heap-add
  6 heap heap-add
  heap heap-pop assert( 1 = )
  5 heap heap-add
  10 heap heap-add
  heap heap-pop assert( 5 = )
  heap heap-pop assert( 6 = )
  heap heap-pop assert( 7 = )
  heap heap-pop assert( 8 = )
  heap heap-pop assert( 9 = )
  heap heap-pop assert( 10 = )
  heap heap-size assert( 0 = )
;

: test3
  ['] nop heap-init { heap }
  1000 0 do
    i i 2 mod 1 = if
      negate
    then
    heap heap-add
  loop
  heap heap-size assert( 1000 = )
  500 0 do
    heap heap-pop assert( -1000 i 2 * 1 + + = )
  loop
  500 0 do
    heap heap-pop assert( i 2 * = )
  loop
  heap heap-size assert( 0 = )
  heap heap-maxsize assert( 1023 = )
;

." test1" test1 ."  succeeded" CR
." test2" test2 ."  succeeded" CR
." test3" test3 ."  succeeded" CR
bye
