using System;

namespace Evalua
{
    class Program
    {
        static void Main(string[] args)
        {
            Lenguaje L = new Lenguaje();
            //byte x = (byte)(300);
            try
            {
                
                //byte y = (byte)(255);

                L.Programa();
                /*while(!L.FinAchivo())
                {
                    L.NextToken();
                }*/
            }
            catch (Exception)
            {
               Console.WriteLine("Fin de Compilacion. Verifique el codigo");
            }
        }
           

    }
}