using BinaryNetworks.Application.Interfaces.Persistence;
using BinaryNetworks.Domain.Entities;
using BinaryNetworks.Infrastructure.Persistence;

namespace BinaryNetworks.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;
    
    public IRepository<BinaryNetwork> BinaryNetworks { get; }

    public UnitOfWork(ApplicationDbContext dbContext, IRepository<BinaryNetwork> binaryNetworks)
    {
        _dbContext = dbContext;
        BinaryNetworks = binaryNetworks;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    public int SaveChanges()
    {
        return _dbContext.SaveChanges();
    }
}