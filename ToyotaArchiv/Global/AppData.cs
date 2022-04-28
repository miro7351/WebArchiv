using ToyotaArchiv.Infrastructure;

namespace ToyotaArchiv.Global
{
    //MH: 14.04.2022
    //Staticke readonly parametre, pristupne z celej aplikacie
    public class AppData
    {
        static AppData()
        {
            CurrentUserDetail = new UserDetail();   
        }
        public static UserDetail CurrentUserDetail { get; set; }

        public static void LogoutCurrentUser()
        {
            CurrentUserDetail.UserLogin = string.Empty; 
            CurrentUserDetail.UserRole  = "READONLY";
        }

        public static void SetCurrentUser( string login, string role)
        {
            CurrentUserDetail.UserLogin=login;
            CurrentUserDetail.UserRole = role;  
        }

        /*
         * AutoOpenFile nastavuje sa v appconfig.json
         * AutoOpenFile = false; ak uzivatel downloaduje dokument zo servera subor sa mu neotvori automaticky,
         * ale v browseri sa otvori dialogove okno a tam si moze zvolit "Open file"
         * AutoOpenFile = true; ak uzivatel downloaduje dokument zo servera subor sa mu otvori automaticky v novom okne browsera;
         */
        public static bool AutoOpenFile = false;

        public static string SessionLogin => "_Login";
        public static string SessionRole => "_Role";


    }
}
