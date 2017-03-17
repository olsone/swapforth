module test;

  /* Make a reset that pulses once. */
  reg reset = 0;
  initial begin
     value = 16'haa00;
     # 0 reset = 1;
     # 2 reset = 0;
     # 100 value = 16'hff00; // should not be reflected in sdin until #1280
     # 512 $stop;
  end

  /* Make a regular pulsing clock. */
  reg clk = 0;
  always #1 clk = !clk;

  reg [15:0] value;
  wire mclk, lrclk, sdin, sclk;
  i2s c1 (value, clk, reset, mclk, lrclk, sdin, sclk);

  initial
  begin
     $monitor("At time %t, value = %h (%0d) %d %d %d %d",
              $time, value, value, mclk, lrclk, sdin, sclk);
  end

  initial begin
    $dumpfile("i2s.vcd");
    $dumpvars(0, test);
  end
endmodule // test
