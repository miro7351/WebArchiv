using ToyotaArchiv.Domain;

namespace ToyotaArchiv.Models
{
    public class ZakazkaZOViewModel
    {
        public ZakazkaZOViewModel()
        {
            //TODO: nacitat zo session
            CurrentUser = new User() { UserRole = Infrastructure.USER_ROLE.None };
        }

        public User CurrentUser { get; set; }


        public ZakazkaZO MyZakazkaZO{ get; set;}
    }
}
