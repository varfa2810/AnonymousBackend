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
        private readonly ICompanyAdminInvite _companyAdminInvite;

        public EmailService(
            IConfiguration config,
            IDbConnectionFactory connectionFactory,
            IWebHostEnvironment env,
            ICompanyAdminInvite companyAdminInvite
            )
        {
            _config = config;
            _connectionFactory = connectionFactory;
            _env = env;
            _companyAdminInvite = companyAdminInvite;
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
                string inviteLink = await _companyAdminInvite.CreateCompanyAdminInviteLink(companyId);

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

    }
}
