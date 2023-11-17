using System.Linq.Expressions;

namespace BinaryNetworks.Application.Specifications.Abstract;

public interface IFilterSpecification<TEntity>
{
    Expression<Func<TEntity, bool>> Criteria { get; }
}