namespace MesaSitec.Aplicacion.DTOs.Solicitudes;

public class ResultadoPaginadoDto<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int Total { get; set; }
    public int TotalPaginas { get; set; }
}
