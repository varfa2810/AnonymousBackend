using AnonymousApplication.Interfaces;
using Dapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnonymousApplication.Services
{
    public class LocationService : ILocation
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public LocationService(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public async Task<List<dynamic>> GetCountries()
        {
            using var connection = _connectionFactory.CreateConnection();

            string query = @"select * from Countries;";

            var countries = await connection.QueryAsync<dynamic>(query);
            return countries.ToList();
        }

        public async Task<List<dynamic>> GetStates(int countryId)
        {
            using var connection = _connectionFactory.CreateConnection();

            string query = @"select StateId, StateName from States where CountryId = @countryid;";

            var states = await connection.QueryAsync<dynamic>(query, new { countryid = countryId });
            return states.ToList();
        }

        public async Task<List<dynamic>> GetCities(int stateId)
        {
            using var connection = _connectionFactory.CreateConnection();

            string query = @"select CityId, CityName from Cities where StateId = @stateid;";

            var cities = await connection.QueryAsync<dynamic>(query, new { stateid = stateId });
            return cities.ToList();
        }


    }
}
