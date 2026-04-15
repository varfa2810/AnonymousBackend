using AnonymousApplication.DTOs;
using AnonymousApplication.Interfaces;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace AnonymousApplication.Services
{
    public class RegisterCompanyService : IRegsiterCompany
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public RegisterCompanyService(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Guid> RegisterCompany(RegisterCompanyDto register)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new
            {
                CompanyName = register.CompanyName,
                EmployeeStrength = register.EmployeeStrength,
                Email = register.Email,
                Phone = register.Phone,
                CountryId = register.CountryId,
                StateId = register.StateId,
                CityId = register.CityId,
                CompanyAddress = register.CompanyAddress,
                UpdateDate = DateTime.UtcNow,
                HREmail = register.HREmail
               
            };

            var companyId = await connection.QuerySingleAsync<Guid>(
                "[RegisterCompany]",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return companyId;
        }
    }
}
