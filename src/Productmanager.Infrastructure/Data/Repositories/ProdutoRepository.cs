using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using JHipsterNet.Core.Pagination;
using JHipsterNet.Core.Pagination.Extensions;
using .Domain;
using .Domain.Repositories.Interfaces;
using .Infrastructure.Data.Extensions;

namespace .Infrastructure.Data.Repositories
{
    public class ProdutoRepository : GenericRepository<Produto>, IProdutoRepository
    {
        public ProdutoRepository(IUnitOfWork context) : base(context)
        {
        }

        public override async Task<Produto> CreateOrUpdateAsync(Produto produto)
        {
            bool exists = await Exists(x => x.Id == produto.Id);

            if (produto.Id != 0 && exists)
            {
                Update(produto);
            }
            else
            {
                _context.AddOrUpdateGraph(produto);
            }
            return produto;
        }
    }
}
