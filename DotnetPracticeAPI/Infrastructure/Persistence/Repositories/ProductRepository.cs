using Domain.Entities;
using DotnetPracticeAPI.Application.Interfaces.Repositories;
using Infrastructure.Persistence;

namespace DotnetPracticeAPI.Infrastructure.Persistence.Repositories
{
    public class ProductRepository : GenericRepository<Product> , IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {

        }
    }
}
