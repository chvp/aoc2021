needs ../lib.fs
needs ../treeset.fs

: w @ ;
: x cell+ @ ;
: y 2 cells + @ ;
: z 3 cells + @ ;
: ip 4 cells + @ ;

: type-state
  { state }
  5 0 do
    state i cells + @ .
  loop
  CR
;

: state-eq
  { state1 state2 -- f }
  true
  state1 w state2 w = and
  state1 z state2 z = and
  state1 ip state2 ip = and
;

: state-lt
  { state1 state2 -- f }
  state1 w state2 w < if true exit then
  state1 w state2 w > if false exit then
  state1 z state2 z < if true exit then
  state1 z state2 z > if false exit then
  state1 ip state2 ip <
;

: copy-state
  { state -- state' }
  5 cells allocate throw
  5 0 do
    state i cells + @ over i cells + !
  loop
;

defer 'search
defer loop-op

: instr-numop
  { state dest num xt }
  state dest cells + @ num xt execute
  state dest cells + !
;

: instr-add ['] + instr-numop ;
: instr-mul ['] * instr-numop ;
: instr-div ['] / instr-numop ;
: instr-mod ['] mod instr-numop ;
: instr-eql
  { state dest num }
  state dest cells + @ num = if 1 else 0 then
  state dest cells + !
;

create memo-set
create insts
create num-insts

: instr-inp'
  { dest state num }
  num state dest cells + !
  1 state 4 cells + +!
  state 'search
  state free throw
;

: instr-inp
  { state dest num }
  state memo-set @ treeset-contains if false exit then
  state ip 18 / 1 = if
    dest state copy-state 5 loop-op instr-inp' if
      5 loop-op .
      true unloop exit
    then
  else
    9 0 do
      state ip 18 / 4 < if
        state ip 18 / . i loop-op . CR
      then
      dest state copy-state i loop-op instr-inp' if
        i loop-op .
        true unloop exit
      then
    loop
  then
  state copy-state memo-set @ treeset-add
  false
;

: search-first
  { state }
  state ip num-insts @ = if state z 0 = exit then
  state ip 18 / 4 > state z 26 15 state ip 18 / - pow > and if false exit then
  state
  insts @ state ip 4 * 1 + cells + @
  insts @ state ip 4 * 3 + cells + @ if
    insts @ state ip 4 * 2 + cells + @
  else
    state insts @ state ip 4 * 2 + cells + @ cells + @
  then
  insts @ state ip 4 * cells + @
  dup ['] instr-inp = invert if
    execute
    1 state 4 cells + +!
    state recurse
  else
    execute
  then
;
' search-first is 'search

: largest-loop-op { n } 9 n - ;
: smallest-loop-op 1+ ;

: find-extreme
  5 cells allocate throw
  5 0 do 0 over i cells + ! loop
  search-first CR
;

: add-instr-add ['] instr-add ;
: add-instr-mul ['] instr-mul ;
: add-instr-div ['] instr-div ;
: add-instr-mod ['] instr-mod ;
: add-instr-eql ['] instr-eql ;

: parse-instruction
  { s n }
  s 3 s" inp" str= if
    ['] instr-inp
    s 4 chars + c@ [char] w -
    0 true
    exit
  then
  ['] add-instr-add
  s" add" ['] add-instr-add
  s" mul" ['] add-instr-mul
  s" div" ['] add-instr-div
  s" mod" ['] add-instr-mod
  s" eql" ['] add-instr-eql
  5 s 3 switch
  s 4 chars + c@ [char] w -
  s 6 chars + c@ [char] 9 <= if
    s 6 chars + n 6 - to-number
    true
  else
    s 6 chars + c@ [char] w -
    false
  then
  s free throw
;

: read-instructions
  { fd }
  0 >r
  begin
    max-line chars allocate throw
    dup fd read-single-line while
    parse-instruction
    r> 4 + >r
  repeat
  drop free throw
  r>
  to-array
  4 /
;

:noname
  next-arg 2drop
  next-arg to-number 1 = if
    ['] largest-loop-op is loop-op
  else
    ['] smallest-loop-op is loop-op
  then
  ['] state-lt ['] state-eq treeset-init memo-set !
  next-arg fopen
  read-instructions num-insts ! insts !
  find-extreme
  bye
; execute
