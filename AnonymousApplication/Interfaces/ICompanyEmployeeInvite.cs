using System;
using System.Collections.Generic;
using System.Text;

namespace AnonymousApplication.Interfaces
{
    public interface ICompanyEmployeeInvite
    {
        Task<string> CreateCompanyEmployeeInviteLink(Guid companyId);
    }
}
