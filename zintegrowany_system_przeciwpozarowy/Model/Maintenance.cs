using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace zintegrowany_system_przeciwpozarowy.Model
{
    public class Maintenance
    {
        public int Id { get; set; }
        public int IdOwner { get; set; }
        public int IdSensor { get; set; }
        public DateTime DataConservation { get; set; }
        public DateTime DataNextConservation { get; set; }
        public bool Work { get; set; }

    }
}
