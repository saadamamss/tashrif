namespace tashrif.Repository;
public class GenericRepository<T>(tashrifDBContext context) : IGenericRepository<T> where T : class
{
    #region Fields & Properties

    private readonly tashrifDBContext _context = context;

    #endregion

    #region Functions

    public async Task<IQueryable<T>> GetQueryable()
    {
        return await Task.FromResult(_context.Set<T>().AsNoTracking().AsQueryable());
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _context.Set<T>().AsNoTracking().ToListAsync();
    }

    public async Task<T?> GetByIdAsync(object id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task AddAsync(T entity) => await _context.Set<T>().AddAsync(entity);

    public void Update(T entity) => _context.Set<T>().Update(entity);

    public async Task AddRangeAsync(IEnumerable<T> entities) => await _context.Set<T>().AddRangeAsync(entities);

    public void Delete(T entity) => _context.Set<T>().Remove(entity);

    public void DeleteRange(IEnumerable<T> entities) => _context.Set<T>().RemoveRange(entities);

    public async Task<IEnumerable<TResult>> GetAllAsync<TResult>(
        Expression<Func<T, TResult>> projection,
        bool ignoreGlobalQueryFilter = false,
        Expression<Func<T, bool>>? filter = null,
        Expression<Func<T, object?>>? orderBy = null)
    {
        var query = _context.Set<T>().AsNoTracking().AsQueryable();

        if (ignoreGlobalQueryFilter)
        {
            query = query.IgnoreQueryFilters();
        }

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (orderBy != null)
        {
            query = query.OrderBy(orderBy);
        }

        return await query.Select(projection).ToListAsync();
    }

    public async Task<TResult?> GetAsync<TResult>(
        Expression<Func<T, TResult>> projection,
        bool ignoreGlobalQueryFilter = false,
        Expression<Func<T, bool>>? filter = null,
        Expression<Func<T, object?>>? orderBy = null)
    {
        var query = _context.Set<T>().AsNoTracking().AsQueryable();

        if (ignoreGlobalQueryFilter)
        {
            query = query.IgnoreQueryFilters();
        }

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (orderBy != null)
        {
            query = query.OrderBy(orderBy);
        }

        return await query.Select(projection).FirstOrDefaultAsync();
    }

    public async Task<PaginationResultDto<TResult>> GetAllWithPaginationAsync<TResult>(
        PaginationDto pagination,
        Expression<Func<T, TResult>> projection,
        bool ignoreGlobalQueryFilter = false,
        Expression<Func<T, bool>>? filter = null,
        Expression<Func<T, object?>>? orderBy = null,
        Expression<Func<T, bool>>? paginationSearchFilter = null)
    {
        var query = _context.Set<T>().AsNoTracking().AsQueryable();

        if (ignoreGlobalQueryFilter)
        {
            query = query.IgnoreQueryFilters();
        }

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (paginationSearchFilter != null && !string.IsNullOrWhiteSpace(pagination.SearchQuery))
        {
            query = query.Where(paginationSearchFilter);
        }

        var totalRecords = await query.CountAsync();

        if (orderBy != null)
        {
            query = query.OrderBy(orderBy);
        }

        var data = await query
            .Skip((pagination.Page - 1) * pagination.Limit)
            .Take(pagination.Limit)
            .Select(projection)
            .ToListAsync();

        return new PaginationResultDto<TResult>
        {
            TotalRecords = totalRecords,
            Data = data
        };
    }

    #endregion
}
