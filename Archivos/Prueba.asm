; Instituo Tecnologico de Queretaro
; Jesus Chavez Arias
; -----------------------------------
; Contenido de Prueba.asm: 
; 09:35:31 a. m.
; 28-03-2022
; -----------------------------------
include "emu8086.inc"
ORG 100h
PRINT "maximo= "
CALL SCAN_NUM
MOV maximo, CX
MOV AX, 0
PUSH AX
POP AX
MOV i, AX
EBeginFOR0:
MOV AX, i
PUSH AX
MOV AX, maximo
PUSH AX
POP CX
POP BX
CMP CX, BX
JE EEndFor1
MOV AX, i
PUSH AX
POP AX
MOV j, AX
EBeginWHILE0:
MOV AX, j
PUSH AX
MOV AX, 0
PUSH AX
POP CX
POP BX
CMP CX, BX
JE EEndWHILE1
MOV AX, j
PUSH AX
MOV AX, 2
PUSH AX
POP BX
POP AX
DIV BX
PUSH DX
MOV AX, 0
PUSH AX
POP CX
POP BX
CMP CX, BX
JNE EIF0
PRINT "+"
EIF0:
DEC j
JMP EBeginWHILE0
EEndWHILE1:
PRINT "."
INC i
JMP EBeginFOR0
EEndFor1:
ret
define_print_string
define_print_num
define_print_num_uns
define_scan_num
; Variables
i dw 0
j dw 0
maximo dw 0
