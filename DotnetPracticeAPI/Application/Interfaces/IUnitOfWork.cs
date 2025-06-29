using DotnetPracticeAPI.Application.Interfaces.Repositories;

namespace Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IProductRepository Products { get; }
        Task<int> SaveChangesAsync();
    }
}
