namespace Evalua
{
    public class Variable
    {

        public enum TDatos{
            sinTipo, CHAR, INT, FLOAT, 
        }
        string Nombre;
        float Valor;
        TDatos TipoDato;        

        public Variable(string Nombre, TDatos TipoDato)
        {
            this.Nombre = Nombre;
            this.TipoDato = TipoDato;
            Valor = 0;
        }
        public void SetValor(float Valor)
        {
            this.Valor = Valor;
        }

        public string getNombre()
        {
            return this.Nombre;
        }
        public float getValor()
        {
            return this.Valor;
        }
        public TDatos getTipoDato()
        {
            return this.TipoDato;
        }
    }
}