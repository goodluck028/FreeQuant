using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Framework {
    class T_Strategy {
        [Key]
        public string ClassName { get; set; }
        public string Name { get; set; }
        public bool Enable { get; set; }
        //
        public ICollection<T_Position> Positions;
        public ICollection<T_Order> Orders;
    }

    class T_Position {
        [Key]
        [Column(Order = 1)]
        public T_Strategy Strategy { get; set; }
        [Key]
        [Column(Order = 2)]
        public string InstrumentID { get; set; }
        public long Position { get; set; }
        public DateTime LastTime { get; set; }
    }

    class T_Order {
        [Key]
        public string OrderId { get; set; }
        public T_Strategy Strategy { get; set; }
        public string InstrumentID { get; set; }
        public string Direction { get; set; }
        public double Price { get; set; }
        public long Volume { get; set; }
        public long VolumeTraded { get; set; }
        public DateTime OrderTime { get; set; }
    }
}
