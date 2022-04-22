using System.ComponentModel.DataAnnotations;

namespace ToyotaArchiv.Domain
{
    /*
     * Test vytvorenia Controlera pre entitu ktora nie je v DB;
     * 
     * Po pridani FotoFileName, FotoFormFile, FotoContent Model ModelState.IsValid je True aj ked nie su v Bind
     * public  IActionResult Create([Bind("PersonID,FirstName,LastName")] Person role)
        {
            if (ModelState.IsValid)
     * 
     */
    public class Person
    {

        public Person()
        {

        }
        //public Person(int id,string firstName, string lastName)
        //{
        //    PersonID = id;  
        //    FirstName = firstName;  
        //    LastName = lastName;
        //}
        [Display(Name = "PIN")]
        [Required(ErrorMessage ="Zadajte, prosím, PIN osoby")]
        public int PersonID { get; set; }

        [Display(Name="Meno")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Zadajte údaj na 3 az 20 znakov")]
        public string FirstName { get; set; }

        [StringLength(20, MinimumLength = 3, ErrorMessage = "Zadajte údaj na 3 az 20 znakov")]
        [Display(Name = "Priezvisko")]
        public string LastName { get; set; }



        [Display(Name = "Foto")]
        [MaxLength(64)]
        public string? FotoFileName { get; set; }

        public IFormFile? FotoFormFile { get; set; }

        public byte[]? FotoContent { get; set; }
    }
}
