module i2s(in, clk, reset, mclk, lrclk, sdin, sclk);

  parameter WIDTH = 16;

  input [WIDTH-1 : 0] in;
  input 	       clk, reset;
  output               mclk, lrclk, sdin, sclk;

  reg [WIDTH-1 : 0]   out;

  reg          mclk, lrclk, sdin, sclk;
  wire 	       clk, reset;
  reg  [WIDTH-1 : 0] value;
  
  reg  [3 : 0] i;

  always @clk
  begin
    mclk <= clk;
    sclk <= clk;
  end


  always @(posedge clk)
  begin
    if (i==14) 
    begin
      lrclk <= !lrclk;
    end
    sdin <= value[0];
    i = i + 1;
    value <= value >> 1;
  end

  always @reset
    if (reset)
    begin
      assign lrclk = 0;
      assign value = 0;
      assign i = 0;
    end
    else
      deassign out;

endmodule // i2s
