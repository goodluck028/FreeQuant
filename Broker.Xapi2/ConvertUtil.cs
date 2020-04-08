using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeQuant.Framework;
using XAPI;

namespace Broker.Xapi2 {
    static class ConvertUtil {
        /// <summary>
        /// 转换交易方向
        /// </summary>
        /// <param name="side"></param>
        /// <returns></returns>
        public static DirectionType ConvertDirectionType(OrderSide side){
            return side == OrderSide.Buy ? DirectionType.Buy : DirectionType.Sell;
        }

        /// <summary>
        /// 转换开平
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static FreeQuant.Framework.OpenCloseType ConvertOpenCloseType(XAPI.OpenCloseType type) {
            switch (type) {
                case XAPI.OpenCloseType.Close:
                    return FreeQuant.Framework.OpenCloseType.CloseToday;
                case XAPI.OpenCloseType.CloseToday:
                    return FreeQuant.Framework.OpenCloseType.CloseToday;
                default:
                    return FreeQuant.Framework.OpenCloseType.Open;
            }
        }

        /// <summary>
        /// 转换订单状态
        /// </summary>
        /// <param name="brokerStatus"></param>
        /// <returns></returns>
        public static FreeQuant.Framework.OrderStatus ConvertOrderStatus(XAPI.OrderStatus brokerStatus) {
            FreeQuant.Framework.OrderStatus status = FreeQuant.Framework.OrderStatus.Normal;
            switch (brokerStatus) {
                case XAPI.OrderStatus.NotSent:
                case XAPI.OrderStatus.PendingNew:
                case XAPI.OrderStatus.New:
                    status = FreeQuant.Framework.OrderStatus.Normal;
                    break;
                case XAPI.OrderStatus.Rejected:
                case XAPI.OrderStatus.Expired:
                    status = FreeQuant.Framework.OrderStatus.Error;
                    break;
                case XAPI.OrderStatus.PartiallyFilled:
                    status = FreeQuant.Framework.OrderStatus.Partial;
                    break;
                case XAPI.OrderStatus.Filled:
                    status = FreeQuant.Framework.OrderStatus.Filled;
                    break;
                case XAPI.OrderStatus.Cancelled:
                    status = FreeQuant.Framework.OrderStatus.Canceled;
                    break;
                case XAPI.OrderStatus.PendingCancel:
                case XAPI.OrderStatus.PendingReplace:
                case XAPI.OrderStatus.Replaced:
                    break;
            }
            return status;
        }

        /// <summary>
        /// 转换连接状态
        /// </summary>
        /// <param name="borkerStatus"></param>
        /// <returns></returns>
        public static FreeQuant.Framework.ConnectionStatus ConvertConnectionStatus(XAPI.ConnectionStatus borkerStatus) {
            switch (borkerStatus) {
                case XAPI.ConnectionStatus.Done:
                    return FreeQuant.Framework.ConnectionStatus.Connected;
                case XAPI.ConnectionStatus.Disconnected:
                    return FreeQuant.Framework.ConnectionStatus.Disconnected;
                default:
                    return FreeQuant.Framework.ConnectionStatus.Connecting;
            }
        }

        /// <summary>
        /// 转换交易所
        /// </summary>
        /// <param name="exchangeID"></param>
        /// <returns></returns>
        public static Exchange ConvertExchange(string exchangeID) {
            switch (exchangeID) {
                case "SHFE":
                    return Exchange.SHFE;
                case "DCE":
                    return Exchange.DCE;
                case "CZCE":
                    return Exchange.CZCE;
                case "CFFEX":
                    return Exchange.CFFEX;
                default:
                    return Exchange.Unknown;
            }
        }
    }
}
