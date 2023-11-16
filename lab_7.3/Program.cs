using System;
using System.Collections.Generic;

class FunctionCache<TKey, TResult>
{
    private Dictionary<TKey, CacheItem<TResult>> cache = new Dictionary<TKey, CacheItem<TResult>>();

    public delegate TResult FuncDelegate(TKey key);

    public TResult GetOrAdd(TKey key, FuncDelegate func, TimeSpan cacheDuration)
    {
        if (cache.TryGetValue(key, out CacheItem<TResult> cachedItem) && !IsCacheItemExpired(cachedItem, cacheDuration))
        {
            Console.WriteLine($"Cache hit for key: {key}");
            return cachedItem.Value;
        }
        else
        {
            Console.WriteLine($"Cache miss for key: {key}");
            TResult result = func(key);
            cache[key] = new CacheItem<TResult> { Value = result, ExpirationTime = DateTime.Now.Add(cacheDuration) };
            return result;
        }
    }

    private bool IsCacheItemExpired(CacheItem<TResult> cachedItem, TimeSpan cacheDuration)
    {
        return DateTime.Now > cachedItem.ExpirationTime;
    }

    private class CacheItem<T>
    {
        public T Value { get; set; }
        public DateTime ExpirationTime { get; set; }
    }
}

class Program
{
    static void Main()
    {
        // Використання кешу для функції, яка повертає квадрат числа
        FunctionCache<int, int> cache = new FunctionCache<int, int>();

        // Визначення функції, яку будемо кешувати
        FunctionCache<int, int>.FuncDelegate squareFunction = x =>
        {
            Console.WriteLine($"Calculating square for {x}");
            return x * x;
        };

        // Виклик функції через кеш
        int result1 = cache.GetOrAdd(5, squareFunction, TimeSpan.FromSeconds(5));
        int result2 = cache.GetOrAdd(5, squareFunction, TimeSpan.FromSeconds(5)); // повинен бути взятий з кешу

        Console.WriteLine($"Result 1: {result1}");
        Console.WriteLine($"Result 2: {result2}");
    }
}
