using System.Linq.Expressions;
using tashrif.Data.Models;

namespace tashrif.Data.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task<IQueryable<T>> GetQueryable();
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(object id);
    Task AddAsync(T entity);
    void Update(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    void Delete(T entity);
    void DeleteRange(IEnumerable<T> entities);
    Task<IEnumerable<TResult>> GetAllAsync<TResult>(
        Expression<Func<T, TResult>> projection,
        bool ignoreGlobalQueryFilter = false,
        Expression<Func<T, bool>>? filter = null,
        Expression<Func<T, object?>>? orderBy = null);
    Task<TResult?> GetAsync<TResult>(
        Expression<Func<T, TResult>> projection,
        bool ignoreGlobalQueryFilter = false,
        Expression<Func<T, bool>>? filter = null,
        Expression<Func<T, object?>>? orderBy = null);
    Task<PaginationResultDto<TResult>> GetAllWithPaginationAsync<TResult>(
        PaginationDto pagination,
        Expression<Func<T, TResult>> projection,
        bool ignoreGlobalQueryFilter = false,
        Expression<Func<T, bool>>? filter = null,
        Expression<Func<T, object?>>? orderBy = null,
        Expression<Func<T, bool>>? paginationSearchFilter = null);
}
