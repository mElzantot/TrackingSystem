using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace NanoHealthSuite.Data.Enums;

public abstract class BaseRepository<TContext, TEntity, TId> : IBaseRepository<TEntity, TId>
    where TContext : DbContext
    where TEntity : class, new()
{
    protected readonly TContext Context;
    private readonly Action<TEntity, TId> _idSetter;

    protected BaseRepository(TContext context, Action<TEntity, TId> idSetter)
    {
        Context = context;
        _idSetter = idSetter;
    }

    public async Task<List<TResult>> GetAllAsync<TResult>(
        [NotNull] Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        bool asNoTracking = true,
        int? skip = null,
        int? limit = null,
        List<(Expression<Func<TEntity, object>> KeySelector, NanoHealthSuite.Data.Enums.SortingDirection
            SortingDirection)>? sortings = null)
    {
        IQueryable<TEntity> query = Context.Set<TEntity>();

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        if (sortings != null && sortings.Any())
        {
            var orderedQuery = sortings[0].SortingDirection switch
            {
                SortingDirection.Ascending => query.OrderBy(sortings[0].KeySelector),
                SortingDirection.Descending => query.OrderByDescending(sortings[0]
                    .KeySelector),
                _ => throw new NotSupportedException(),
            };

            for (int i = 0; i < sortings.Count; i++)
            {
                var item = sortings[i];
                orderedQuery = item.SortingDirection switch
                {
                    SortingDirection.Ascending => orderedQuery.ThenBy(item.KeySelector),
                    SortingDirection.Descending => orderedQuery.ThenByDescending(
                        item.KeySelector),
                    _ => throw new NotSupportedException(),
                };
            }

            query = orderedQuery;
        }

        if (skip != null && limit != null)
        {
            query = query.Skip(skip.Value).Take(limit.Value);
        }

        return await query
            .Select(selector)
            .ToListAsync();
    }

    public async Task<List<TResult>> GetAllAsync<TResult>(
        [NotNull] Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate,
        bool asNoTracking = true,
        int? skip = null,
        int? limit = null,
        List<(Expression<Func<TEntity, object>> KeySelector, SortingDirection SortingDirection)>? sortings = null,
        params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = Context.Set<TEntity>();

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        if (sortings != null)
        {
            foreach (var item in sortings)
            {
                query = item.SortingDirection switch
                {
                    SortingDirection.Ascending => query.OrderBy(item.KeySelector),
                    SortingDirection.Descending => query.OrderByDescending(item.KeySelector),
                    _ => throw new NotSupportedException(),
                };
            }
        }

        if (skip != null && limit != null)
        {
            query = query.Skip(skip.Value).Take(limit.Value);
        }

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query
            .Select(selector)
            .ToListAsync();
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null)
    {
        var query = Context.Set<TEntity>().AsNoTracking();

        return predicate != null ? await query.CountAsync(predicate) : await query.CountAsync();
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? predicate = null)
    {
        var query = Context.Set<TEntity>().AsNoTracking();

        return predicate != null ? await query.AnyAsync(predicate) : await query.AnyAsync();
    }

    public async Task<TResult?> GetFirstAsync<TResult>(
        [NotNull] Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        bool asNoTracking = true)
    {
        IQueryable<TEntity> query = Context.Set<TEntity>();

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        return await query
            .Select(selector)
            .FirstOrDefaultAsync();
    }

    public async Task<TResult?> GetSingleAsync<TResult>(
        [NotNull] Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate,
        bool asNoTracking = true,
        params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = Context.Set<TEntity>();

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query
            .Select(selector)
            .SingleOrDefaultAsync();
    }

    public async Task AddAsync(TEntity entity)
    {
        await Context.Set<TEntity>().AddAsync(entity);

        await Context.SaveChangesAsync();

        //Detach entity from context so that it can be used again to update
        Context.Entry(entity).State = EntityState.Detached;
    }

    public async Task AddRangeAsync(List<TEntity> entities)
    {
        await Context.Set<TEntity>().AddRangeAsync(entities);

        await Context.SaveChangesAsync();

        foreach (var entity in entities)
        {
            //Detach entity from context so that it can be used again to update
            Context.Entry(entity).State = EntityState.Detached;
        }
    }

    public async Task ForEachUpdateAsync(IEnumerable<TId> entityIds, Action<TEntity> updateExpression)
    {
        foreach (var id in entityIds)
        {
            TEntity entity = new();
            _idSetter.Invoke(entity, id);
            Context.Attach(entity);

            updateExpression.Invoke(entity);
        }

        await Context.SaveChangesAsync();
        foreach (var id in entityIds)
        {
            TEntity entity = new();
            _idSetter.Invoke(entity, id);

            //Detach entity from context so that it can be used again to update
            Context.Entry(entity).State = EntityState.Detached;
        }
    }

    public async Task UpdateAsync(TId id, Action<TEntity> updateExpression)
    {

        TEntity entity = new();
        _idSetter.Invoke(entity, id);
        Context.Attach(entity);

        updateExpression.Invoke(entity);

        await Context.SaveChangesAsync();

        //Detach entity from context so that it can be used again to update
        Context.Entry(entity).State = EntityState.Detached;
    }

    public async Task DeleteAsync(TEntity entity)
    {
        Context.Attach(entity);
        Context.Remove(entity);
        await Context.SaveChangesAsync();
    }

    public async Task DeleteRangeAsync(List<TEntity> entities)
    {
        Context.AttachRange(entities);
        Context.RemoveRange(entities);
        await Context.SaveChangesAsync();
    }
}
