using System.Linq.Expressions;

namespace NanoHealthSuite.Data.Enums;

public interface IBaseRepository<TEntity, in TId> where TEntity : new()
{
    Task<List<TResult>> GetAllAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        bool asNoTracking = true,
        int? skip = null,
        int? limit = null,
        List<(Expression<Func<TEntity, object>> KeySelector, SortingDirection SortingDirection)>? sortings = null);

    Task<List<TResult>> GetAllAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate,
        bool asNoTracking = true,
        int? skip = null,
        int? limit = null,
        List<(Expression<Func<TEntity, object>> KeySelector, SortingDirection SortingDirection)>? sortings = null,
        params Expression<Func<TEntity, object>>[] includes);

    Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null);
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? predicate = null);

    Task<TResult?> GetFirstAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        bool asNoTracking = true);

    Task<TResult?> GetSingleAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate,
        bool asNoTracking = true,
        params Expression<Func<TEntity, object>>[] includes);

    Task AddAsync(TEntity entity);

    Task AddRangeAsync(List<TEntity> entities);

    Task ForEachUpdateAsync(
        IEnumerable<TId> entityIds,
        Action<TEntity> updateExpression);

    Task UpdateAsync(
        TId entityId,
        Action<TEntity> updateExpression);

    Task DeleteAsync(TEntity entity);

    Task DeleteRangeAsync(List<TEntity> entities);
}
