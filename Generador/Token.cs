namespace Generador
{
    public class Token
    {
        public enum Tipos{
            SNT, ST, Flechita, Epsilon, FinProduccion, PIzquierdo, PDerecho, Or,
        }
        string Contenido;
        Tipos Clasificacion;
        public void setContenido(string Contenido)
        {
            this.Contenido=Contenido;
        }
        public void setClasificacion(Tipos Clasificacion)
        {
            this.Clasificacion=Clasificacion;
        }  
        public string getContenido()
        {
            return Contenido;
        }     
        public Tipos getClasificacion()
        {
            return Clasificacion;
        }
    }
}