using MesaSitec.Aplicacion.DTOs.Categorias;
using MesaSitec.Aplicacion.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesaSitec.Api.Controllers;

[Authorize]
public class CategoriasController : BaseApiController
{
    private readonly ICategoriaService _categoriaService;

    public CategoriasController(ICategoriaService categoriaService)
    {
        _categoriaService = categoriaService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoriaDto>>> ObtenerCategorias()
    {
        var tenantId = ObtenerTenantId();
        var categorias = await _categoriaService.ObtenerCategoriasActivasAsync(tenantId);
        return Ok(categorias);
    }
}
