using System;
using System.Collections.Generic;

namespace Evalua
{
    /* 
    Requerimiento 1: Programar el residuo de la division en c# y asm (DX)  ------ SI
    Requeremiento 2: Programar en el ensamblador el scanf y printf (asm.WriteLine("ret") -------- SI
                                                                    asm.WriteLine("define_print_string");
                                                                    asm.WriteLine("define_print_num");
                                                                    asm.WriteLine("define_print_num_uns");
                                                                    asm.WriteLine("define_scan_num");)
                                                                    asm.WriteLine("include \"emu8086.inc\""); <=== Antes del ORG
    Requerimiento 3: Programar el >,>=,<,<= en ASM                         -------- SI
    Requerimiento 4: Programar el While en ASM                             -------- SI
    Requerimiento 5: Programar el else en ASM                              -------- SI
    */
    public class Lenguaje:Sintaxis
    {
        int contadorIF = 0;
        int contadorFor = 0;
        int contadorELSE = 0;
        int contadorWHILE = 0;
        List<Variable> LV ;
        Stack<float> SE; 
        List<string> LS;
        Variable.TDatos Dominante; //verifdicar si encontramos un int. char, float
        public Lenguaje()
        {
            LV = new List<Variable>();
            SE = new Stack<float>();
            LS = new List<string>();
        }

        // Programa	-> 	Librerias Variables Main
        public void Programa()
        {
            asm.WriteLine("include \"emu8086.inc\"");
            Librerias();
            
            Variables();
            asm.WriteLine("ORG 100h");
            Main();
            asm.WriteLine("ret");
            asm.WriteLine("define_print_string");
            asm.WriteLine("define_print_num");
            asm.WriteLine("define_print_num_uns");
            asm.WriteLine("define_scan_num");
            ImprimeLista();
            
        }
        // Librerias->	#include<identificador(.h)?> Librerias?
        private void Librerias()
        {
            Match("#");
            Match("include");
            Match("<");
            Match(Tipos.identificador);
            if(getContenido()==".")
            {
                Match(".");
                Match("h");
            }
            Match(">");
            if(getContenido()=="#")
            {
                Librerias();
            }
        }
        private Variable.TDatos StringtoEnum(string tipo)
        {
            switch(tipo){
                case "char": return Variable.TDatos.CHAR;
                case "int": return Variable.TDatos.INT;
                case "float": return Variable.TDatos.FLOAT;
                default : return Variable.TDatos.sinTipo;
            }
        }
        //Variables ->  tipoDato ListaIdentificadores; Variables?
        private void Variables()
        {
            Variable.TDatos tipo = Variable.TDatos.CHAR; //inicializamos
            tipo = StringtoEnum(getContenido()); //contiene el token int, char, float o uk
            Match(Tipos.tipoDato);
            ListaIdentificadores(tipo);
            Match(Tipos.finSentencia);
            if(getClasificacion()==Tipos.tipoDato) //recursividad de Variables
            {
                Variables();
            }
        }
        private void ImprimeLista() //Imprimir en el log
        {
            log.WriteLine("LISTA DE VARIABLES ");
            asm.WriteLine("; Variables");
            foreach(Variable L in LV){
                log.WriteLine(L.getNombre()+ " "+L.getTipoDato()+" "+L.getValor());
                asm.Write(L.getNombre()+ " ");
                switch(L.getTipoDato())
                {
                    case Variable.TDatos.CHAR: asm.WriteLine("db 0"); break;
                    case Variable.TDatos.INT: asm.WriteLine("dw 0"); break;
                    case Variable.TDatos.FLOAT: asm.WriteLine("dd 0"); break;
                }
            }
        }
        private bool Existe(string nombre) //verifica si hay variables duplicadas
        {
            foreach(Variable L in LV){
                if(L.getNombre() == nombre)
                {
                    return true;
                }
            }
            return false;
        }
        private void Modifica(string nombre, float valor) 
        {
            foreach(Variable L in LV){
                if(L.getNombre() == nombre)
                {
                   L.SetValor(valor);
                }
            }
        }
        private float GetValor(string nombre) 
        {
            foreach(Variable L in LV){
                if(L.getNombre() == nombre)
                {
                   return L.getValor();
                }
            }
            return 0;
        }

