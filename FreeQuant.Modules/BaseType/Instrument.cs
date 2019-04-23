using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Modules {
    public class Instrument
    {
		/// 合约代码
        private string mInstrumentID;

        /// 品种代码
        private string mProductID;

        /// 交易所代码
        private Exchange mExchange = Exchange.SHFE;

        /// 合约数量乘数
        private int mVolumeMultiple;

        /// 最小变动价位
        private double mPriceTick;

        /// 最大委托量[限价]
        private int mMaxOrderVolume;

        public Instrument(string instrumentID, string productID, Exchange exchange, int volumeMultiple, double priceTick, int maxOrderVolume)
        {
            mInstrumentID = instrumentID;
            mProductID = productID;
            mExchange = exchange;
            mVolumeMultiple = volumeMultiple;
            mPriceTick = priceTick;
            mMaxOrderVolume = maxOrderVolume;
        }

        public string InstrumentID
        {
            get
            {
                return mInstrumentID;
            }
        }

        public string ProductID
        {
            get
            {
                return mProductID;
            }
        }

        public Exchange ExchangeID
        {
            get
            {
                return mExchange;
            }
        }

        public int VolumeMultiple
        {
            get
            {
                return mVolumeMultiple;
            }
        }

        public double PriceTick
        {
            get
            {
                return mPriceTick;
            }
        }

        public int MaxOrderVolume
        {
            get
            {
                return mMaxOrderVolume;
            }
        }
    }

    public enum Exchange
    {
        /// 大商所
        DCE,
        /// 郑商所
        CZCE,
        /// 上期所
        SHFE,
        /// 中金所
        CFFEX,
        /// 上海国际能源交易中心股份有限公司
        INE,
    }
}
