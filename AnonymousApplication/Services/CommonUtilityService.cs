using AnonymousApplication.DTOs;
using AnonymousApplication.Interfaces;
using AnonymousApplication.Redis;
using Dapper;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using static Confluent.Kafka.ConfigPropertyNames;

namespace AnonymousApplication.Services
{
    public class CommonUtilityService(
        IDbConnectionFactory connectionFactory,
        IRedisCache cache,
        ILogger<CommonUtilityService> logger) : ICommonUtilities
    {
        private readonly IDbConnectionFactory _connectionFactory = connectionFactory;
        private readonly IRedisCache _cache = cache;
        private readonly ILogger<CommonUtilityService> _logger = logger;

        public async Task<List<dynamic>> GetCountries()
        {
            using var connection = _connectionFactory.CreateConnection();

            string key = CacheKeys.CountryList;
            string query = @"select * from Countries;";

            var cachedCountries = await _cache.GetAsync<List<dynamic>>(key);

            if (cachedCountries != null)
            {
                return cachedCountries;
            }

            var countries = await connection.QueryAsync<dynamic>(query);

            await _cache.SetAsync(key, countries, TimeSpan.FromHours(6));

            return [.. countries];
        }

        public async Task<List<dynamic>> GetStates(int countryId)
        {
            using var connection = _connectionFactory.CreateConnection();

            string query = @"select StateId, StateName from States where CountryId = @countryid;";

            var states = await connection.QueryAsync<dynamic>(query, new { countryid = countryId });
            return [.. states];
        }

        public async Task<List<dynamic>> GetCities(int stateId)
        {
            using var connection = _connectionFactory.CreateConnection();

            string query = @"select CityId, CityName from Cities where StateId = @stateid;";

            var cities = await connection.QueryAsync<dynamic>(query, new { stateid = stateId });
            return [.. cities];
        }

        public async Task<List<dynamic>> GetAllRoles()
        {
            using var connection = _connectionFactory.CreateConnection();

            string query = @"select RoleId, RoleName from Roles";

            var roles = await connection.QueryAsync<dynamic>(query);
            return [.. roles];

        }

        public async Task<List<dynamic>> GetAllCompanyAdminDesignation()
        {
            using var connection = _connectionFactory.CreateConnection();

            string query = @"select * from CompanyAdminDesignation";

            var cad = await connection.QueryAsync<dynamic>(query);
            return [.. cad];
        }
        public async Task<List<dynamic>> GetAllCommentViolationsOptions()
        {
            using var connection = _connectionFactory.CreateConnection();

            string query = @"select * from ViolationOptions";

            var options = await connection.QueryAsync<dynamic>(query);
            return [.. options];
        }
    }
}
