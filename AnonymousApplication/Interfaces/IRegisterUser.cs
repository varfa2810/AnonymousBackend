using AnonymousApplication.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnonymousApplication.Interfaces
{
    public interface IRegisteruser
    {
        Task<bool> RegisterEmployee(RegisterUserDto request);

        Task<bool> RegisterCompanyadmin(RegisterCompanyAdminDto request);
        Task<Guid> RegisterCompany(RegisterCompanyDto register);

    }
}
