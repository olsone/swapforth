module test;

  /* Make a reset that pulses once. */
  reg reset = 0;
  initial begin
    divideby = 2;
    #0 reset = 1;
    #5 reset = 0;     
    # 100 $stop;
  end



  initial begin

   end

  /* Make a regular pulsing clock. */
  reg clk = 0;
  always #5 clk = !clk;

  reg [15:0] divideby;
  wire out;
  
  clockdiv c1 (clk, reset, divideby, clkout);

  initial begin
    $dumpfile("clockdiv.vcd");
    $dumpvars(0, test);
  end
endmodule // test
