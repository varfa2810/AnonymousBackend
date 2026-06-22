using AnonymousApplication.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace AnonymousInfrastructure.Data
{
    public class SqlConnectionFactory(IConfiguration configuration) : IDbConnectionFactory
    {
        private readonly string _connectionString = configuration.GetConnectionString("Default")!;

        public IDbConnection CreateConnection()
            => new SqlConnection(_connectionString);
    }
}
