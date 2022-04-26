using System;
using System.Collections.Generic;

namespace PA.TOYOTA.DB
{
    public partial class Log
    {
        public int LogId { get; set; }
        public DateTime LogDate { get; set; }
        public string? TableName { get; set; }
        public string? LogMessage { get; set; }
        public string? UserAction { get; set; }
        public string? UserName { get; set; }
    }
}
