using System;
using System.Collections.Generic;
using System.Text;

namespace AnonymousApplication.DTOs
{
    public class ReportDto
    {
        public int ReportId { get; set; }
        public int MessageId { get; set; }
        public string ReportedMessage { get; set; }
        public string ViolationType { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ReportCount { get; set; }
    }

    public class ReportResponseDto
    {
        public List<ReportDto> Reports { get; set; }
        public int TotalRecords { get; set; }

    }

}
