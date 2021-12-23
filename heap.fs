needs lib.fs

: heap-size
  { heap }
  heap @
;

: heap-maxdepth
  { heap }
  heap cell+ @
;

: heap-maxsize
  { heap }
  2 heap heap-maxdepth pow 1 -
;

: heap-tree
  { heap }
  heap 2 cells + @
;

: heap-realloc
  { heap }
  2 heap heap-maxdepth 1 + pow 1 - cells allocate throw
  heap heap-maxsize 0 do
    heap heap-tree i cells + @ over i cells + !
  loop
  heap heap-tree free throw
  heap heap-maxdepth 1 + heap cell+ !
  heap 2 cells + !
;

: heap-init
  { get-value }
  4 cells allocate throw
  0 over !
  6 over cell+ !
  2 6 pow 1 - cells allocate throw over 2 cells + !
  get-value over 3 cells + !
;

: heap-getvalue
  { e heap }
  e heap 3 cells + @ execute
;

: heap-getelement
  { spot heap }
  heap heap-tree spot cells + @
;

: heap-bubble-up
  { spot heap }
  spot 0 = if exit then
  heap heap-tree spot cells + @ heap heap-getvalue
  heap heap-tree spot 1- 2/ cells + @ heap heap-getvalue > if
    exit
  then
  heap heap-tree spot cells + @
  heap heap-tree spot 1- 2/ cells + @
  heap heap-tree spot cells + !
  heap heap-tree spot 1- 2/ cells + !
  spot 1- 2/ heap recurse
;
  
: heap-add
  { e heap }
  heap heap-size heap heap-maxsize = if
    heap heap-realloc
  then
  e heap heap-tree heap heap-size cells + !
  heap heap-size heap heap-bubble-up
  1 heap +!
; 

: heap-has-left-child
  { spot heap }
  spot 2 * 1 + heap @ <
;

: heap-has-right-child
  { spot heap }
  spot 2 * 2 + heap @ <
;

defer 'heap-bubble-down

: heap-down-left
  { spot heap }
  heap heap-tree spot cells + @
  heap heap-tree spot 2 * 1 + cells + @
  heap heap-tree spot cells + !
  heap heap-tree spot 2 * 1 + cells + !
  spot 2 * 1 + heap 'heap-bubble-down
;

: heap-down-right
  { spot heap }
  heap heap-tree spot cells + @
  heap heap-tree spot 2 * 2 + cells + @
  heap heap-tree spot cells + !
  heap heap-tree spot 2 * 2 + cells + !
  spot 2 * 2 + heap 'heap-bubble-down
;

: heap-bubble-down
  { spot heap }
  spot heap heap-has-left-child if
    spot heap heap-has-right-child if
      spot 2 * 1 + heap heap-getelement heap heap-getvalue
      spot 2 * 2 + heap heap-getelement heap heap-getvalue
      < if
        spot heap heap-getelement heap heap-getvalue
        spot 2 * 1 + heap heap-getelement heap heap-getvalue
        > if spot heap heap-down-left then
      else
        spot heap heap-getelement heap heap-getvalue
        spot 2 * 2 + heap heap-getelement heap heap-getvalue
        > if spot heap heap-down-right then
      then
    else
      spot heap heap-getelement heap heap-getvalue
      spot 2 * 1 + heap heap-getelement heap heap-getvalue
      > if spot heap heap-down-left then
    then
  then
;
' heap-bubble-down is 'heap-bubble-down

: heap-pop
  { heap }
  heap heap-tree @
  heap heap-tree heap heap-size 1 - cells + @ heap heap-tree !
  -1 heap +!
  0 heap heap-bubble-down
;
