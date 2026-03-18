using AnonymousApplication.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnonymousApplication.Interfaces
{
    public interface ISendMessage
    {
        Task<int> SendMessage(SendMessageDto message);
    }
}
