using System.IO;
using System;

namespace Generador
{
    public class Lexico:Token
    {
        protected int linea;
        const int  E=-2;//negativo por si crece el automata 
        const int  F=-1;
        int[,] TranD =
        {
        //  WS	L	-  >	?	;	|	(	)	\ Lamda

            {0, 1,  2,11,   4,  5,  6,  7,  8,  9,11,},
            {F, 1,  F,  F,  F,  F,  F,  F,  F,  F,  F,},
            {F, F,  F,  3,  F,  F,  F,  F,  F,  F,  F,},
            {F, F,  F,  F,  F,  F,  F,  F,  F,  F,  F,},
            {F, F,  F,  F,  F,  F,  F,  F,  F,  F,  F,},
            {F, F,  F,  F,  F,  F,  F,  F,  F,  F,  F,},
            {F, F,  F,  F,  F,  F,  F,  F,  F,  F,  F,},
            {F, F,  F,  F,  F,  F,  F,  F,  F,  F,  F,},
            {F, F,  F,  F,  F,  F,  F,  F,  F,  F,  F,},
            {F, F,  F,  F,10,10,10,10,10,   F,  F,},
            {F, F,  F,  F,  F,  F,  F,  F,  F,  F,  F,},
            {F, F,  F,  F,  F,  F,  F,  F,  F,  F,  F,},
        };
        private StreamReader archivo;
        protected StreamWriter log;
        protected StreamWriter gen;
        public Lexico()
        {
            linea=1;
            archivo = new StreamReader("D:\\Archivos\\Gramatica2.txt");
            log = new StreamWriter("D:\\Archivos\\Lenguaje.Log");
            gen = new StreamWriter("D:\\Archivos\\Lenguaje.cs");
            log.AutoFlush = true;
            gen.AutoFlush = true;
            log.WriteLine("Instituo Tecnologico de Queretaro");
            log.WriteLine("Jesus Chavez Arias");
            log.WriteLine("-----------------------------------");
            log.WriteLine("Contenido de Prueba.cs: ");
            string time = DateTime.Now.ToString("hh:mm:ss tt"); // hora
            string Date = DateTime.Now.ToString("dd-MM-yyyy"); //fecha 
            log.WriteLine(time);
            log.WriteLine(Date);
            log.WriteLine("-----------------------------------");

            gen.WriteLine("// Instituo Tecnologico de Queretaro");
            gen.WriteLine("// Jesus Chavez Arias");
            gen.WriteLine("// -----------------------------------");
            gen.WriteLine("// Contenido de la Clase Lenguaje.cs: ");
            
            gen.WriteLine("// "+time);
            gen.WriteLine("// "+Date);
            gen.WriteLine("// -----------------------------------");

            gen.WriteLine("using System;");
            gen.Write("namespace ");
            
        }
        public void CerrarArchivos()
        {
            archivo.Close();
            log.Close();
            gen.Close();
        }
        private int Columna(char Trancision)
        {
            //  WS	L	-  >	?	;	|	(	)	\ Lamda
            if(char.IsWhiteSpace(Trancision))
            {
                return 0;
            }else if(char.IsLetter(Trancision))
            {
                return 1;
            }else if(Trancision == '-')
            {
                return 2;
            }else if(Trancision == '>')
            {
                return 3;
            }else if(Trancision == '?')
            {
                return 4;
            }else if(Trancision == ';')
            {
                return 5;
            }else if(Trancision == '|')
            {
                return 6;
            }else if(Trancision == '('){
                return 7;
            }else if(Trancision == ')')
            {
                return 8;
            }else if(Trancision == '\\')
            {
                return 9;
            }
            return 10;
        }

        private void Clasifica(int Estado)
        {
            switch(Estado)
            {
                case 1: break;
                case 2: break;
                case 9: case 11: setClasificacion(Tipos.ST);break;
                case 3: setClasificacion(Tipos.Flechita);break;
                case 4: setClasificacion(Tipos.Epsilon);break;
                case 5: setClasificacion(Tipos.FinProduccion);break;
                case 6: setClasificacion(Tipos.Or);break;
                case 7: setClasificacion(Tipos.PIzquierdo);break;
                case 8: setClasificacion(Tipos.PDerecho);break;
            }
        }
        public void NextToken()
        {
            char c;
            string Buffer="";
            int Estado=0;
            while(Estado>=0)
            {
                c=(char)archivo.Peek();
                Estado=TranD[Estado,Columna(c)]; 
                if(Estado>=0)
                {
                    archivo.Read();
                    if(c=='\n')// para contar lineas 
                    { 
                        linea++;
                    }
                    if(Estado>0)
                    { 
                        Clasifica(Estado);
                        Buffer+=c;
                    }
                    else
                    { 
                        Buffer=""; //cadena vacia
                    }
                }
            }
            if(Estado == E)
            {
                
            }
            if(getClasificacion() == Tipos.ST)
            {
                if(char.IsUpper(Buffer[0]))
                {
                    setClasificacion(Tipos.SNT);
                }
            }
            setContenido(Buffer);
            
            if(!FinAchivo())
            {
                log.WriteLine(getContenido()+" = "+getClasificacion());
            }
        }
        public bool FinAchivo()
        {
            return archivo.EndOfStream;
        }
    }
}