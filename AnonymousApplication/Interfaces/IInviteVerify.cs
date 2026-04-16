using System;
using System.Collections.Generic;
using System.Text;

namespace AnonymousApplication.Interfaces
{
    public interface IInviteVerify
    {
        Task<bool> VerifyInviteLink(string link);
    }
}
