
namespace ToyotaArchiv.Infrastructure
{
    //MH: april 2022
    /*
     * Po prihlaseni uzivatela nastavit do session
     * 
     * UserDetail userDetail = new UserDetail( myLogin, myRole, myDbLogin, myDbPassword);
       SessionWrite(ToyotaArchiv.Global.SessionUser, userDetail)

       pre pouzitie v *Controller napr. metoda Index()

      ViewBag.User = HttpContext.Session.SessionRead<UzivatelDetail>(ToyotaArchiv.Global.SessionUser);
      
    
      UzivatelDetail user = HttpContext.Session.SessionRead<UzivatelDetail>(ToyotaArchiv.Global.SessionUser);
      ViewBag.UzivatelLogin = user.UzivatelLogin;

    Index.cshtml
    @using  ToyotaArchiv.Infrastructure;

    var user = ViewBag.User as UzivatelDetail;
    string userLogin = ViewBag.UzivatelLogin;

    */
    public class UserDetail
    {
        public UserDetail()
        {
            UserLogin = string.Empty;
            UserRole  = "READONLY";
            UserPassword = string.Empty;    
        }

        public UserDetail(string login, string rola)
        {
            UserLogin = login;
            UserRole = rola;
            UserPassword = string.Empty;
        }

        public UserDetail(string login, string rola, string dbLogin, string dbPassword)
        {
            UserLogin = login;
            UserRole = rola;
            UserPassword = string.Empty;
            DbLogin = dbLogin;  
            DbPassword = dbPassword;
        }

        /// <summary>
        /// Login uzivatela  (Account.LoginName)   do aplikacie;
        /// </summary>
        public string UserLogin { get; set; }

        /// <summary>
        /// Heslo uzivatela  (Account.LoginPassword)   do aplikacie;
        /// </summary>
        public string UserPassword { get; set; }


        /// <summary>
        /// Rola uzivatela  (Account.LoginRola)   do aplikacie, nacitana z db tab. Account
        /// </summary>
        public string UserRole { get; set; }

        /// <summary>
        /// Login do databazy
        /// </summary>
        public string? DbLogin { get; set; }

        /// <summary>
        /// Password do databazy
        /// </summary>
        public string? DbPassword { get; set; }
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
