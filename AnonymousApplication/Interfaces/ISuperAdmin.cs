using AnonymousApplication.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnonymousApplication.Interfaces
{
    public interface ISuperAdmin
    {
        Task<int> ProcessCompanyRequest(ApproveorRejectCompanyDto request);

        Task<List<CompanyDetailsResponseDto>> GetAllCompanyDetails(int pageNumber, int pageSize, bool? companyStatus);
        Task<CompanyDetailsResponseDto?> GetCompanyDetailsFromCompanyId(Guid companyId);

        Task<List<CompanyAdminsDetailsDto>> GetAllCompanyAdmins();
        Task<List<CompanyAdminsDetailsDto>> GetCompanyAdminsFromCompanyId(Guid companyId);

        Task<List<UserDetailsDto>> GetAllUsersDetails(int? roleId, bool? isCompanyApproved, int pageNumber, int pageSize);
        Task<UserDetailsDto?> GetUserDetailsById(Guid userId);
    }
}   
