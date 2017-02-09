
variable flags

\ light error flags
\ ( x -- ) 
: flag 
    dup 0= if 0 flags ! then
    flags @ or 
    dup flags !
    leds ;

: green 1 flag ;
\ named after clock hours, board in left usb port
: red3 2 flag ;
: red6 16 flag ;
: red9 8 flag ;
: red12 4 flag ;



\ Direction bits: 00001010 (0 input, 1 output)

\ port addresses:
\ $0001 PMOD data register
\ $0002 PMOD direction register

\ ( x -- x) write and read 8 bits from SPI port
\ 8 lshift is to put the MSB in the sign bit
\ mask  2 is to mask the MOSI bit out of a $ffff flag
\ 
: spi
\    dup .x ." >spi> "
    8 lshift
    8 0 do
        dup 0< 2 and
        
        dup $0001 io!               
        8 + $0001 io!               
        2*                      
        $0001 io@ 4 and +   
    loop
    2/ 2/
;

: cs ( 0|1 --) 
    $0001 io! ;

\ to place the card into the SPI mode, set the MOSI and CS lines to logic value 1
\ and toggle SD CLK for at least 74 cycles.
: sd.reset 
   1 cs 
   20 0 do 
       $ff spi drop
   loop  ;

: sd.init
	$000b $0002 io! 
    
    sd.reset 
    0 flag
;

\ get 32 bits after status code
: read32 
        $ff spi
        8 lshift
        $ff spi
        +
        
        $ff spi
        8 lshift
        $ff spi
        +
        
        over .x dup .x
        cr
;

\ Send command to the mmc device followed by the data 
\ the data is packed into a 32 bit word, dummy CRC is 
\ used. NOTE send Most significant byte goes first 
\ ( 16data cmd -- nn) 
\ this was for a 32 bit word size. 
\ its going to transmit only 16 bits with 0s in the msb.
\ this seems benign for now
: cmd 
        $ff  spi  drop         
        $40  or  spi  drop   
        dup  24  rshift  $ff  and  spi  drop   
        dup  16  rshift  $ff  and  spi  drop   
        dup   8  rshift  $ff  and  spi  drop  
        $ff  and  spi  drop   
        $95  spi  drop        
        $ff  spi  drop
        $ff  spi              
;



\ 32 bit payload
\ ( d cmd -- n )
: dcmd
        $ff  spi  drop         
        $40  or  spi  drop   
\ data nnnn
        dup   8  rshift  $ff  and  spi  drop  
        $ff  and  spi  drop   
        dup   8  rshift  $ff  and  spi  drop  
        $ff  and  spi  drop   
\ crc
        $95  spi  drop        
        $ff  spi drop
        $ff  spi              
;

\ 32 bit payload
\ ( crc d cmd -- n )
: dcmdcrc 
        $ff  spi  drop         
        $40  or  spi  drop   
\ data nnnn
        dup   8  rshift  $ff  and  spi  drop  
        $ff  and  spi  drop   
        dup   8  rshift  $ff  and  spi  drop  
        $ff  and  spi  drop   
\ crc
        $ff  and  spi  drop        
        $ff spi drop
        $ff  spi              
;

512 constant blocklen

\ set block length, specified as a 16 bit number, normally 512 
: cmd16 ( u -- f )
        0  cs  
        16  cmd 1 and 1 = drop
        1  cs
;


\ ( -- f)
: cmd0
        0  cs                    
        0  0  cmd 
        ." cmd0 " dup .x cr
        1 =
        1 cs
;


\ ( -- nn nn f)
: cmd8
        0  cs          
        $87 $01aa $0000 8  dcmdcrc 
        ." cmd8 " dup .x
        1 =
        read32
        cr
        rot
        1 cs
;

\ repeat until no longer idle
\ ( -- nn nn f)
: cmd41
		0
		20 0 do
			0 cs
			$65 $0000 $0000 55 dcmdcrc read32 drop drop drop 
            $77 $0000 $4000 41 dcmdcrc
        	0= if drop i leave then
        	1 ms
    		1 cs
        loop
        dup
        0= if ." cmd41 failed to get idle " cr 
        else ." cmd41 success, tries= " dup . cr 
        then
;

\ ( -- nn nn f)
: cmd58
        0  cs          
        $75 0.  58 dcmdcrc 
        ." cmd58 " dup .x
        0 =
        read32
        cr
        rot
        1 cs
;

