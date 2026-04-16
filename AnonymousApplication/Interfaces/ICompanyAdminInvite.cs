using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnonymousApplication.Interfaces
{
    public interface ICompanyAdminInvite
    {
         Task<string> CreateCompanyAdminInviteLink(Guid companyId);
    }
}
