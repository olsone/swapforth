\ set direction with $0f $0080 io!
\ up counter, to $ff, on outputs


: hdr2.dir
  $ff $0080 io!
;

: hdr2.cycle
 $ff 0 do 
   i $0040 io!
 loop
;

: hdr2.start
  hdr2.dir
  begin
    hdr2.cycle
  0 until
;
