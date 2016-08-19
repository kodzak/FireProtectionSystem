using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zintegrowany_system_przeciwpozarowy.Model;

namespace zintegrowany_system_przeciwpozarowy
{
    
        public class EdContext : DbContext
        {
            public EdContext()
                : base("Default")
            { }

            public DbSet<Owner> Owner { get; set; }
            public DbSet<Sensor> Sensors { get; set; }
            public  DbSet<Event> Events { get; set; }
            public DbSet<Intervention> Interventions { get; set; }
            public DbSet<Logs> Logs { get; set; }
            public DbSet<Maintenance> Maintenances { get; set; }

        }
    
}
