using AnonymousApplication.Interfaces;
using Dapper;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AnonymousApplication.Services
{
    public class InviteService : ICompanyAdminInvite, ICompanyEmployeeInvite, IInviteVerify
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;

        public InviteService(IDbConnectionFactory connectionFactory, IConfiguration config, IWebHostEnvironment webHost)
        {
            _connectionFactory = connectionFactory;
            _config = config;
            _env = webHost;
        }
        public async Task<string> CreateCompanyAdminInviteLink(Guid companyId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var query = @"select c.CompanyName, co.CountryName, ct.CityName, cb.BranchId from Companies as c 
                          left join CompanyBranches as cb on c.CompanyId = cb.CompanyId
                          LEFT JOIN Countries AS co ON cb.CountryId = co.CountryId
                          LEFT JOIN Cities AS ct ON cb.CityId = ct.CityId
                          where c.CompanyId = @companyid;";

            var companyDetails = await connection.QuerySingleOrDefaultAsync<dynamic>(query, new { companyId });

            var key = new SymmetricSecurityKey(
               Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                  new Claim("company", companyDetails?.CompanyName),
                  new Claim("branch", companyDetails?.BranchId),
                  new Claim("country", companyDetails?.CountryName),
                  new Claim("city", companyDetails?.CityName),
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials
            );

            string inviteLink = string.Empty;
            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            if (_env.EnvironmentName.Equals("Development"))
            {
                inviteLink = $"https://localhost:7107/register/register-companyadmin?token={jwtToken}";
            }
            else if (_env.EnvironmentName.Equals("Production"))
            {
                inviteLink = $"https://localhost:7107/register/register-companyadmin?token={jwtToken}";
            }

            return inviteLink;
        }

        public async Task<string> CreateCompanyEmployeeInviteLink(Guid companyId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var query = @"select c.CompanyName, co.CountryName, ct.CityName, cb.BranchId from Companies as c 
                          left join CompanyBranches as cb on c.CompanyId = cb.CompanyId
                          LEFT JOIN Countries AS co ON cb.CountryId = co.CountryId
                          LEFT JOIN Cities AS ct ON cb.CityId = ct.CityId
                          where c.CompanyId = @companyid;";

            var companyDetails = await connection.QuerySingleOrDefaultAsync<dynamic>(query, new { companyId });

            var key = new SymmetricSecurityKey(
               Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                  new Claim("company", companyDetails?.CompanyName),
                  new Claim("branch", companyDetails?.BranchId),
                  new Claim("country", companyDetails?.CountryName),
                  new Claim("city", companyDetails?.CityName),
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials
            );

            string inviteLink = string.Empty;
            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            if (_env.EnvironmentName.Equals("Development"))
            {
                inviteLink = $"https://localhost:7107/register/register-companyemployee?token={jwtToken}";
            }
            else if (_env.EnvironmentName.Equals("Production"))
            {
                inviteLink = $"https://localhost:7107/register/register-companyemployee?token={jwtToken}";
            }

            return inviteLink;
        }

        public async Task<bool> VerifyInviteLink(string inviteLink)
        {
            var uri = new Uri(inviteLink);
            var queryParams = QueryHelpers.ParseQuery(uri.Query);
            string? token = queryParams["token"];

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _config["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _config["Jwt:Audience"],
                ValidateLifetime = true, // expiry check
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                var companyName = principal.FindFirst("company")?.Value;
                var country = principal.FindFirst("country")?.Value;
                var city = principal.FindFirst("city")?.Value;

                return true;
            }
            catch (SecurityTokenExpiredException)
            {
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
