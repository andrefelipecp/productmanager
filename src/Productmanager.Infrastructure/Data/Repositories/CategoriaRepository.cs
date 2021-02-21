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
    public class CategoriaRepository : GenericRepository<Categoria>, ICategoriaRepository
    {
        public CategoriaRepository(IUnitOfWork context) : base(context)
        {
        }

        public override async Task<Categoria> CreateOrUpdateAsync(Categoria categoria)
        {
            bool exists = await Exists(x => x.Id == categoria.Id);

            if (categoria.Id != 0 && exists)
            {
                Update(categoria);
            }
            else
            {
                _context.AddOrUpdateGraph(categoria);
            }
            return categoria;
        }
    }
}
