module test;

  /* Make a reset that pulses once. */
  reg reset = 0;
  initial begin
     #5 si = 0;
     #10 si = 1;
     #15 si = 0;
     #20 si = 1;
     #25 si = 0;
     
     # 100 $stop;
  end

  reg  load = 0;
  reg [7:0] in;

  initial begin
     #0 in = 8'hae;
     #0 load = 1;
     #10 load = 0;
     
   end

  /* Make a regular pulsing clock. */
  reg clk = 0;
  always #5 clk = !clk;

  reg  si;
  wire so;
  shift c1 (clk, load, in, so);

  initial begin
    $dumpfile("shift.vcd");
    $dumpvars(0, test);
  end
endmodule // test
