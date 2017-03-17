module i2s(in, clk, reset, mclk, lrclk, sdin, sclk);

  parameter WIDTH = 16;

  input [WIDTH-1 : 0] in;
  input 	       clk, reset;
  output               mclk, lrclk, sdin, sclk;

  reg          mclk, lrclk, sdin, sclk;
  wire 	       clk, reset;
  reg  [WIDTH-1 : 0] value;
  
  // ratio of LRCLK/SCK external is 256
  reg [6 : 0 ] i;

  always @(clk)
  begin
	mclk = clk;
	sclk = (!reset & clk);
  end

  always @(posedge reset)
  begin
    begin
		lrclk = 0;
		i = 0;
		value = 0;
		sdin = 0;
    end
  end

  // lrclk changes on posedge clk
  always @(posedge clk)
  begin
    /* if (!reset) */
		begin
		  if (i==127) 
			begin
			  lrclk = !lrclk;
			end
		  i = i + 1;	
		end
  end
  
  // lrclk changes on negedge sclk.
  // SDIN must be stable on posedge CLK.
  // CS4344: MSB must appear at 2nd posedge CLK after LRCLK. 
  // this wouldn't work with 16x ratio where LSB is supposed to go in next frame.  
  always @(negedge clk)
  begin
    if (!reset) 
		begin
			if (i==0)
				begin
				  /*assign*/ value = in;
				end
			else 
				begin
				  /*assign*/ sdin = value[WIDTH-1]; // MSB
			  	  /*assign*/ value = value << 1;
				end
	
			
		end
  end

  
  
endmodule // i2s
