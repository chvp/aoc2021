needs ../lib.fs

: parse-line
  ( s n -- f xmin xmax ymin ymax zmin zmax )
  BL str-split
  2swap s" on" str= -rot
  swap 2 chars + swap 2 - \ Skip x=
  [char] . str-split
  2swap to-number -rot
  swap 1 chars + swap 1 - \ Skip .
  [char] , str-split
  2swap to-number -rot
  swap 2 chars + swap 2 - \ Skip y=
  [char] . str-split
  2swap to-number -rot
  swap 1 chars + swap 1 - \ Skip .
  [char] , str-split
  2swap to-number -rot
  swap 2 chars + swap 2 - \ Skip y=
  [char] . str-split
  2swap to-number -rot
  swap 1 chars + swap 1 - \ Skip .
  to-number
;

: handle-input
  { fd buf xt }
  begin
    buf buf fd read-single-line while
    parse-line
    xt execute
  repeat
  drop free throw
  fd close-file throw
;

: fill-cuboid
  { grid f xmin xmax ymin ymax zmin zmax }
  xmax 50 + 100 min 1 + xmin 50 + 0 max +do
    ymax 50 + 100 min 1 + ymin 50 + 0 max +do
      zmax 50 + 100 min 1 + zmin 50 + 0 max +do
        f grid k 100 * j + 100 * i + cells + !
      loop
    loop
  loop
  grid
;

: count-true
  { grid }
  0
  101 101 101 * * 0 do
    grid i cells + @ if
      1+
    then
  loop
;

: no-overlap-interval
  { min max min' max' }
  max' min < min' max > or
;

: count-total-volume
  { l n }
  0
  n 0 > if
    n 0 do
      l i 6 * 0 + cells + @ l i 6 * 1 + cells + @ - abs 1+
      l i 6 * 2 + cells + @ l i 6 * 3 + cells + @ - abs 1+
      l i 6 * 4 + cells + @ l i 6 * 5 + cells + @ - abs 1+
      * * +
    loop
  then
;

: add-to-list
  { xmin xmax ymin ymax zmin zmax }
  2dup 6 * cells + xmin swap !
  2dup 6 * 1 + cells + xmax swap !
  2dup 6 * 2 + cells + ymin swap !
  2dup 6 * 3 + cells + ymax swap !
  2dup 6 * 4 + cells + zmin swap !
  2dup 6 * 5 + cells + zmax swap !
  1+
;

