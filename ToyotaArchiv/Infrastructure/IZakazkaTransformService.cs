using PA.TOYOTA.DB;
using ToyotaArchiv.Domain;

namespace ToyotaArchiv.Infrastructure
{
    //MH: april 2022
    //funcie pre transformaciu typu ZakazkaZO <-> Zakazka
    //ZakazkaZO je viewmodel pre typ Zakazka, len nazov |ZakazkaZO  nie je velmi dobry;
    public interface IZakazkaTransformService
    {
      
        Zakazka ConvertZakazkaZO_To_NewZakazka(ref ZakazkaZO myZakZO);
        ZakazkaZO ConvertZakazka_To_ZakazkaZO(ref Zakazka zakazka);

        void ConvertZakazkaZO_To_Zakazka(ref ZakazkaZO myZakZO, ref Zakazka zakazkaDB);

        ZakazkaZO VytvorPrazdnuZakazkuZO(short pocetPriloh);

    }
}
