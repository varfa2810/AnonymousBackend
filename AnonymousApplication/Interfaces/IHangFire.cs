using System;
using System.Collections.Generic;
using System.Text;

namespace AnonymousApplication.Interfaces
{
    public interface IHangFire
    {
        Task DeleteOldMessages();
    }

}
