using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeQuant.Framework;

namespace FreeQuant.Components {
    public class FrameworkDispatcher {
        public static FrameworkDispatcher mInstance;
        private FrameworkDispatcher() {
            EventBus.Register(this);
            //登录行情
            MdBrokerLoginRequest request = new MdBrokerLoginRequest();
            EventBus.PostEvent(request);
        }
        public static void Run() {
            if (mInstance == null) {
                mInstance = new FrameworkDispatcher();
            }
        }

        //todo 响应行情登录成功
        [OnEvent]
        public void OnMdLogin(MdBrokerLoginEvent evt)
        {

        }
    }
}
