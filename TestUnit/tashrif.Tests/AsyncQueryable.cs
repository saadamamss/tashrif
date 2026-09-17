using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query;

namespace tashrif.Tests;

public static class AsyncQueryable
{
    public static IQueryable<T> BuildMock<T>(this IEnumerable<T> source)
    {
        var queryable = source.AsQueryable();
        return new TestAsyncEnumerable<T>(queryable);
    }
}

public class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
    public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
    public TestAsyncEnumerable(Expression expression) : base(expression) { }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        => new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());

    IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
}

public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _inner;
    public TestAsyncEnumerator(IEnumerator<T> inner) => _inner = inner;
    public ValueTask DisposeAsync() { _inner.Dispose(); return ValueTask.CompletedTask; }
    public ValueTask<bool> MoveNextAsync() => ValueTask.FromResult(_inner.MoveNext());
    public T Current => _inner.Current;
}

public class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
{
    private readonly IQueryProvider _inner;
    public TestAsyncQueryProvider(IQueryProvider inner) => _inner = inner;

    public IQueryable CreateQuery(Expression expression) => new TestAsyncEnumerable<TEntity>(expression);
    public IQueryable<TElement> CreateQuery<TElement>(Expression expression) => new TestAsyncEnumerable<TElement>(expression);

    public object? Execute(Expression expression)
    {
        // Handle ExecuteUpdate — apply SetProperty changes to in-memory collection
        // EF Core's ExecuteUpdateAsync internally calls ExecuteUpdate (sync)
        if (expression is MethodCallExpression mce
            && mce.Method.Name == "ExecuteUpdate")
        {
            return HandleExecuteUpdate(mce);
        }

        return _inner.Execute(expression);
    }

    public TResult Execute<TResult>(Expression expression)
    {
        // EF Core's ExecuteUpdate (sync, called by ExecuteUpdateAsync) calls Provider.Execute<int>()
        // — intercept before it reaches the inner EnumerableQuery provider
        if (typeof(TResult) == typeof(int))
        {
            var result = Execute(expression);
            return (TResult)result!;
        }
        if (typeof(TResult) == typeof(Task<int>))
        {
            var result = Execute(expression);
            return (TResult)(object)Task.FromResult((int)result!)!;
        }
        return _inner.Execute<TResult>(expression);
    }

    public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
    {
        var result = Execute(expression);
        var resultType = typeof(TResult);
        if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Task<>))
        {
            var innerType = resultType.GetGenericArguments()[0];
            var fromResult = typeof(Task).GetMethod("FromResult")!.MakeGenericMethod(innerType);
            return (TResult)fromResult.Invoke(null, new[] { result })!;
        }
        return (TResult)result!;
    }

    /// <summary>
    /// Handles ExecuteUpdate by evaluating the source query and applying
    /// SetProperty changes to the in-memory entities. This allows unit tests
    /// to use ExecuteUpdateAsync with in-memory collections.
    /// </summary>
    private object HandleExecuteUpdate(MethodCallExpression mce)
    {
        // Arguments[0] = source IQueryable expression
        var sourceExpression = mce.Arguments[0];

        // Evaluate the source to get entities
        var sourceQuery = _inner.CreateQuery(sourceExpression);
        var entities = sourceQuery.Cast<object>().ToList();

        // Extract the SetProperty calls from the lambda
        var updates = ExtractSetPropertyCalls(mce);

        // Apply updates to each entity
        foreach (var entity in entities)
        {
            foreach (var (propertyName, value) in updates)
            {
                var prop = entity.GetType().GetProperty(propertyName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (prop != null && prop.CanWrite)
                {
                    var convertedValue = value;
                    if (value != null && !prop.PropertyType.IsAssignableFrom(value.GetType()))
                    {
                        convertedValue = Convert.ChangeType(value, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                    }
                    prop.SetValue(entity, convertedValue);
                }
            }
        }

        return entities.Count;
    }

    /// <summary>
    /// Extracts (PropertyName, Value) pairs from ExecuteUpdate's SetProperty lambda.
    /// Walks the expression tree to find SetProperty calls and extracts property names + values.
    /// </summary>
    private static List<(string PropertyName, object? Value)> ExtractSetPropertyCalls(MethodCallExpression mce)
    {
        var result = new List<(string, object?)>();

        // Arguments[1] = UnaryExpression(Quote) wrapping the lambda: a => a.SetProperty(...)
        if (mce.Arguments.Count < 2) return result;

        var lambdaArg = mce.Arguments[1];
        LambdaExpression? outerLambda = Unwrap<LambdaExpression>(lambdaArg);
        if (outerLambda == null) return result;

        // The body is a chain of SetProperty calls
        WalkSetPropertyChain(outerLambda.Body, result);
        return result;
    }

    /// <summary>
    /// Recursively walks the SetProperty chain on SetPropertyCalls{T}.
    /// SetProperty is an INSTANCE method: mce.Object = chain, Args[0] = property, Args[1] = value.
    /// </summary>
    private static void WalkSetPropertyChain(Expression expression, List<(string, object?)> result)
    {
        if (expression is not MethodCallExpression mce) return;
        if (mce.Method.Name != "SetProperty") return;

        // Recurse into mce.Object (the chain / previous calls)
        if (mce.Object != null)
        {
            WalkSetPropertyChain(mce.Object, result);
        }

        if (mce.Arguments.Count < 2) return;

        var propertyArg = mce.Arguments[0];
        string? propertyName = ExtractPropertyName(propertyArg);
        if (propertyName == null) return;

        // Arguments[1] = value expression — compile and invoke to get the actual value
        var valueArg = mce.Arguments[1];
        try
        {
            var value = Expression.Lambda(valueArg).Compile().DynamicInvoke();
            result.Add((propertyName, value));
        }
        catch
        {
            // If we can't compile the value expression, skip it
        }
    }

    /// <summary>
    /// Extracts a property name from various expression wrapping patterns.
    /// Handles: LambdaExpression, UnaryExpression(Quote), MemberExpression, etc.
    /// </summary>
    private static string? ExtractPropertyName(Expression expression)
    {
        // Unwrap Quote/Convert nodes
        var unwrapped = expression;
        while (unwrapped is UnaryExpression ue)
        {
            unwrapped = ue.Operand;
        }

        // If it's a lambda, get the body
        if (unwrapped is LambdaExpression lambda)
        {
            unwrapped = lambda.Body;
        }

        // If it's a member access, return the name
        if (unwrapped is MemberExpression member)
        {
            return member.Member.Name;
        }

        return null;
    }

    /// <summary>
    /// Unwraps an expression by stripping UnaryExpression(Quote/Convert) layers.
    /// </summary>
    private static T? Unwrap<T>(Expression expression) where T : Expression
    {
        var current = expression;
        while (current is UnaryExpression ue)
        {
            if (ue.Operand is T typed) return typed;
            current = ue.Operand;
        }
        return current as T;
    }
}
