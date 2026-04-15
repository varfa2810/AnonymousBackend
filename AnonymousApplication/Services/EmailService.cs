using AnonymousApplication.DTOs;
using AnonymousApplication.Interfaces;
using Dapper;
using Hangfire;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AnonymousApplication.Services
{
    public class EmailService : IEmail
    {
        private readonly IConfiguration _config;
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IWebHostEnvironment _env;

        public EmailService(IConfiguration config, IDbConnectionFactory connectionFactory, IWebHostEnvironment env)
        {
            _config = config;
            _connectionFactory = connectionFactory;
            _env = env;
        }


        [AutomaticRetry(Attempts = 0)]
        public async Task<string> SendCompanyApproveOrDissapproveEmail(bool action, Guid companyId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var query = @"select CompanyName , HREmail ,Email from Companies where CompanyId = @companyid;";

            var details = await connection.QuerySingleOrDefaultAsync<dynamic>(query, new { companyId });

            if (details == null)
            {
                throw new Exception("No details found.");
            }

            string templatePath = string.Empty;

            if (action)
            {
                templatePath = Path.Combine(Directory.GetCurrentDirectory(),
                   "AnonymousInfrastructure", "EmailTemplates", "CompanyApproved.html");
            }
            else
            {
                templatePath = Path.Combine(Directory.GetCurrentDirectory(),
                    "AnonymousInfrastructure", "EmailTemplates", "CompanyRejected.html");
            }

            var template = await File.ReadAllTextAsync(templatePath);

            string body = string.Empty;

            if (action)
            {
                string inviteLink = GenerateInviteToken();

                body = template
                         .Replace("{{CompanyName}}", details?.CompanyName)
                         .Replace("{{LoginUrl}}", inviteLink);
            }
            else
            {
                body = template
                         .Replace("{{CompanyName}}", details?.CompanyName);

            }

            var apiKey = _config["SendGrid:ApiKey"];
            var client = new SendGrid.SendGridClient(apiKey);

            var from = new SendGrid.Helpers.Mail.EmailAddress(
                _config["SendGrid:FromEmail"],
                _config["SendGrid:FromName"]
            );

            var to = new SendGrid.Helpers.Mail.EmailAddress(details?.Email);

            var msg = SendGrid.Helpers.Mail.MailHelper.CreateSingleEmail(
                from,
                to,
                $"Company {(action ? "Approved" : "Rejected")} 🎉",
                "",
                body
            );

            var response = await client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Email sending failed");

            }

            return "Email sent successfully";

        }

        private string GenerateInviteToken()
        {

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                 issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials
            );

            string inviteLink = string.Empty;
            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            if (_env.EnvironmentName.Equals("Development"))
            {
                inviteLink = $"https://localhost:7107?token={jwtToken}";
            }
            else if (_env.EnvironmentName.Equals("Production"))
            {
                inviteLink = $"https://localhost:7107?token={jwtToken}";
            }

            return inviteLink;
        }

    }
}
