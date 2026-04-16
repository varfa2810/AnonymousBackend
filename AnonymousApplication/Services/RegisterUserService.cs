using AnonymousApplication.DTOs;
using AnonymousApplication.Interfaces;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace AnonymousApplication.Services
{
    public class RegisterUserService : IRegisteruser
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public RegisterUserService(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<bool> RegisterEmployee(RegisterUserDto request)
        {
            using var connection = _connectionFactory.CreateConnection();

            // Check if username already exists
            var checkQuery = @"SELECT COUNT(1) FROM Users WHERE Username = @Username;";

            var exists = await connection.ExecuteScalarAsync<int>(
                checkQuery,
                new { request.Username });

            if (exists > 0)
                return false;

            // Hash password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var insertQuery = @"
        INSERT INTO Users (Username, Password, RoleId, IsActive, BranchId)
        VALUES (@Username, @Password, 1, 1, @BranchId);";

            var result = await connection.ExecuteAsync(insertQuery, new
            {
                request.Username,
                Password = hashedPassword,
                request.CompanyBranchId
            });

            return result > 0;
        }

        public async Task<bool> RegisterCompanyadmin(RegisterCompanyAdminDto request)
        {
            if (request.DesignationId <=0 || request.CompanyBranchId <= 0)
            {
                throw new ArgumentException("Designation and BranchId are required for company admin registration.");
            }

            using var connection = _connectionFactory.CreateConnection();

            // Check if username already exists
            var checkQuery = @"SELECT COUNT(1) FROM Users WHERE Username = @Username;";

            var exists = await connection.ExecuteScalarAsync<int>(
                checkQuery,
                new { request.Username });

            if (exists > 0)
                return false;

            // Hash password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var insertQuery = @"
        INSERT INTO Users (Username, Password, RoleId, IsActive, BranchId, DesignationId)
        VALUES (@Username, @Password, 2, 1, @BranchId, @DesignationId);";

            var result = await connection.ExecuteAsync(insertQuery, new
            {
                Username = request.Username,
                Password = hashedPassword,
                BranchId = request.CompanyBranchId,
                Designationid = request.DesignationId
            });

            return result > 0;
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
