using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace FreeQuant.Framework {
    public class StrategyEntity {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string ClassType { get; set; }
        public string Name { get; set; }
        public string Enable { get; set; }
    }

    public class PositionEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Indexed]
        public string StrategyId { get; set; }
        public string InstrumentId { get; set; }
        public int Position { get; set; }
        public DateTime LastTime { get; set; }
    }

    public class OrderEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Indexed]
        public string OrderId { get; set; }
        [Indexed]
        public string StrategyId { get; set; }
        public string InstrumentId { get; set; }
        public string Direction { get; set; }
        public double Price { get; set; }
        public long Volume { get; set; }
        public long VolumeTraded { get; set; }
        public DateTime OrderTime { get; set; }
    }
}
