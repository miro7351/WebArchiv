using PA.TOYOTA.DB;
using ToyotaArchiv.Domain;

namespace ToyotaArchiv.Interfaces
{
    public interface IZakazkaTransformService
    {
        Zakazka ConvertZakazkaZO_To_Zakazka(ZakazkaZO myZakZO);

        ZakazkaZO ConvertZakazka_To_ZakazkaZO(Zakazka zakazka);
    }
}
