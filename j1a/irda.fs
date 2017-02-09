: x
begin
$2000 io@ \ read from input port
8 and 0= \ true if bit 3 (IrDA RXD) is 0
1 and
$0004 io! \ write to LEDS
again
;

