// Instituo Tecnologico de Queretaro
// Jesus Chavez Arias
// -----------------------------------
// Contenido de la clase Program.cs: 
// 09:22:35 a. m.
// 13-05-2022
// -----------------------------------
using System;
namespace Cplusplus
{
	class Program
	{
		static void Main(string[] args)
		{
			Lenguaje L = new Lenguaje();
			try
			{
				L.X();
			}
			catch (Exception)
			{
				Console.WriteLine("Fin de Compilacion. Verifique el codigo");
			}
		}
	}
}
