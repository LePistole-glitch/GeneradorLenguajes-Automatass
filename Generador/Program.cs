using System;

namespace Generador
{
    class Program
    {
        static void Main(string[] args)
        {
            Lenguaje L = new Lenguaje();
            try
            {
                L.Gramatica();
            }
            catch (Exception)
            {
               Console.WriteLine("Fin de Compilacion. Verifique el codigo");
            }
        }
           

    }
}