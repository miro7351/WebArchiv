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

        public static string SessionLogin => "_Login";
        public static string SessionRole => "_Role";


    }
}
