using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zintegrowany_system_przeciwpozarowy
{
    public class Event
    {   [Key]
        public int Id { get; set; }
        [Display(Name = "Id sensora z którego nastąpiło zdarzenie")]
        public int IdSensor { get; set; }
        [Display(Name = "Data zdarzenia")]
        public DateTime Date { get; set; }
        [Display(Name = "Pożar?")]
        public bool RealEvent { get; set; }
    }
}
