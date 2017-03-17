\ set direction=output, of the top 4 bits on j3 
: j3.dir
  $f0 $0080 io!
;
 

\ measure time to count to 16:
\ without this word: 117.5 us 
\ with this: 123 us
: j3!  ( d -- )
   $0040 io!
;

\ up counter, to $ff, on outputs
: j3.count
  $ff 0 do 
    i j3!
  loop
;

\ test
: j3.test
  j3.dir
  begin
    j3.count
  0 until
;

\ i2s protocol bit banging
\ register masks:
\  80    40    20   10
\ SDIN  BCK  LRCK  MCK

\ send 16 bit word samples
\ LRCK toggles before the LSB.

\ on entry, LRCK has been lowered already.
\ measurement of MCK: 1.55 us/cycle or 645kHz (spec .5 to 50 MHz)
\ input sample rate is 1/32 of that or 24kHz (spec 2 to 200 kHz)
: i2s.word ( d -- )
  0                     \ LRCK register on stack 
  2 0 do 
    over                \ init shift register on stack
    16 0 do

      \ register out on stack:
      over                 \ copy LRCK
      over 0< $80 and +     \ MSB to SDIN
      dup $0040 io!        \ write to register

      \ with clocks low, do some processing and kill time
      \ ( LRCK SHIFT J3REG )
      rot
      \ toggle LRCK at i=14
      i $e = if $20 xor then
      \ ( SHIFT J3REG LRCK )
      rot 
      2*                \ shift
      \ ( J3REG LRCK SHIFT )
      rot
      \ raise MCK and BCK
      $50 or $0040 io!
      \ kill time
      1 1+ 1+ 1+ drop \ seeing 50% duty cycle on MCLK 645 kHz
    loop
    drop                \ drop shift register
  loop

  drop                  \ drop LRCK register
  drop                  \ drop input 
;

\ 16-bit triangle wave
: i2s.test
  j3.dir
  begin
    $3fff 0 do
      i i2s.word
    loop
  0 until
;

