using System;
using System.Collections.Generic;

namespace DataTable1.DB.Model
{
    public partial class CustomerTb
    {
        public int CustomerId { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? PhoneNo { get; set; }
    }
}
