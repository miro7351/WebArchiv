using System;
using System.Collections.Generic;

namespace PA.TOYOTA.DB
{
    public partial class Account
    {
        public int LoginId { get; set; }
        public string LoginName { get; set; } = null!;
        public string LoginPassword { get; set; } = null!;
        public string LoginRola { get; set; } = null!;
    }
}
