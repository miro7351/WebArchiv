using Microsoft.EntityFrameworkCore;

//MH: maj 2022
/*
 * 
 */ 

namespace PA.TOYOTA.DB
{
    public partial class ToyotaContext : DbContext
    {
        public string? ConnectionString { get; }
        public ToyotaContext()
        {
            ConnectionString = Database.GetConnectionString();
        }

        public ToyotaContext(DbContextOptions<ToyotaContext> options)
            : base(options)
        {
            ConnectionString = Database.GetConnectionString();
        }
    }
}
