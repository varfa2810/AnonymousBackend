using AnonymousApplication.DTOs;
using AnonymousApplication.Interfaces;
using Dapper;
using Hangfire;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AnonymousApplication.Services
{
    public class SuperAdminService : ISuperAdmin
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public SuperAdminService(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public async Task<int> ProcessCompanyRequest(ApproveorRejectCompanyDto request)
        {
            using var connection = _connectionFactory.CreateConnection();

            if (request.Action)
            {
                string query = @"UPDATE Companies SET IsApproved = 1 WHERE CompanyId = @companyid;";
                var rowsreturned = await connection.ExecuteAsync(query, new { companyid = request.CompanyId });
                BackgroundJob.Enqueue<IEmail>(x => x.SendCompanyApproveOrDissapproveEmail(request.Action, request.CompanyId));
                return rowsreturned;
            }
            else
            {
                string query = @"DELETE FROM Companies WHERE CompanyId = @companyid;";
                var rowsreturned = await connection.ExecuteAsync(query, new { companyid = request.CompanyId });
                BackgroundJob.Enqueue<IEmail>(x => x.SendCompanyApproveOrDissapproveEmail(request.Action, request.CompanyId));
                return rowsreturned;
            }
        }

        public async Task<List<CompanyAdminsDetailsDto>> GetAllCompanyAdmins()
        {
            using var connection = _connectionFactory.CreateConnection();

            var adminDetails = await connection.QueryAsync<CompanyAdminsDetailsDto>
                ("[GetAllCompanyAdminsDetails]", commandType: CommandType.StoredProcedure);

            return adminDetails.ToList();
        }

        public async Task<List<CompanyAdminsDetailsDto>> GetCompanyAdminsFromCompanyId(Guid companyId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var adminDetails = await connection.QueryAsync<CompanyAdminsDetailsDto>
                ("[GetAllCompanyAdminsDetails]", new { CompanyId = companyId }, commandType: CommandType.StoredProcedure);

            return adminDetails.ToList();
        }

        public async Task<List<CompanyDetailsResponseDto>> GetAllCompanyDetails(int pageNumber, int pageSize, bool? companyStatus)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new
            {
                Pagenumber = pageNumber,
                Pagesize = pageSize,
                CompanyStatus = companyStatus
            };

            var details = await connection.QueryAsync<CompanyDetailsResponseDto>
                ("GetCompaniesWithBranches", parameters, commandType: CommandType.StoredProcedure);

            return details.ToList();

        }

        public async Task<CompanyDetailsResponseDto?> GetCompanyDetailsFromCompanyId(Guid companyId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new
            {
                CompanyId = companyId
            };

            var details = await connection.QueryFirstOrDefaultAsync<CompanyDetailsResponseDto>
                  ("GetCompaniesWithBranches", parameters, commandType: CommandType.StoredProcedure);

            return details;
        }
    }
}
