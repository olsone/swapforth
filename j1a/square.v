module square(clk, reset, period, out);

parameter WIDTH = 16;

input clk, reset;
input [WIDTH-1 : 0] period;

output out; 
 
reg [WIDTH-1:0] cnt; 
reg stage;
reg out;
   
  always @(posedge clk) 
    if (reset)
    begin
    	assign cnt = period;
    	assign stage = 0;
    end
    else
    begin 
      if (cnt == 0)
      begin
          assign stage = !stage;
          assign cnt = period;
      end
      
      assign cnt = cnt - 1;
      
      out = stage;
    end
endmodule // square
