// Instituo Tecnologico de Queretaro
// Jesus Chavez Arias
// -----------------------------------
// Contenido de la clase Lenguaje.cs: 
// 09:22:35 a. m.
// 13-05-2022
// -----------------------------------
using System;
namespace Cplusplus;
{
	public class Lenguaje:Sintaxis
	{
		public void X()
		{
			Librerias();
			Main();
		}
		private void Librerias()
		{
			Match("#");
			Match("include");
			Match("<");
			Match(Tipos.identificador);
			if(getContenido() == ".")
			{
				Match(".");
				Match("h");
			}
			Match(">");
			if(getContenido() == "#")
			{
				Match("#");
				Librerias();
			}
		}
		private void Main()
		{
			Match("void");
			Match("main");
			Match("(");
			Match(")");
			Match("{");
			if(getContenido() == "a")
			{
				Match("a");
				E();
				if(getClasificacion() == Tipos.identificador)
				{
					Match(Tipos.identificador);
				}
			}
			else if(getContenido() == "b")
			{
				Match("b");
				A();
			}
			else if(getClasificacion() == Tipos.numero)
			{
				Match(Tipos.numero);
				if(getContenido() == "#")
				{
					Match("#");
					R();
				}
			}
			Match(";");
			Match("?");
			Match("}");
		}
	}
}
