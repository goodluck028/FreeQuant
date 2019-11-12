using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace FreeQuant.Framework {
    class MyDbContext :DbContext{
        public MyDbContext():base("FqData") {}
        public virtual DbSet<T_Strategy> Strategys { get; set; }
        public virtual DbSet<T_Position> Positions { get; set; }
        public virtual DbSet<T_Order> Orders { get; set; }
    }
}
