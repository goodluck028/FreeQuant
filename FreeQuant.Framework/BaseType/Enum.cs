using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Framework {
    public enum DirectionType {
        /// <summary>
        /// 买
        /// </summary>
        Buy,

        /// <summary>
        /// 卖
        /// </summary>
        Sell
    }

    public enum OpenCloseType {
        /// <summary>
        /// 自动
        /// </summary>
        Auto,
        /// <summary>
        /// 开仓
        /// </summary>
        Open,
        /// <summary>
        /// 平仓
        /// </summary>
        Close,
        /// <summary>
        /// 平今
        /// </summary>
        CloseToday
    }

    public enum OrderStatus {
        /// <summary>
        /// 委托
        /// </summary>
        Normal,
        /// <summary>
        /// 部成
        /// </summary>
        Partial,
        /// <summary>
        /// 全成
        /// </summary>
        Filled,
        /// <summary>
        /// 撤单[某些"被拒绝"的委托也会触发此状态]
        /// </summary>
        Canceled,
        /// <summary>
        /// 错误
        /// </summary>
        Error
    }

    public enum ConnectionStatus {
        /// <summary>
        /// 连接已经断开
        /// </summary>
        Disconnected,
        /// <summary>
        /// 连接中...
        /// </summary>
        Connecting,
        /// <summary>
        /// 连接成功
        /// </summary>
        Connected
    }
}
