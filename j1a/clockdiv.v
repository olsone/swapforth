module clockdiv(clk, reset, divideby, clkout);

parameter WIDTH = 16;

input clk, reset;
input [WIDTH-1 : 0] divideby;

output clkout; 
 
reg [WIDTH-1:0] count; 
reg clkout;
   
  always @* 
    if (reset)
    begin
    	count <= 0;
    	clkout <= 0;
    end

  
  always @(clk)
    if (!reset)
    begin 
      count <= count + 1;
      if (count == divideby)
      begin
          clkout = !clkout;
          count <= 1;
      end
    end
endmodule // clockdiv
