using AnonymousApplication.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnonymousApplication.Interfaces
{
    public interface IMessages
    {
        Task<List<MessageDto>> GetAllMessages(Guid userid);
        Task<List<MessageDto>> GetMessagesByUserId(Guid userid);

        Task<bool> ReactToMessage(ReactToMessageDto reactToMessageDto);
        Task<bool> DeleteMessage(int messageId);

    }
}
