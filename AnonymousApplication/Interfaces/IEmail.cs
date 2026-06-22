using AnonymousApplication.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnonymousApplication.Interfaces
{
    public interface IEmail
    {
        Task<bool> SendCompanyApproveOrDissapproveEmail(bool action, Guid CompanyId);
    }
}
