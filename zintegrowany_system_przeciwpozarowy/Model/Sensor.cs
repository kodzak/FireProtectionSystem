using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zintegrowany_system_przeciwpozarowy
{
    public class Sensor
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Numer telefonu czujnika")]
        public string SensorNumber { get; set; }
        public int OwnerId { get; set; }
        [Display(Name = "Miejsce montażu")]
        public string Place { get; set; }
    }
}
