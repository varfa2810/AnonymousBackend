using AnonymousApplication.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnonymousApplication.Interfaces
{
    public interface IUserAuthentication
    {
        Task<LoginResponseDto?> LoginAsync(LoginDto request);
        Task<bool> CheckUniqueUsername(string username);
        Task<int> DeleteUser(Guid userId);
        Task<UserProfileResponseDto?> GetUserProfileByUserId(Guid userId);
    }
}
