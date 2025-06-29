using Domain.Entities;
using DotnetPracticeAPI.Application.Interfaces.Repositories;
using Infrastructure.Persistence;

namespace DotnetPracticeAPI.Infrastructure.Persistence.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {

        }
    }
}
