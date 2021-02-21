
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
    public class ProdutoController : ControllerBase
    {
        private const string EntityName = "produto";
        private readonly IProdutoRepository _produtoRepository;
        private readonly ILogger<ProdutoController> _log;

        public ProdutoController(ILogger<ProdutoController> log,
            IProdutoRepository produtoRepository)
        {
            _log = log;
            _produtoRepository = produtoRepository;
        }

        [HttpPost("produtos")]
        [ValidateModel]
        public async Task<ActionResult<Produto>> CreateProduto([FromBody] Produto produto)
        {
            _log.LogDebug($"REST request to save Produto : {produto}");
            if (produto.Id != 0)
                throw new BadRequestAlertException("A new produto cannot already have an ID", EntityName, "idexists");

            await _produtoRepository.CreateOrUpdateAsync(produto);
            await _produtoRepository.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProduto), new { id = produto.Id }, produto)
                .WithHeaders(HeaderUtil.CreateEntityCreationAlert(EntityName, produto.Id.ToString()));
        }

        [HttpPut("produtos")]
        [ValidateModel]
        public async Task<IActionResult> UpdateProduto([FromBody] Produto produto)
        {
            _log.LogDebug($"REST request to update Produto : {produto}");
            if (produto.Id == 0) throw new BadRequestAlertException("Invalid Id", EntityName, "idnull");
            await _produtoRepository.CreateOrUpdateAsync(produto);
            await _produtoRepository.SaveChangesAsync();
            return Ok(produto)
                .WithHeaders(HeaderUtil.CreateEntityUpdateAlert(EntityName, produto.Id.ToString()));
        }

        [HttpGet("produtos")]
        public async Task<ActionResult<IEnumerable<Produto>>> GetAllProdutos(IPageable pageable)
        {
            _log.LogDebug("REST request to get a page of Produtos");
            var result = await _produtoRepository.QueryHelper()
                .Include(produto => produto.Categoria)
                .GetPageAsync(pageable);
            return Ok(result.Content).WithHeaders(result.GeneratePaginationHttpHeaders());
        }

        [HttpGet("produtos/{id}")]
        public async Task<IActionResult> GetProduto([FromRoute] long id)
        {
            _log.LogDebug($"REST request to get Produto : {id}");
            var result = await _produtoRepository.QueryHelper()
                .Include(produto => produto.Categoria)
                .GetOneAsync(produto => produto.Id == id);
            return ActionResultUtil.WrapOrNotFound(result);
        }

        [HttpDelete("produtos/{id}")]
        public async Task<IActionResult> DeleteProduto([FromRoute] long id)
        {
            _log.LogDebug($"REST request to delete Produto : {id}");
            await _produtoRepository.DeleteByIdAsync(id);
            await _produtoRepository.SaveChangesAsync();
            return Ok().WithHeaders(HeaderUtil.CreateEntityDeletionAlert(EntityName, id.ToString()));
        }
    }
}