        private Variable.TDatos GetTipo(string nombre) 
        {
            foreach(Variable L in LV){
                if(L.getNombre() == nombre)
                {
                   return L.getTipoDato();
                }
            }
            return Variable.TDatos.sinTipo; //variables o variable
        }
        //ListaIdentificadores ->  identificador (,ListaIdentificadores)?
        private void ListaIdentificadores(Variable.TDatos tipo)
        {
            if(tipo != Variable.TDatos.sinTipo)
            {
                if(!Existe(getContenido())) 
                {
                    LV.Add(new Variable(getContenido(), tipo));
                }else{
                    throw new Error("ERROR DE SINTAXIS: VARIABLE DUPLICADA: " +getContenido(),linea,log);
                }
            }else
            {
                LS.Add(GetValor(getContenido()).ToString());
            }
            Match(Tipos.identificador);
            if(getContenido()==",")//recursividad de lista
            {
                Match(",");
                ListaIdentificadores(tipo);
            }
        }
        // Main  ->	void main() BloqueInstrucciones
        private void Main()
        {
            Match("void");
            Match("main");
            Match("(");
            Match(")");
            BloqueInstrucciones(true); //por que no hay instrucciones
        }
        // BloqueInstrucciones ->  {ListaInstrucciones}
        private void BloqueInstrucciones(bool ejecuta)
        {
            Match("{");
            ListaInstrucciones(ejecuta);
            Match("}");
        }
        // ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones(bool ejecuta)
        {
            Instruccion(ejecuta);
            if(getContenido() != "}") 
            {
                ListaInstrucciones(ejecuta);
            }
    
        }
        // Instrccion  -> Printf  | Scanf | If | For | While | Switch | Asignacion 
        private void Instruccion(bool ejecuta)
        {
            if(getContenido() == "printf")
            {
                Printf(ejecuta);
            }
            else if(getContenido() == "scanf")
            {
                Scanf(ejecuta);
            }
            else if(getContenido() == "if")
            {
                If(ejecuta);
            }
            else if(getContenido() == "for")
            {
                For(ejecuta);
            }
            else if(getContenido() == "while")
            {
                While(ejecuta);
            }
            else if(getContenido() == "switch")
            {
                Switch(ejecuta);
            }
            else if(getContenido() == "do")
            {
                doWhile(ejecuta);
            }
            else
            {
                Asignacion(ejecuta);
            }
        }
        // Printf -> printf(cadena (,ListaIdentificadores)?);
        private void Printf(bool ejecuta)
        {   
            Match("printf"); 
            Match("(");
            string cadenaPrint = getContenido();
            cadenaPrint = string.Join("", cadenaPrint.Split('"'));  //Borra comilla doble
            cadenaPrint = cadenaPrint.Replace("\\n","\n");          //Salto de linea 
            cadenaPrint = cadenaPrint.Replace("\\t", "\t");         //Tab de linea
            string[] cadenaPrint_2 = cadenaPrint.Split(' ');        //Creacion de Array con el contenido de mi String sin "", \n, \t y seprado por espacios
            Match(Tipos.Cadena);
            if(getContenido() == ",")
            {
                Match(",");
                LS.Clear();
                ListaIdentificadores(Variable.TDatos.sinTipo);
                if(cadenaPrint.Contains("%")){
                    int var = 0;
                    foreach(string L in LS)     //Se recorre L en LS
                    {
                        foreach(string cadena in cadenaPrint_2)
                        {
                            if(cadena.Contains("%"))
                            {
                                cadenaPrint_2[var] = cadenaPrint_2[var].Replace("%f",L); //Ingreso el contenido en L en el %f 
                                break; //Salgo el while por si reemplazo el %f 
                            }
                            var++;
                        }
                       var = 0; //Reseteo la variable var para el siguiente elementos L en LS
                    }
                }
            }
            Match(")");
            Match(Tipos.finSentencia);
            cadenaPrint = string.Join(" ", cadenaPrint_2);
            if(ejecuta)
            {
                Console.Write(cadenaPrint + " ");
                //asm.WriteLine("PRINT "+"\""+cadenaPrint+"\"");// Quizas dentro del IF
            }
            asm.WriteLine("PRINT "+"\""+cadenaPrint+"\"");// Quizas dentro del IF
        } 
        // Scanf -> scanf(cadena,ListadeAmpersas);
        private void Scanf(bool ejecuta)
        {
            Match("scanf");
            Match("(");
            Match(Tipos.Cadena);
            Match(",");
            ListadeAmpersas(ejecuta);
            Match(")");
            Match(Tipos.finSentencia);
        } 
        // ListadeAmpersas -> & identificador(,ListadeAmpersas)?
        private void ListadeAmpersas(bool ejecuta)
        {
            Match("&");
            if(!Existe(getContenido()))//Buscar en MATCH y si o existe lanzar exception
            {
                throw new Error("ERROR DE SINTAXIS: VARIABLE no declarada: " +getContenido(),linea,log);
            }
            asm.WriteLine("CALL SCAN_NUM");
            asm.WriteLine("MOV "+getContenido() +", CX");
            if(ejecuta)
            {
                string str = ""+Console.ReadLine();
                float valor = float.Parse(str); //Convierte el string --> Float
                Modifica(getContenido(),valor);
            }
            //Console.ReadLine();
            Match(Tipos.identificador);
            if(getContenido()==",")
            {
                Match(",");
                ListadeAmpersas(ejecuta);
            }
        }
        // If -> if(Condicion) BloqueInstrucciones | Intruccion (else BloqueInstrcciones | Instruccion)?
        private void If(bool ejecuta)
        {
            string etiqueta = "EIF"+contadorIF++;
            Match("if");
            Match("(");
            bool evalua = Condicion(etiqueta); //si es falsa o verdadera la condicion y se guarda en la variable bool
            //Console.WriteLine("\n"+evalua);
            Match(")");
            //asm.WriteLine(etiqueta +":");
            if(getContenido()!="{")
            {
                
                Instruccion(evalua && ejecuta);
            }
            else
            {
                BloqueInstrucciones(evalua && ejecuta);
            }
            if(getContenido()=="else")
            {
                string etiqueta_else = "EELSE"+contadorELSE++;
                Match("else");
                asm.WriteLine("JMP "+etiqueta_else);
                asm.WriteLine(etiqueta +":");
                if(getContenido()!="{")
                {
                    Instruccion(!evalua && ejecuta); //Verificar esto
                }
                else
                {
                    BloqueInstrucciones(!evalua && ejecuta);
                }
                asm.WriteLine(etiqueta_else +":");
                return;
            }
            asm.WriteLine(etiqueta +":");
        }
        // ***Condicion -> Expresion oprRelacional Expresion  
        private bool Condicion(string etiqueta)
        {
            //considerando la negacion 
            bool negacion = false;
            if(getContenido()=="!")
            {
                Match("!");
                negacion = true;
            }
            Expresion();
            string operador = getContenido();
            Match(Tipos.opRelacional);
            Expresion();
            float Resultado_2 = SE.Pop();
            asm.WriteLine("POP CX");
            float Resultado_1 = SE.Pop();
            asm.WriteLine("POP BX");
            asm.WriteLine("CMP CX, BX");
            //Evalua la condicion que regresa falso o verdadero
            if(negacion) // Req 3
            {
                switch(operador)
                {
                    case "==":  asm.WriteLine("JE "+etiqueta); return Resultado_1 != Resultado_2;  //Regresa booleano en todos los casos INVERTIDOS!!!
                    case ">=":  asm.WriteLine("JLE "+etiqueta); return Resultado_1 <= Resultado_2; 
                    case ">":   asm.WriteLine("JL "+etiqueta); return Resultado_1 < Resultado_2;  
                    case "<=":  asm.WriteLine("JGE "+etiqueta); return Resultado_1 >= Resultado_2; 
                    case "<":   asm.WriteLine("JG "+etiqueta); return Resultado_1 > Resultado_2;  
                    default:    asm.WriteLine("JNE "+etiqueta); return Resultado_1 == Resultado_2; 
                }
            }
            else
            {
                switch(operador)
                {
                    case "==":  asm.WriteLine("JNE "+etiqueta); return Resultado_1 == Resultado_2;  //Regresa booleano en todos los casos
                    case ">=":  asm.WriteLine("JGE "+etiqueta); return Resultado_1 >= Resultado_2; 
                    case ">":   asm.WriteLine("JG "+etiqueta); return Resultado_1 > Resultado_2;  
                    case "<=":  asm.WriteLine("JLE "+etiqueta); return Resultado_1 <= Resultado_2; 
                    case "<":   asm.WriteLine("JL "+etiqueta); return Resultado_1 < Resultado_2;  
                    default:    asm.WriteLine("JE "+etiqueta); return Resultado_1 != Resultado_2; 
                }
            }
        }
        // ***For -> for(identificador=Expresion; Condicion; identificador incTermino) BloqueInstrucciones | Instruccion
        private void For(bool ejecuta)
        {
            string EBeginFOR = "EBeginFOR"+contadorFor++;
            string EEndFOR = "EEndFor" + contadorFor;
            bool plus_minus = false;
            Match("for");
            Match("(");
            if(!Existe(getContenido())) 
            {
                throw new Error("ERROR DE SINTAXIS: VARIABLE NO DECLARADA: " +getContenido(),linea,log);
            }
            string variable = getContenido();
            Match(Tipos.identificador);
            Match("=");
            Expresion();
            float Resultado = SE.Pop();
            asm.WriteLine("POP AX");
            Modifica(variable, Resultado);
            asm.WriteLine("MOV "+variable +", AX");
            Match(Tipos.finSentencia);
            asm.WriteLine(EBeginFOR+":"); //Inicoi del FOR LOOP EN ASM
            bool evalua = Condicion(EEndFOR);
            Match(Tipos.finSentencia);
            string variable_2 = getContenido();
            if(!Existe(getContenido()))
            {
                throw new Error("ERROR DE SINTAXIS: VARIABLE NO DECLARADA: " +getContenido(),linea,log);
            }
            Match(Tipos.identificador);
            string operador = getContenido();
            Match(Tipos.incTermino);
            if(operador == "++"){
                Modifica(variable_2, GetValor(variable_2)+1);
                //asm.WriteLine("INC "+variable_2);
                plus_minus = true;
            }
            else if(operador == "--")
            {
                Modifica(variable_2, GetValor(variable_2)-1);
                //asm.WriteLine("DEC "+variable_2);
                plus_minus = false;
            }
            Modifica(variable_2, Resultado); //esto no!!!
            Match(")");
            if(getContenido()!="{")
            {
                Instruccion(evalua);
            }
            else
            {
                BloqueInstrucciones(evalua);
            }

            if(plus_minus)
            {
                asm.WriteLine("INC "+variable_2);
            }
            else if(!plus_minus)
            {
                asm.WriteLine("DEC "+variable_2);
            }
            asm.WriteLine("JMP "+EBeginFOR);
            asm.WriteLine(EEndFOR+":");
        }

