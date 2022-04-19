namespace ToyotaArchiv.Global
{
    //MH: 14.04.2022
    //Staticke readonly parametre, pristupne z celej aplikacie
    public class AppData
    {
        public static string SessionName => "_Name";
        public static string SessionAge => "_Age";
        public static string SessionTime => "_SessionTime";

        public static string SessionLogin => "_Login";
        public static string SessionRole => "_Role";
    }
}
