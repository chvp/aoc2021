needs ../treeset.fs

: test1
  ['] < ['] = treeset-init { set }
  4 set treeset-add
  2 set treeset-add
  1 set treeset-add
  3 set treeset-add
  6 set treeset-add
  5 set treeset-add
  7 set treeset-add
  1 set treeset-add
  assert( 1 set treeset-contains )
  assert( 2 set treeset-contains )
  assert( 3 set treeset-contains )
  assert( 4 set treeset-contains )
  assert( 5 set treeset-contains )
  assert( 6 set treeset-contains )
  assert( 7 set treeset-contains )
  assert( 8 set treeset-contains invert )
  set treeset-size assert( 7 = )
;

." test1" test1 ."  succeeded" CR
bye
