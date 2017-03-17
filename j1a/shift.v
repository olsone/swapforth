module shift (clk, load, in, out); 
input clk;
input load;
input [7:0] in; 
output out; 
 
reg [7:0] tmp; 
 
  always @(posedge clk) 
    begin 
      tmp = tmp << 1; 
      if (load) 
      	tmp = in; 
    end 
    assign out = tmp[7]; 
endmodule 