: add-minus-to-list
  { xmin xmax ymin ymax zmin zmax xmin' xmax' ymin' ymax' zmin' zmax' }
  xmin xmax xmin' xmax' no-overlap-interval
  ymin ymax ymin' ymax' no-overlap-interval or
  zmin zmax zmin' zmax' no-overlap-interval or if
    xmin xmax ymin ymax zmin zmax add-to-list exit
  then
  xmin xmin' < if
    ymin ymin' < if
      zmin zmin' < if
        xmin xmin' 1 - ymin ymin' 1 - zmin zmin' 1 - add-to-list
      then
      xmin xmin' 1 - ymin ymin' 1 - zmin zmin' max zmax zmax' min add-to-list
      zmax zmax' > if
        xmin xmin' 1 - ymin ymin' 1 - zmax' 1 + zmax add-to-list
      then
    then
    zmin zmin' < if
      xmin xmin' 1 - ymin ymin' max ymax ymax' min zmin zmin' 1 - add-to-list
    then
    xmin xmin' 1 - ymin ymin' max ymax ymax' min zmin zmin' max zmax zmax' min add-to-list
    zmax zmax' > if
      xmin xmin' 1 - ymin ymin' max ymax ymax' min zmax' 1 + zmax add-to-list
    then
    ymax ymax' > if
      zmin zmin' < if
        xmin xmin' 1 - ymax' 1 + ymax zmin zmin' 1 - add-to-list
      then
      xmin xmin' 1 - ymax' 1 + ymax zmin zmin' max zmax zmax' min add-to-list
      zmax zmax' > if
        xmin xmin' 1 - ymax' 1 + ymax zmax' 1 + zmax add-to-list
      then
    then
  then
  ymin ymin' < if
    zmin zmin' < if
      xmin xmin' max xmax xmax' min ymin ymin' 1 - zmin zmin' 1 - add-to-list
    then
    xmin xmin' max xmax xmax' min ymin ymin' 1 - zmin zmin' max zmax zmax' min add-to-list
    zmax zmax' > if
      xmin xmin' max xmax xmax' min ymin ymin' 1 - zmax' 1 + zmax add-to-list
    then
  then
  zmin zmin' < if
    xmin xmin' max xmax xmax' min ymin ymin' max ymax ymax' min zmin zmin' 1 - add-to-list
  then
  \ xmin xmin' max xmax xmax' min ymin ymin' max ymax ymax' min zmin zmin' max zmax zmax' min add-to-list
  zmax zmax' > if
    xmin xmin' max xmax xmax' min ymin ymin' max ymax ymax' min zmax' 1 + zmax add-to-list
  then
  ymax ymax' > if
    zmin zmin' < if
      xmin xmin' max xmax xmax' min ymax' 1 + ymax zmin zmin' 1 - add-to-list
    then
    xmin xmin' max xmax xmax' min ymax' 1 + ymax zmin zmin' max zmax zmax' min add-to-list
    zmax zmax' > if
      xmin xmin' max xmax xmax' min ymax' 1 + ymax zmax' 1 + zmax add-to-list
    then
  then
  xmax xmax' > if
    ymin ymin' < if
      zmin zmin' < if
        xmax' 1 + xmax ymin ymin' 1 - zmin zmin' 1 - add-to-list
      then
      xmax' 1 + xmax ymin ymin' 1 - zmin zmin' max zmax zmax' min add-to-list
      zmax zmax' > if
        xmax' 1 + xmax ymin ymin' 1 - zmax' 1 + zmax add-to-list
      then
    then
    zmin zmin' < if
      xmax' 1 + xmax ymin ymin' max ymax ymax' min zmin zmin' 1 - add-to-list
    then
    xmax' 1 + xmax ymin ymin' max ymax ymax' min zmin zmin' max zmax zmax' min add-to-list
    zmax zmax' > if
      xmax' 1 + xmax ymin ymin' max ymax ymax' min zmax' 1 + zmax add-to-list
    then
    ymax ymax' > if
      zmin zmin' < if
        xmax' 1 + xmax ymax' 1 + ymax zmin zmin' 1 - add-to-list
      then
      xmax' 1 + xmax ymax' 1 + ymax zmin zmin' max zmax zmax' min add-to-list
      zmax zmax' > if
        xmax' 1 + xmax ymax' 1 + ymax zmax' 1 + zmax add-to-list
      then
    then
  then
;

: calculate-intersections-list
  { l n f xmin xmax ymin ymax zmin zmax }
  n 26 * 1 + 6 * cells allocate throw
  0
  n 0 > if
    n 0 do
      l i 6 * cells + @ l i 6 * 1 + cells + @
      l i 6 * 2 + cells + @ l i 6 * 3 + cells + @
      l i 6 * 4 + cells + @ l i 6 * 5 + cells + @
      xmin xmax ymin ymax zmin zmax add-minus-to-list
    loop
  then
  f if
    xmin xmax ymin ymax zmin zmax add-to-list
  then
  l free throw
;

:noname
  next-arg 2drop
  next-arg to-number 1 = if
    101 101 101 * * cells allocate throw
    101 101 101 * * 0 do
      false over i cells + !
    loop
    next-arg fopen
    max-line chars allocate throw
    ['] fill-cuboid handle-input
    count-true
  else
    1 cells allocate throw 0
    next-arg fopen
    max-line chars allocate throw
    ['] calculate-intersections-list handle-input
    count-total-volume
  then
  . CR
  bye
; IS 'cold
