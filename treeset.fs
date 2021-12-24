needs lib.fs

: treeset-data @ ;
: treeset-maxdepth cell+ @ ;
: treeset-eq 2 cells + @ ;
: treeset-lt 3 cells + @ ;
: treeset-size 4 cells + @ ;

: treeset-init
  { lt eq }
  5 cells allocate throw
  2 6 pow 1 - cells allocate throw
  2 6 pow 1 - 0 do
    0 over i cells + !
  loop
  over !
  5 over cell+ !
  eq over 2 cells + !
  lt over 3 cells + !
  0 over 4 cells + !
;

: treeset-grow
  { treeset }
  2 treeset treeset-maxdepth 2 + pow 1 - cells allocate throw
  2 treeset treeset-maxdepth 2 + pow 1 - 2 treeset treeset-maxdepth 1 + pow 1 - do
    0 over i cells + !
  loop
  2 treeset treeset-maxdepth 1 + pow 1 - 0 do
    treeset treeset-data i cells + @ over i cells + !
  loop
  treeset treeset-data free throw
  treeset !
;

: treeset-add'
  { spot depth e treeset }
  depth treeset treeset-maxdepth > if
    treeset treeset-grow
  then
  treeset treeset-data spot cells + @ 0 = if
    e treeset treeset-data spot cells + !
    true
    exit
  then
  treeset treeset-data spot cells + @ e treeset treeset-eq execute if false exit then
  treeset treeset-data spot cells + @ e treeset treeset-lt execute if
    spot 2* 1+ 
  else
    spot 2* 2 +
  then
  depth 1+ e treeset recurse
;

: treeset-add
  { e treeset }
  0 0 e treeset treeset-add' if
    1 treeset 4 cells + +!
  then
;

: treeset-contains'
  { spot depth e treeset }
  depth treeset treeset-maxdepth > if false exit then
  treeset treeset-data spot cells + @ 0 = if false exit then
  treeset treeset-data spot cells + @ e treeset treeset-eq execute if true exit then
  treeset treeset-data spot cells + @ e treeset treeset-lt execute if
    spot 2* 1+ 
  else
    spot 2* 2 +
  then
  depth 1+ e treeset recurse
;

: treeset-contains
  { e treeset }
  0 0 e treeset treeset-contains'
;
