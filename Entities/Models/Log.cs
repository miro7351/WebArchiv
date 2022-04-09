using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PA.TOYOTA.DB
{
    public partial class Log
    {
        [Display(Name = "ID")]
        public int LogId { get; set; }
        [Display(Name="Dátum")]
        public DateTime LogDate { get; set; }
        [Display(Name = "Zdrojová tabuľka")]
        public string? TableName { get; set; }
        [Display(Name = "Správa")]
        public string? LogMessage { get; set; }
        [Display(Name = "Akcia užívateľa")]
        public string? UserAction { get; set; }
        [Display(Name = "Užívateľ")]
        public string? UserName { get; set; }
    }
}
