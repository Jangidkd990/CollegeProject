namespace PRISM.Data.Repositories;

using Microsoft.EntityFrameworkCore;
using PRISM.Core.Entities;
using PRISM.Core.Interfaces;
using System.Linq.Expressions;

/// <summary>
/// Generic repository implementation for CRUD operations
/// </summary>
/// <typeparam name="T">Entity type that derives from BaseEntity</typeparam>
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext _dbContext;

    public Repository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbContext.Set<T>().FindAsync(id);
    }

    public virtual async Task<IReadOnlyList<T>> ListAllAsync()
    {
        return await _dbContext.Set<T>().ToListAsync();
    }

    public virtual async Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbContext.Set<T>().Where(predicate).ToListAsync();
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        _dbContext.Set<T>().Add(entity);
        await _dbContext.SaveChangesAsync();
        return entity;
    }

    public virtual async Task UpdateAsync(T entity)
    {
        entity.ModifiedAt = DateTime.UtcNow;
        _dbContext.Entry(entity).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(T entity)
    {
        // Soft delete
        entity.IsDeleted = true;
        entity.ModifiedAt = DateTime.UtcNow;
        await UpdateAsync(entity);
    }

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbContext.Set<T>().Where(predicate).CountAsync();
    }
}