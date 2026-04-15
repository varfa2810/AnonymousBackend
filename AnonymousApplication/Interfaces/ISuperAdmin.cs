using AnonymousApplication.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnonymousApplication.Interfaces
{
    public interface ISuperAdmin
    {
        Task<int> ApproveOrRejectCompanyRequest(Guid companyId, bool approve);

        Task<List<CompanyDetailsResponseDto>> GetAllCompanyDetails();
        Task<CompanyDetailsResponseDto?> GetCompanyDetailsFromCompanyId(Guid companyId);
    }
}
