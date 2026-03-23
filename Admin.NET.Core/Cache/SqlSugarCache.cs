// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace Admin.NET.Core;

/// <summary>
/// SqlSugar二级缓存
/// </summary>
public class SqlSugarCache : ICacheService
{
    /// <summary>
    /// 系统缓存服务（延迟初始化，避免启动时未准备好）
    /// </summary>
    private SysCacheService GetCache()
    {
        try
        {
            var cache = App.GetService<SysCacheService>();
            return cache;
        }
        catch
        {
            // 如果获取失败，返回 null
            return null;
        }
    }

    public void Add<V>(string key, V value)
    {
        var cache = GetCache();
        cache?.Set($"{CacheConst.SqlSugar}{key}", value);
    }

    public void Add<V>(string key, V value, int cacheDurationInSeconds)
    {
        var cache = GetCache();
        cache?.Set($"{CacheConst.SqlSugar}{key}", value, TimeSpan.FromSeconds(cacheDurationInSeconds));
    }

    public bool ContainsKey<V>(string key)
    {
        var cache = GetCache();
        return cache?.ExistKey($"{CacheConst.SqlSugar}{key}") ?? false;
    }

    public V Get<V>(string key)
    {
        var cache = GetCache();
        return cache != null ? cache.Get<V>($"{CacheConst.SqlSugar}{key}") : default;
    }

    public IEnumerable<string> GetAllKey<V>()
    {
        var cache = GetCache();
        return cache?.GetKeysByPrefixKey(CacheConst.SqlSugar) ?? Enumerable.Empty<string>();
    }

    public V GetOrCreate<V>(string key, Func<V> create, int cacheDurationInSeconds = int.MaxValue)
    {
        var cache = GetCache();
        if (cache == null) return create();
        
        return cache.GetOrAdd<V>($"{CacheConst.SqlSugar}{key}", (cacheKey) =>
        {
            return create();
        }, cacheDurationInSeconds);
    }

    public void Remove<V>(string key)
    {
        var cache = GetCache();
        cache?.Remove(key); // SqlSugar调用Remove方法时，key中已包含了CacheConst.SqlSugar前缀
    }
}
