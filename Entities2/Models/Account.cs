
using System.ComponentModel.DataAnnotations;

namespace PA.TOYOTA.DB
{
    public partial class Account
    {
        public int LoginId { get; set; }
        [Required( ErrorMessage="Údaj je povinný")]
        [StringLength(16, ErrorMessage = "Zadajte údaj na max. 16 znakov")]
        [Display(Name = "Účet")]
        public string LoginName { get; set; } = null!; //varchar(16), not null

        [Required(ErrorMessage = "Údaj je povinný")]
        [StringLength(16, ErrorMessage = "Zadajte údaj na max. 16 znakov")]
        public string LoginPassword { get; set; } = null!;//varchar(16), not null

        [Required(ErrorMessage = "Údaj je povinný")]
        [StringLength(16, ErrorMessage = "Vyberte údaj")]
        public string LoginRola { get; set; } = null!; //varchar(16), not null

        [StringLength(16, ErrorMessage = "Zadajte údaj na max. 16 znakov")]
        [Display(Name = "DB login")]
        public string? DbLogin { get; set; }//nvarchar(16),  null

        [StringLength(16, ErrorMessage = "Zadajte údaj na max. 16 znakov")]
        [Display(Name = "DB heslo")]
        public string? DbPassword { get; set; }

        [Required(ErrorMessage = "Údaj je povinný")]
        [Display(Name = "Aktívny")]   //bit, not null; default=1
        public bool Aktivny { get; set; }
        // @Html.CheckBoxFor(m=>m.Aktivny.Value) bude tam checkbox!!
        //bool? Aktivny nefunguje!!  (ModelState.IsValid) je false!!
        //bool Aktivny nefunguje!!  (ModelState.IsValid) je true!!
    }
}
