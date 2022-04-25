﻿using System;
using System.Collections.Generic;

namespace PA.TOYOTA.DB
{
    public partial class Error
    {
        //MH: april 2022
        //nad zaznamamy sa nedaju robit operacie: DELETE ani UPDATE !!! 
        public int ErrorLogId { get; set; }
        public DateTime ErrorDate { get; set; }
        public string? ErrorMsg { get; set; }
        public int? ErrorNumber { get; set; }
        public string? ErrorProcedure { get; set; }
        public int? ErrorLine { get; set; }
        public string? User { get; set; }
    }
}
