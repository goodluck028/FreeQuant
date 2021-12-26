using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Framework {
    public class Instrument {
        public Instrument(string instrumentID, string productID, Exchange exchange, int volumeMultiple, double priceTick, int maxOrderVolume) {
            InstrumentID = instrumentID;
            ProductID = productID;
            Exchange = exchange;
            VolumeMultiple = volumeMultiple;
            PriceTick = priceTick;
            MaxOrderVolume = maxOrderVolume;
        }

        /// 合约代码
        public string InstrumentID { get; private set; }

        /// 品种代码
        public string ProductID { get; private set; }

        /// 交易所代码
        public Exchange Exchange { get; private set; }

        /// 合约数量乘数
        public int VolumeMultiple { get; private set; }

        /// 最小变动价位
        public double PriceTick { get; private set; }

        /// 最大委托量[限价]
        public int MaxOrderVolume { get; private set; }
    }

    public enum Exchange {
        /// 大商所
        DCE,
        /// 郑商所
        CZCE,
        /// 上期所
        SHFE,
        /// 中金所
        CFFEX,
        /// 未知
        Unknown
    }
}
