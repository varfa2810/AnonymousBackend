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
        public async Task<int> ApproveOrRejectCompanyRequest(Guid companyId, bool action)
        {
            using var connection = _connectionFactory.CreateConnection();

            if (action)
            {
                string query = @"UPDATE Companies SET IsApproved = 1 WHERE CompanyId = @companyid;";
                var rowsreturned = await connection.ExecuteAsync(query, new { companyid = companyId });
                BackgroundJob.Enqueue<IEmail>(x => x.SendCompanyApproveOrDissapproveEmail(action, companyId));
                return rowsreturned;
            }
            else
            {
                string query = @"DELETE FROM Companies WHERE CompanyId = @companyid;";
                var rowsreturned = await connection.ExecuteAsync(query, new { companyid = companyId });
                BackgroundJob.Enqueue<IEmail>(x => x.SendCompanyApproveOrDissapproveEmail(action, companyId));
                return rowsreturned;
            }
        }

        public async Task<List<CompanyDetailsResponseDto>> GetAllCompanyDetails()
        {
            using var connection = _connectionFactory.CreateConnection();

            string query = @"
                        SELECT 
                            c.CompanyId,
                            c.CompanyName,
                            c.EmployeeStrength,
                            c.Email,
                            c.Phone,
                            c.CompanyAddress,
                            c.CreatedDate AS CompanyCreatedDate,
                            c.UpdatedDate AS CompanyUpdatedDate,
                            c.IsApproved,
                            c.HREmail,
                            cb.BranchId,
                            co.CountryName,
                            st.StateName,
                            ct.CityName,
                            cb.CreatedDate AS BranchCreatedDate
                        FROM Companies AS c
                        LEFT JOIN CompanyBranches AS cb ON c.CompanyId = cb.CompanyId
                        LEFT JOIN Countries AS co ON cb.CountryId = co.CountryId
                        LEFT JOIN States AS st ON cb.StateId = st.StateId
                        LEFT JOIN Cities AS ct ON cb.CityId = ct.CityId;
                        ";

            var details = await connection.QueryAsync<CompanyDetailsResponseDto>(query);

            return details.ToList();

        }

        public async Task<CompanyDetailsResponseDto?> GetCompanyDetailsFromCompanyId(Guid companyId)
        {
            using var connection = _connectionFactory.CreateConnection();

            string query = @"
                        SELECT 
                            c.CompanyId,
                            c.CompanyName,
                            c.EmployeeStrength,
                            c.Email,
                            c.Phone,
                            c.CompanyAddress,
                            c.CreatedDate AS CompanyCreatedDate,
                            c.UpdatedDate AS CompanyUpdatedDate,
                            c.IsApproved,
                            c.HREmail,
                            cb.BranchId,
                            co.CountryName,
                            st.StateName,
                            ct.CityName,
                            cb.CreatedDate AS BranchCreatedDate
                        FROM Companies AS c
                        LEFT JOIN CompanyBranches AS cb ON c.CompanyId = cb.CompanyId
                        LEFT JOIN Countries AS co ON cb.CountryId = co.CountryId
                        LEFT JOIN States AS st ON cb.StateId = st.StateId
                        LEFT JOIN Cities AS ct ON cb.CityId = ct.CityId
                        where c.CompanyId = @companyid;
                        ";

            var details = await connection.QueryFirstOrDefaultAsync<CompanyDetailsResponseDto>(query);

            return details;
        }
    }
}
