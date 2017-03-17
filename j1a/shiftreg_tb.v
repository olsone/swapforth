module test;

  /* Make a reset that pulses once. */
  reg reset = 0;
  initial begin
     # 5 reset = 1;
     # 10 reset = 0;
     # 100 $stop;
  end

  /* Make a regular pulsing clock. */
  reg clk = 0;
  always #5 clk = !clk;
  reg  [7:0] value;
  wire [7:0] cnt;
  wire [7:0] out;
  
  shiftreg c1 (value, clk, reset, cnt, out);
  
  initial begin
     value = 8'haa;
  end
     
  initial begin
  
     $monitor("At time %t, value = %h (%0d)",
              $time, value, value);
    $dumpfile("shiftreg.vcd");
    $dumpvars(0, test);
  end
endmodule // test
