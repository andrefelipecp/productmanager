
using System.Collections.Generic;
using System.Threading.Tasks;
using JHipsterNet.Core.Pagination;
using .Domain;
using .Crosscutting.Exceptions;
using .Domain.Repositories.Interfaces;
using .Web.Extensions;
using .Web.Filters;
using .Web.Rest.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace .Controllers
{
    [Authorize]
    [Route("api")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private const string EntityName = "categoria";
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly ILogger<CategoriaController> _log;

        public CategoriaController(ILogger<CategoriaController> log,
            ICategoriaRepository categoriaRepository)
        {
            _log = log;
            _categoriaRepository = categoriaRepository;
        }

        [HttpPost("categorias")]
        [ValidateModel]
        public async Task<ActionResult<Categoria>> CreateCategoria([FromBody] Categoria categoria)
        {
            _log.LogDebug($"REST request to save Categoria : {categoria}");
            if (categoria.Id != 0)
                throw new BadRequestAlertException("A new categoria cannot already have an ID", EntityName, "idexists");

            await _categoriaRepository.CreateOrUpdateAsync(categoria);
            await _categoriaRepository.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCategoria), new { id = categoria.Id }, categoria)
                .WithHeaders(HeaderUtil.CreateEntityCreationAlert(EntityName, categoria.Id.ToString()));
        }

        [HttpPut("categorias")]
        [ValidateModel]
        public async Task<IActionResult> UpdateCategoria([FromBody] Categoria categoria)
        {
            _log.LogDebug($"REST request to update Categoria : {categoria}");
            if (categoria.Id == 0) throw new BadRequestAlertException("Invalid Id", EntityName, "idnull");
            await _categoriaRepository.CreateOrUpdateAsync(categoria);
            await _categoriaRepository.SaveChangesAsync();
            return Ok(categoria)
                .WithHeaders(HeaderUtil.CreateEntityUpdateAlert(EntityName, categoria.Id.ToString()));
        }

        [HttpGet("categorias")]
        public async Task<ActionResult<IEnumerable<Categoria>>> GetAllCategorias(IPageable pageable)
        {
            _log.LogDebug("REST request to get a page of Categorias");
            var result = await _categoriaRepository.QueryHelper()
                .GetPageAsync(pageable);
            return Ok(result.Content).WithHeaders(result.GeneratePaginationHttpHeaders());
        }

        [HttpGet("categorias/{id}")]
        public async Task<IActionResult> GetCategoria([FromRoute] long id)
        {
            _log.LogDebug($"REST request to get Categoria : {id}");
            var result = await _categoriaRepository.QueryHelper()
                .GetOneAsync(categoria => categoria.Id == id);
            return ActionResultUtil.WrapOrNotFound(result);
        }

        [HttpDelete("categorias/{id}")]
        public async Task<IActionResult> DeleteCategoria([FromRoute] long id)
        {
            _log.LogDebug($"REST request to delete Categoria : {id}");
            await _categoriaRepository.DeleteByIdAsync(id);
            await _categoriaRepository.SaveChangesAsync();
            return Ok().WithHeaders(HeaderUtil.CreateEntityDeletionAlert(EntityName, id.ToString()));
        }
    }
}
