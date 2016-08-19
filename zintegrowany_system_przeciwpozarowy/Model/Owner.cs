using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zintegrowany_system_przeciwpozarowy
{
    public class Owner
    {    [Key]
        public int Id { get; set; }
        [Display(Name = "Imie właściciciela")]
        public string Name { get; set; }
        [Display(Name = "Nazwisko właściciela")]
        public string Surname { get; set; }
        [Display(Name = "Adres zamieszkania")]
        public string Address { get; set; }
        [Display(Name = "Numer telefonu właściciela")]
        public string OwnerNumber { get; set; }
        [Display(Name = "Typ budynku")]
        public string TypeOfBulding { get; set; }

    }
}

