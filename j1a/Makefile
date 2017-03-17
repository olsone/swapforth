build/ram.v: build/nuc.hex mkrom.py
	python mkrom.py

build/nuc.hex: cross.fs basewords.fs nuc.fs
	gforth cross.fs basewords.fs nuc.fs

bootstrap: build/nuc.hex mkrom.py verilator/bootstrap
	make -C verilator
	(cd ./verilator/; ./bootstrap)
	python mkrom.py

clean:
	rm -f build/nuc.hex
	make -C icestorm clean
	make -C verilator clean

UNAME_S := $(shell uname -s)
ifeq ($(UNAME_S),Linux)

connect: /dev/ttyUSB1
	sudo python shell.py -h /dev/ttyUSB1 -p ../common/

j4a:
	make -C icestorm j4a

j1a8k:
	make -C icestorm j1a8k

j1a-iceblink:
	make -C icestorm j1a-iceblink

j1a:
	make -C icestorm j1a

endif

ifeq ($(UNAME_S),Darwin)

/dev/tty.usbserial-*B: mackextload

connect: /dev/tty.usbserial-*B
	sudo python shell.py -h /dev/tty.usbserial-*B -p ../common/

j4a: mackextunload
	make -C icestorm j4a

j1a8k: mackextunload
	make -C icestorm j1a8k

j1a: mackextunload
	make -C icestorm j1a

mackextunload:
	sudo kextunload -bundle-id com.apple.driver.AppleUSBFTDI

mackextload:
	sudo kextload -bundle-id com.apple.driver.AppleUSBFTDI

endif

.PHONY: connect clean bootstrap mackextload mackextunload macconnect

# my verilog testbench targets

i2s: i2s_tb.v i2s.v
	iverilog -o i2s i2s_tb.v i2s.v
	vvp -n i2s 
	open -a gtkwave i2s.vcd


counter: counter_tb.v counter.v
	iverilog -o counter counter_tb.v counter.v
	vvp -n counter 
	open -a gtkwave counter.vcd

shiftreg: shiftreg_tb.v shiftreg.v
	iverilog -o shiftreg shiftreg_tb.v shiftreg.v
	vvp -n shiftreg 
	open -a gtkwave shiftreg.vcd

shift: shift_tb.v shift.v
	iverilog -o shift shift_tb.v shift.v
	vvp -n shift 
	open -a gtkwave shift.vcd
	
	
clockdiv: clockdiv_tb.v clockdiv.v
	iverilog -o clockdiv clockdiv_tb.v clockdiv.v
	vvp -n clockdiv 
	open -a gtkwave clockdiv.vcd
	
	
square: square_tb.v square.v
	iverilog -o square square_tb.v square.v
	vvp -n square 
	open -a gtkwave square.vcd
	
	
squarei2s: squarei2s_tb.v squarei2s.v clockdiv.v i2s.v
	iverilog -o squarei2s squarei2s_tb.v squarei2s.v clockdiv.v i2s.v
	vvp -n squarei2s 
	open -a gtkwave squarei2s.vcd
	
	


