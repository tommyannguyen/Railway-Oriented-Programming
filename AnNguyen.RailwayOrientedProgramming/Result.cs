using System.Collections.Immutable;

namespace AnNguyen.RailwayOrientedProgramming;

public record Result<T>(
    T? Value,
    ImmutableArray<string> Errors)
{
    public bool Success => Errors.Length == 0;
}

public static class ResultExtensions
{
    public static Result<T> Failure<T>(ImmutableArray<string> errors) => new(default, errors);
     public static Result<T> Success<T>(T value) => new(value, ImmutableArray<string>.Empty);

    // Bind sync
    public static Result<U> Bind<T, U>(this Result<T> r, Func<T, Result<U>> method)
    {
        try
        {
            return r.Success
                ? method(r.Value)
                : Failure<U>(r.Errors);
        }
        catch (Exception)
        {
            throw;
        }
    }

    // Bind async
    public static async Task<Result<U>> Bind<T, U>(this Task<Result<T>> result, Func<T, Task<Result<U>>> method)
    {
        try
        {
            var r = await result;
            return r.Success
                ? await method(r.Value)
                : Failure<U>(r.Errors);
        }
        catch (Exception)
        {
            throw;
        }
    }

    // Then sync
    public static Result<T> Then<T>(this Result<T> r, Action<T> action)
    {
        try
        {
            if (r.Success)
            {
                action(r.Value);
            }

            return r;
        }
        catch (Exception)
        {
            throw;
        }
    }

    // Then async
    public async static Task<Result<T>> Then<T>(this Task<Result<T>> result, Func<T, Task> action)
    {
        try
        {
            var r = await result;
            if (r.Success)
            {
                await action(r.Value);
            }

            return r;
        }
        catch (Exception)
        {
            throw;
        }
    }
    
    /// <summary>
    /// Map async
    /// </summary>
    public static Result<U> Map<T, U>(this Result<T> r, Func<T, U> mapper)
    {
        try
        {
            return r.Success
                ? Success(mapper(r.Value))
                : Failure<U>(r.Errors);
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Map async
    /// </summary>
    public static async Task<Result<U>> Map<T, U>(this Task<Result<T>> result, Func<T, Task<U>> mapper)
    {
        try
        {
            var r = await result;
            return r.Success
                ? Success(await mapper(r.Value))
                : Failure<U>(r.Errors);
        }
        catch (Exception)
        {
            throw;
        }
    }
}