        private void doWhile(bool ejecuta)
        {
            //int lineaArchivo = linea;
            Match("do");
            BloqueInstrucciones(true);
            Match("while");
            Match("(");
            bool evalua = Condicion("");
            Match(")");
            Match(";");
            //linea = linea - 7;
        }
        // While -> while(Condicion) BloqueInstrucciones | Instruccion
        private void While(bool ejecuta)
        {
            string EBeginWHILE = "EBeginWHILE"+contadorWHILE++;
            string EEndWHILE = "EEndWHILE"+contadorWHILE;
            
            Match("while");
            Match("(");
            if(!Existe(getContenido()))//Buscar en MATCH y si o existe lanzar exception
            {
                throw new Error("ERROR DE SINTAXIS: VARIABLE no declarada: " +getContenido(),linea,log);
            }
            
            asm.WriteLine(EBeginWHILE +":");
            bool evalua = Condicion(EEndWHILE);
            Match(")");
            if(getContenido()!="{") // bloque de instrucciones y instrucciones
            {
                Instruccion(evalua);
            }
            else
            {
                BloqueInstrucciones(evalua);
            }

            asm.WriteLine("JMP "+EBeginWHILE);
            asm.WriteLine(EEndWHILE+":");
        }
        // Switch -> switch(Expresion)  {VariosCase Default}
        private void Switch(bool ejecuta)
        {
            Match("switch");
            Match("(");
            if(!Existe(getContenido()))
            {
                throw new Error("ERROR DE SINTAXIS: VARIABLE NO DECLARADA" +getContenido(),linea,log);
            }
            Expresion();
            float Resultado = SE.Pop();
            Match(")");
            Match("{");
            VariosCase();
            Default();
            Match("}");
        }
        // VariosCase -> case numero: VariosCase | CaseInstruccion
        private void VariosCase()
        {
            Match("case");  
            Match(Tipos.numero);
            Match(":");
            if(getContenido() != "case")
            {
                CaseInstruccion();
            }
            else
            {
                VariosCase();
            } 
        }
        // CaseInstruccion  -> Instruccion| BloqueInstrucciones Break 
        private void CaseInstruccion()
        {
            if(getContenido()!="{")
            {
                Instruccion(true);
            }
            else
            {
                BloqueInstrucciones(true);
            }
            Break();
        }
        //Break -> break;? VariosCase
        private void Break()
        {
            if(getContenido() == "break")
            {
                Match("break");
                Match(Tipos.finSentencia);
                if(getContenido() == "case")
                {
                    VariosCase();
                }
            }
        } 
        // Default-> default:? Intruccion | BloqueInstrcciones 
        private void Default()
        {
            if(getContenido()=="default")
            {
                Match("default");
                Match(":");
                if(getContenido()!="{")
                {
                    Instruccion(true);
                }
                else
                {
                    BloqueInstrucciones(true);
                }
            }   
        }
        // Asignacion  -> identificador = Expresion;
        private void Asignacion(bool ejecuta)
        {
            Dominante = Variable.TDatos.sinTipo;
            string variable =  getContenido();
            Match(Tipos.identificador);
            if(getContenido() == "++")
            {
                string variable_2 = getContenido();
                Match(Tipos.incTermino);
                if(variable_2 == "++")
                {
                    Modifica(variable, GetValor(variable)+1);
                    asm.WriteLine("INC "+variable);
                }
                Match(Tipos.finSentencia);
            }
            else if(getContenido() == "--")
            {
                string variable_2 = getContenido();
                Match(Tipos.incTermino);
                if(variable_2 == "--")
                {
                    Modifica(variable, GetValor(variable)-1);
                    asm.WriteLine("DEC "+variable);
                }
                Match(Tipos.finSentencia);
            }
            else
            {
                Match(Tipos.asignacion);
                Expresion();
                float Resultado = SE.Pop(); //variable del resultado
                asm.WriteLine("POP AX");
                asm.WriteLine("MOV " +variable+ ", AX");
                if(Dominante < ValorToEnum(Resultado))
                {
                    Dominante = ValorToEnum(Resultado);
                }
                //exception
                if(Dominante <= GetTipo(variable))
                {
                    Modifica(variable, Resultado);
                }
                else
                {
                    throw new Error("ERROR DE SEMANTICA: La variable " + variable +" es de tipo " +GetTipo(variable)+" y se esta asignando un "+ Dominante,linea, log);
                }
                Match(Tipos.finSentencia);
            }
        }
        // Expresion  -> Termino MasTermino 
        private Variable.TDatos ValorToEnum(float valor) //Comparar rango de los char
        {
            if(valor % 1 != 0){
                return Variable.TDatos.FLOAT;
            }
            else if(valor < 256)
            {
                return Variable.TDatos.CHAR;
            }
            else if(valor < 65535)
            {
                return Variable.TDatos.INT;
            }
            return Variable.TDatos.FLOAT;
        }
        private void Expresion()
        {
            Termino();
            MasTermino();
        }
        // MasTermino -> (opTermino Termino)? 
        private void MasTermino()
        {
            if(getClasificacion()==Tipos.opTermino)
            {
                string operador = getContenido();
                Match(Tipos.opTermino);
                Termino();//segundo termino
                //Console.Write(operador+ " ");
                float N1 = SE.Pop(); //Sacamos el primer numero y el segundo del stack con operaciones
                asm.WriteLine("POP BX");
                float N2 = SE.Pop();
                asm.WriteLine("POP AX");
                switch(operador){
                    case "+":   SE.Push(N2+N1); 
                                asm.WriteLine("ADD AX, BX");
                                asm.WriteLine("PUSH AX"); 
                                break;

                    case "-":   SE.Push(N2-N1); 
                                asm.WriteLine("SUB AX, BX");
                                asm.WriteLine("PUSH AX");
                                break;

                }
            }
        }
        // Termino	  -> Factor PorFactor 
        private void Termino()
        {
            Factor();//reconoce el primer factor
            PorFactor();
        }
        // PorFactor  -> (opFactor Factor)?
        private void PorFactor()
        {
            if(getClasificacion()==Tipos.opFactor)
            {
                string operador = getContenido();
                Match(Tipos.opFactor);
                Factor();//reconoce el segundo factor
                //Console.Write(operador+ " ");
                float N1 = SE.Pop(); //Sacamos el primer numero y el segundo del stack con operaciones
                asm.WriteLine("POP BX");
                float N2 = SE.Pop();
                asm.WriteLine("POP AX");
                switch(operador){
                    case "*":   SE.Push(N2*N1); 
                                asm.WriteLine("MUL BX");
                                asm.WriteLine("PUSH AX"); 
                                break;
                    case "/":   SE.Push(N2/N1); 
                                asm.WriteLine("DIV BX");
                                asm.WriteLine("PUSH AX"); 
                                break;
                    case "%":   SE.Push(N2 % N1); 
                                asm.WriteLine("DIV BX");
                                asm.WriteLine("PUSH DX");
                                break;
                }
            }
        }
        // Factor	  -> numero | identificador | (Expresion) 
        private void Factor()
        {
            if(getClasificacion()==Tipos.numero)
            {
               //Console.Write(getContenido()+ " ");
                if(ValorToEnum(float.Parse(getContenido())) > Dominante)
                {
                    Dominante = ValorToEnum(float.Parse(getContenido())); //1
                    //Y Hacer lo mism con las variables
                }
                
                SE.Push(float.Parse(getContenido()));
                asm.WriteLine("MOV AX, "+getContenido());
                asm.WriteLine("PUSH AX");

                Match(Tipos.numero);
            }
            else if(getClasificacion()==Tipos.identificador)
            {
                //Console.Write(getContenido()+ " ");
                //float variableAyuda = GetValor(getContenido());
                if(!Existe(getContenido())) 
                {
                    throw new Error("ERROR DE SINTAXIS: VARIABLE no declarada: " +getContenido(),linea,log);
                }
                Variable.TDatos variableAyuda_2 = GetTipo(getContenido());
                if(variableAyuda_2 > Dominante)
                {
                    Dominante = variableAyuda_2;  //Requ 1 
                }
                SE.Push(GetValor(getContenido())); //metemos al stack la varible
                asm.WriteLine("MOV AX, "+getContenido());
                asm.WriteLine("PUSH AX");
                Match(Tipos.identificador);
            }
            else
            {
                bool huboCast = false;
                Variable.TDatos TipoCast = Variable.TDatos.sinTipo;
                Match("(");
                if(getClasificacion()==Tipos.tipoDato)
                {
                    huboCast = true;
                    TipoCast = StringtoEnum(getContenido()); //guardamos el contenido para el hubocast
                    Match(Tipos.tipoDato); //Modificar el dominante sin condicion
                    Match(")");
                    Match("(");
                }
                Expresion();
                Match(")");
                //si hubo CAST 
                if(huboCast){
                    float N1 = SE.Pop();
                    SE.Push(ValorCasteado(N1, TipoCast)); //desarrollar este metodo 65536 para un int y 256 para un char, casteo a flotante es sin  IF
                    Dominante = TipoCast;
                    //asm.WriteLine("PUSH");
                }//Requerimiento 4;
            }  
        }
        private float ValorCasteado(float N1, Variable.TDatos Cast){

            if(Cast == Variable.TDatos.CHAR)
            {
                N1 %= 256;
                return N1;
            }
            else if(Cast == Variable.TDatos.INT)
            {
                N1 %= 65536;
                return N1;
            }
            return N1; //Ahora hay que ingresarlo al stack
        }
    }
}