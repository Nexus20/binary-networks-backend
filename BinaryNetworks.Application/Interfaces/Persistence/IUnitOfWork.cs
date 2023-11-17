using BinaryNetworks.Domain.Entities;

namespace BinaryNetworks.Application.Interfaces.Persistence;

public interface IUnitOfWork
{
    IRepository<BinaryNetwork> BinaryNetworks { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    int SaveChanges();
}