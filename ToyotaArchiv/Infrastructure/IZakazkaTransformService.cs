using PA.TOYOTA.DB;
using ToyotaArchiv.Domain;

namespace ToyotaArchiv.Infrastructure
{
    public interface IZakazkaTransformService
    {
      
        Zakazka ConvertZakazkaZO_To_Zakazka(ref ZakazkaZO myZakZO);
        ZakazkaZO ConvertZakazka_To_ZakazkaZO(ref Zakazka zakazka);

        void ConvertZakazkaZO_To_Zakazka(ref ZakazkaZO myZakZO, ref Zakazka zakazkaDB);


    }
}
