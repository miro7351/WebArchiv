using System.ComponentModel.DataAnnotations;

namespace PA.TOYOTA.DB
{
    public partial class Account
    {
        public int LoginId { get; set; }

        [Required(ErrorMessage ="Musíte zadať názov účtu.")]
        [StringLength(16, ErrorMessage ="Maximálny počet znakov je 16.")]
        [Display(Name = "Účet")]
        public string LoginName { get; set; } = null!;  //napr.admin user1, technik, technik2
        //lepsie by to bolo AccountName - nazov uzivatelskeho uctu;

        [Required(ErrorMessage = "Musíte zadať heslo.")]
        [StringLength(16, ErrorMessage = "Maximálny počet znakov je 16.")]
        [Display(Name = "Heslo")]
        public string LoginPassword { get; set; } = null!;

        [Required(ErrorMessage = "Musíte vybrať rolu užívateľa.")]
        [Display(Name = "Rola")]
        public string LoginRola { get; set; } = null!;


        /// <summary>
        /// Login do databazy ak kazdy uzivatel bude mat svoj db login
        /// </summary>
        [StringLength(16, ErrorMessage = "Maximálny počet znakov je 16.")]
        [Display(Name = "DB login")]
        public string? DbLogin { get; set; }

        /// <summary>
        ///  Heslo do databazy ak kazdy uzivatel bude mat svoj db login
        /// </summary>
        [StringLength(16, ErrorMessage = "Maximálny počet znakov je 16.")]
        [Display(Name = "Db heslo")]
        public string? DbPassword { get; set; }

        /// <summary>
        /// Priznak urcuje ci dany  ucet je platny-aktivny;
        /// Zaznamy z tab. Account sa nemazu, len sa nastavuje Aktivny=false, ak sa chce zabranit
        /// pouzivat dany ucet;
        /// </summary>
        [Display(Name = "Aktívny")]
        public bool Aktivny { get; set; }  //na stranke sa zobrazi checkbox
        //MH: VS tu nastavi bool? Aktivny, hoci v db je not null!!
        //bool? Aktivny nefunguje pre checkBox
        //MUSI TO BYT bool Aktivny, checkbox je OK!!!!
    }
}
