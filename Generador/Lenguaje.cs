namespace Generador
{

    //Requerimiento 1: Modificar la matriz TranD para poder comentar codigo de linea y multilinea
    //Requerimiento 2: La primera prodeccion debe ser publica
    public class Lenguaje:Sintaxis
    {
        //Gramatica produce -> Cabecera Producciones
        public void Gramatica()
        {
            Cabecera();
            Producciones();
        }
        //Cabecera -> Lenguaje: SNT
        private void Cabecera()
        {
            Match("Lenguaje");
            Match(":");
            gen.Write(getContenido());
            Match(Tipos.SNT);
            gen.WriteLine(getContenido());
            Match(Tipos.FinProduccion);
        }
        //producciones -> {ListaProducciones}
        private void Producciones()
        {
            Match("{");
            gen.WriteLine("{");
            gen.WriteLine("\tpublic class Lenguaje:Sintaxis");
            gen.WriteLine("\t{");
            ListaProducciones();
            gen.WriteLine("\t}");
            Match("}");
            gen.WriteLine("}");
        }
        //ListaProducciones -> Produccion; ListaProducciones?
        private void ListaProducciones()
        {
            Produccion();
            Match(Tipos.FinProduccion);
            if(getContenido() != "}")
            {
                ListaProducciones();
            }
        }
        //Produccion -> STN Flechita
        private void Produccion()
        {
            gen.WriteLine("\t\tprivate void " + getContenido()+ "()");
            gen.WriteLine("\t\t{");
            Match(Tipos.SNT);
            Match(Tipos.Flechita);
            gen.WriteLine("\t\t}");
            
        }
    }
}