using AnonymousApplication.DTOs;
using AnonymousApplication.Enums;
using AnonymousApplication.Interfaces;
using Dapper;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AnonymousApplication.Services
{
    public class UserAuthenticationService : IUserAuthentication
    {
        private readonly IConfiguration _configuration;
        private readonly IDbConnectionFactory _connectionFactory;

        public UserAuthenticationService(IConfiguration configuration, IDbConnectionFactory connectionFactory)
        {
            _configuration = configuration;
            _connectionFactory = connectionFactory;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginDto request)
        {
            using var connection = _connectionFactory.CreateConnection();

            var query = @"
    SELECT UserId, Username, Password, RoleId, IsActive
    FROM Users
    WHERE Username = @Username;";

            var user = await connection.QueryFirstOrDefaultAsync<UserDto>(
                query,
                new { request.Username });

            if (user == null)
                return null;

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(
                request.Password,
                user.Password);

            if (!isPasswordValid)
                return null;

            var claims = new[]
            {
              new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
              new Claim(ClaimTypes.Name, user.Username),
              new Claim(ClaimTypes.Role, user.RoleId.ToString()),
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiry = DateTime.UtcNow.AddHours(2);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiry,
                signingCredentials: creds
            );

            return new LoginResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiry
            };
        }

        public async Task<bool> RegisterUser(RegisterUserDto request)
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
        INSERT INTO Users (Username, Password)
        VALUES (@Username, @Password);";

            var result = await connection.ExecuteAsync(insertQuery, new
            {
                request.Username,
                Password = hashedPassword
            });

            return result > 0;
        }

        public async Task<bool> CheckUniqueUsername(string username)
        {
            using var connection = _connectionFactory.CreateConnection();

            string query = @"SELECT 1 FROM Users WHERE Username = @username;";

            var exists = await connection.ExecuteScalarAsync<bool>(
                query,
                new { username });

            if (!exists)
            {
                return false;
            }

            return true;
        }

        public async Task<int> DeleteUser(Guid userId)
        {
            using var connection = _connectionFactory.CreateConnection();

            string query = @"Delete from Users where UserId = @userId";

            var deleted = await connection.ExecuteAsync(
                query,
                new { userId });

            if (deleted > 0)
            {
                return deleted;
            }
            return 0;
        }
    }
}   
