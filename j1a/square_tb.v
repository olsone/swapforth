module test;

  /* Make a reset that pulses once. */
  reg reset = 0;
  initial begin
    period = 3;
    #0 reset = 1;
    #10 reset = 0;     
    # 100 $stop;
  end



  initial begin

   end

  /* Make a regular pulsing clock. */
  reg clk = 0;
  always #5 clk = !clk;

  reg [15:0] period;
  wire out;
  
  square c1 (clk, reset, period, out);

  initial begin
    $dumpfile("square.vcd");
    $dumpvars(0, test);
  end
endmodule // test
