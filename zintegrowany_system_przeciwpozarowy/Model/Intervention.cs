using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zintegrowany_system_przeciwpozarowy.Model
{
    public class Intervention
    {
        public int Id { get; set; }
        public int IdOwner { get; set; }
        public int IdEvent { get; set; }
        public DateTime WhenArrive { get; set; }
        public DateTime WhenLeave { get; set; }
        public DateTime WhenEnd { get; set; }
        public int HowLong { get; set; }
        public int InjureVictims { get; set; }
        public int DeadVictims { get; set; }
        public bool Arson { get; set; }
        public string Description { get; set; }


    }
}
