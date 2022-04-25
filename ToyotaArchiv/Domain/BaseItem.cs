using System;
using System.ComponentModel.DataAnnotations;

namespace ToyotaArchiv.Domain
{
    //MH: april 2022
    /// <summary>
    /// Datovy typ obsahuje udaje o subore (subor typu *.txt, *.jpeg, *.img, *.pdf, *.doc,...), ktory patri zakazke;
    /// Zakazka ma viac priradenych suborov;
    /// Skupina=1, NazovSuboru: ZakazkaTG
    /// Skupina=2, NazovSuboru: ZakazkaTB
    /// Skupina=20,...,99  Povinne dokumenty
    /// Skupina=100,101,... Prilohy
    /// </summary>
    public class BaseItem   //Je to ViewModel pre typ Dokument; lepsi nazov by bol DokumentViewModel ( BaseDokumentViewModel,DokumentVM ??)
    {
        public BaseItem()
        {
            //FileItem = new FileItem();
        }
     
        /// <summary>
        /// Nazov polozky
        /// </summary>
        public string? NazovDokumentu   //Nazov: napr. Foto1, BlokMotora2, LaveDvere,... nazvy pre jednu zakazku musia byt rozne!!!
        {
            get;
            set;
        }


        /// <summary>
        /// Nazov polozky, ZAPISUJE SA DO DB
        /// </summary>
        public string? NazovSuboru   //Nazov: suboru napr. Foto1.jpg, PopisMotora.pdf; nazvy suborov pre jednu zakazku musia byt rozne!
        {
            get;
            set;
        }

      
        /// <summary>
        /// Nazov polozky, NEZAPISUJE SA DO DB;
        /// Polozka sa nastavuje pomocou Drop operacie, alebo pri vybere suboru, neda sa editovat rucne!!!
        /// </summary>
        public string? FilePath   //Cesta pre subor napr. C:\Zakazky\Opravy\Foto1.jpg, C:\Dokumenty\PopisMotora.pdf; nazvy suborov pre jednu zakazku musia byt rozne!
        {
            get;
            set;
        }

       /*
        /// <summary>
        /// Udava platnost polozky
        /// </summary>
        public string DokumentPlatny //A/N
        {
            get;
            set;
        }
        */

       
        /// <summary>
        /// Poznamka pre polozku
        /// </summary>
        public string? Poznamka 
        {
            get;
            set;
        }

        /*MH: poznamky
          *  
          * ZakazkaTG Skupina=1
          * ZakazkaTB Skupina=2
          * Povinne subory: Skupina=20,..99
          * Prilohy Skupina=100, 101,....
          * 
          * 
          * Vsetky zaznamy typu BaseItem su v databaze ulozene v jednej tabulke Dokument,
          * Aby som vedel rozlisit k akej skupine dokumentov dokument patri zaviedol som v db. tab Stlpec: Skupina;
          * 
          */

       
        /// <summary>
        /// Urcuje k akemu dokumentu patri polozka
        /// </summary>
        public short? Skupina
        {
            get;
            set;
        }


        [MaxLength]
        public byte[]? FileContent { get; set; }  //obsah suboru prijateho z klienta

        //[Required]
        [Display(Name = "Priložený dokument")]
        public IFormFile? DokFormFile { get; set; } = null!;

        /*Pouziva sa pri nastaveni IsEnbled, Visibility, DropEnabled properties  
         * 
         * Pred zapisom do databazy ak je zadany FileName nastavim Completed="Y";
           Ak sa nacita nazov suboru z databazy, potom sa uz nemoze menit nazov suboru, 
           aby som vedel ze sa uz nemoze menit nazov suboru pouzijem stav Completed
         */
        //private string completed;  //zmena povolena??
        /// <summary>
        /// Priznak ci sa moze/nemoze menit hodnota FileName vo FileItem;
        /// Completed="N" moze sa menit nazov suboru ked sa robi UPDATE;
        /// Completed="Y" nemoze sa menit nazov suboru ked sa robi UPDATE;
        /// </summary>
        //public string Completed
        //{
        //    get => completed;
        //    set => SetValue(value, ref completed);
        //}

        /*
         * Subory do jednej zakazky moze pridavat viac uzivatelov, preto pre sledovanie historie zaznamu
         * je potrebne mat aj tieto properties
       
        public string? Vytvoril { get; set; }

        public DateTime? Vytvorene { get; set; }
        public string? Zmenil { get; set; }
        public DateTime? Zmenene { get; set; }
  */
      
    }
}
