
namespace ToyotaArchiv.Infrastructure
{
    //MH: april 2022
    /*pouzitie: AppData obsahuje static instanciu UserDetail  CurrentUserDetail, do nej sa nastavi login a rola a moze sa pouzivat v ramci celej aplikacie
    zapis: v AccountsController
           ....
           AppData.SetCurrentUser(login: user.LoginName, role: user.LoginRola)
   
    
    citanie: v kode nejakom controleri *Controller: string login = AppData.CurrentUserDetail.UserLogin
   
    na nejakej stranke  *.cshtml: 
   
    @using using ToyotaArchiv.Infrastructure;
   
     @{
        string currentRole = ToyotaArchiv.Global.AppData.CurrentUserDetail.UserRole;
        string login = ToyotaArchiv.Global.AppData.CurrentUserDetail.UserLogin;
    }

    napr. _Layout.cshtml
                       @if (currentRole=="ADMIN" || currentRole=="VEDUCI")  @*Len pre roly: ADMIN, VEDUCI*@
                        {
                             <li class="nav-item">
                                <a class="nav-link text-dark" asp-area="" asp-controller="Accounts" asp-action="Index">Účty</a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link text-dark" asp-area="" asp-controller="Logs" asp-action="Index">Logy</a>
                            </li>
                             <li class="nav-item">
                                <a class="nav-link text-dark" asp-area="" asp-controller="Errors" asp-action="Index">Chyby</a>
                            </li>
                        }
    Pri odhlaseni:
     Global.AppData.LogoutCurrentUser();

    */
    public class UserDetail
    {

        public UserDetail()
        {
            UserLogin = string.Empty;
            UserRole  = "READONLY";
        }

        public UserDetail(string login, string rola)
        {
            UserLogin = login;
            UserRole = rola;
        }

        public string UserLogin { get; set; }
        public string UserRole { get; set; }
    }

    /*
    public class UserDetail2
    {

        public UserDetail2()
        {
            UserLogin = String.Empty;
            UserRole = USER_ROLE.READONLY;
        }

        public UserDetail2(string login, USER_ROLE rola)
        {
            UserLogin = login;
            UserRole = rola;
        }

        public UserDetail2(string login, string rola)
        {
            UserLogin = login;
            UserRole = (USER_ROLE)Enum.Parse(typeof(USER_ROLE), rola);
        }

        public string UserLogin { get; set; }
        public USER_ROLE UserRole { get; set; }
    }
    */

}
