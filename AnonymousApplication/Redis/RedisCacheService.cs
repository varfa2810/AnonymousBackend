using AnonymousApplication.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace AnonymousApplication.Services
{
    public sealed class RedisCacheService : IRedisCache
    {
        private readonly IDatabase _database;
        private readonly ILogger<RedisCacheService> _logger;

        private readonly string _keyPrefix;
        private readonly TimeSpan _defaultExpiry;

        private static readonly JsonSerializerOptions _serializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public RedisCacheService(
            IConnectionMultiplexer multiplexer,
            IConfiguration configuration,
            ILogger<RedisCacheService> logger)
        {
            _database = multiplexer.GetDatabase();

            _logger = logger;

            _keyPrefix = configuration["Redis:KeyPrefix"] ?? "anonymous-api";

            _defaultExpiry = TimeSpan.FromMinutes(
                configuration.GetValue<int>("Redis:DefaultExpiryMinutes"));
        }

        public async Task<T?> GetAsync<T>(string key)
            where T : class
        {
            try
            {
                key = BuildKey(key);

                RedisValue cachedValue = await _database.StringGetAsync(key);

                if (cachedValue.IsNullOrEmpty)
                {
                    _logger.LogInformation("Redis Cache MISS : {Key}", key);
                    return default;
                }

                _logger.LogInformation("Redis Cache HIT : {Key}", key);

                return JsonSerializer.Deserialize<T>(
                    cachedValue.ToString()!,
                    _serializerOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get cache for key {Key}", key);

                return default;
            }
        }

        public async Task<bool> SetAsync<T>(
            string key,
            T value,
            TimeSpan? expiry = null)
            where T : class
        {
            try
            {
                key = BuildKey(key);

                string json = JsonSerializer.Serialize(
                    value,
                    _serializerOptions);

                return await _database.StringSetAsync(
                    key,
                    json,
                    expiry ?? _defaultExpiry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to set cache for key {Key}", key);

                return false;
            }
        }

        public async Task<bool> RemoveAsync(string key)
        {
            try
            {
                key = BuildKey(key);

                return await _database.KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove cache for key {Key}", key);

                return false;
            }
        }

        private string BuildKey(string key)
        {
            return $"{_keyPrefix}:{key}";
        }
    }
}