\ resets card and returns counter of how many times 
\ cmd1 was tried, if 0 then card was not reset. 
\ This is the main start up command when a card is inserted 
\ all commands can be performed after this. 
\ (---x) 
: sd.start 
        sd.init
        cmd0
        if
            1  ms     
    		cmd8 nip nip
    		if  
    			cmd41
    			if
    				cmd58 nip nip
    				if
    					blocklen cmd16
    				else
    				then
    			then
    		then
        else
        	." no card? " cr    
        then
;

\ wait for $fe
: result ( --- f )
        0  
        500 0 do
                $ff  spi  $fe  =  if  drop  -1  leave  then
        loop
;

\ emit only printable char, or space. 2 columns.
: chr.  ( n -- )
		32 emit
		dup 31 > over 128 < and
		if 
			emit 
		else 
			drop
			32 emit
		then 
;


: 2variable create 0 , 0 , ;

variable dbuff 510 allot

: d8  dbuff + c@ ;
: d16 dbuff + @ ;
: d32 dbuff + 2@ swap ;

2variable sector


: spaces 0 do 32 emit loop ;

\ print out dbuff sector 
: dumpsec (  --- )
	cr blocklen 0 do			
		i .x ." : "
		32 0 do
			j i + d16 .x
		2 +loop
		cr
		7 spaces
		32 0 do
		    j i + dup d8 chr.
		    1 + d8 chr.
		    32 emit
		2 +loop
		cr
	32 +loop  
;


\ surprise, j1 is little endian, but so is mbr.
: readsec ( d -- u u )
	2dup sector 2!
	0 cs
    17 dcmd
	0=
	if
		result
		if
			blocklen 0  
			do
				$ff spi dbuff i + c!
			loop
			read32 
			2drop
		else
			." read timeout " cr
		then
	else
		." read failed " cr
	then
	
	1 cs
;


446 constant ptable 
2variable part0lba
2variable part0len

variable secPerClus
variable numFATs
2variable secPerFAT
2variable rootClus

\ surprise, j1 is little endian
\ but so is the data on disk. so we agree.
: readmbr
	0.  readsec
	$1fe  d16 $aa55 = 
	if ." valid "
	else ." mbr not valid "
	then
	cr
;


\ access the partition table  
: part0 ( -- f )
	readmbr
	ptable $08 + d32 2dup part0LBA  2!
	ptable $0c + d32 2dup part0len  2!
	2swap ." part0 " d. 44 emit d.
	ptable $04 + d8 
	$b = if -1 else 0 ." not " then ." FAT32 " 
	cr
;


2variable fatBeginLBA
2variable clusBeginLBA


\ access the beginning of FAT32 filesystem
\ danger: using 16 bit multiply
: fat0
	part0lba 2@ readsec
	$0d d8  secPerClus !
	$10 d8  numFATs !
	$24 d32 secPerFAT 2!
	$2c d32 rootClus 2!
	$1fe d16 
	$aa55 = 
	if 
	     
	    part0LBA 2@ $0e d16 0 d+ fatBeginLBA 2!
	    
	    secPerFAT 2@ swap numFATs @ * swap 
	    fatBeginLBA 2@ d+ clusBeginLBA 2!
	    secPerClus @ 8 - 0=
	    if -1 
	    else 0 ." not 4k "
	    then 
	    
	else
	    0 ."  not valid "
	then
;	



\ current cluster
2variable cluster
: d- dnegate d+ ;

\ read the first sector of cluster
: readclus ( d -- )
  2dup cluster 2!
  rootClus 2@ d-
  d2* d2* d2*
  clusBeginLBA 2@ d+ 
  readsec
;

\ print directory from sector buff. return true if more records in next sector.
: dirsec  ( -- f_more )
  -1
  cr
  blocklen 0 do
    i d8 
    dup 0= if nip leave 
    then 
    $e5 = if 
    else
    	i $0b + d8 dup 
    	$02 and 
    	if 
    	    drop
    	    negate 
    	else .x
			i $1a + d16 .x
			i $14 + d16 .x
			i $1c + d32 d.
			
			11 0 do 
				i j + d8 chr.
				8 i = if 46 chr. then
			loop
			cr
		then
    then	
  32 +loop
  ;

: +1sec
	sector 2@ 1. d+ readsec
;  

\ cd 
\ lookup cluster from directory entry match in current sector and read that cluster.



\ TODO after 8 sectors, find next cluster
: root
  2. readclus
  begin dirsec while +1sec repeat
;



: fat.start
	part0
	if
		fat0
		if
			root
		then
	then
;

