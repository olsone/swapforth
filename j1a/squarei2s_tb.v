module test;

  /* Make a reset that pulses once. */
  reg reset = 0;
  initial begin
    period = 2;
    #1 reset = 1;
    #512 reset = 0;     
    # 2560 $stop;
  end



  initial begin

   end

  /* Make a regular pulsing clock. */
  reg clk = 0;
  always #1 clk = !clk;

  reg [15:0] period;
  wire out;
  wire mclk, lrclk, sdin, sclk;
  
  squarei2s top (clk, reset, period, mclk, lrclk, sdin, sclk);

  initial begin
    $dumpfile("squarei2s.vcd");
    $dumpvars(0, test);
  end
endmodule // test
