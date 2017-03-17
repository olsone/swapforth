module shiftreg(in, clk, reset, cnt, out);

  parameter WIDTH = 8;

  input  [WIDTH-1 : 0] in;
  input 	       clk, reset;
  output [WIDTH-1 : 0] cnt;
  output [WIDTH-1 : 0] out;

  reg [WIDTH-1 : 0]   cnt;
  reg [WIDTH-1 : 0]   out;
  wire 	       clk, reset;

  always @(posedge clk)
    assign cnt = cnt + 1;

  always @(negedge clk)
    out = out << 1;

  always @reset
    if (reset)
    begin
      assign cnt = 0;
      assign out = in;
    end
    else
      deassign cnt;

endmodule // shiftreg
