namespace MesaSitec.Dominio.Excepciones;

public class ReglaNegocioException : Exception
{
    public string Codigo { get; }
    public int StatusCode { get; }

    public ReglaNegocioException(string codigo, string mensaje, int statusCode = 422) 
        : base(mensaje)
    {
        Codigo = codigo;
        StatusCode = statusCode;
    }
}
