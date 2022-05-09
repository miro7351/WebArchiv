namespace ToyotaArchiv.Global
{
    //MH: 14.04.2022
    //Staticke readonly parametre, pristupne z celej aplikacie
    public class AppData
    {

        /*
         * AutoOpenFile nastavuje sa v appconfig.json
         * AutoOpenFile = false; ak uzivatel downloaduje dokument zo servera subor sa mu neotvori automaticky,
         * ale v browseri sa otvori dialogove okno a tam si moze zvolit "Open file"
         * AutoOpenFile = true; ak uzivatel downloaduje dokument zo servera subor sa mu otvori automaticky v novom okne browsera;
         */
        public static bool AutoOpenFile = false;

        /// <summary>
        /// Identifikator pre zapis/citanie hodnoty zo session
        /// </summary>
        public static string SessionLogin => "_Login";
        /// <summary>
        /// Identifikator pre zapis/citanie hodnoty zo session
        /// </summary>
        public static string SessionRole => "_Role";
        /// <summary>
        /// Identifikator pre zapis/citanie hodnoty zo session
        /// </summary>
        public static string SessionUser => "_User";


    }
}
