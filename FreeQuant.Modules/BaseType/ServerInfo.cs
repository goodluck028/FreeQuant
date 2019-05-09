using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Modules {
    public class ServerInfo
    {
        private string appPwd;

        public ServerInfo(string appPwd)
        {
            this.appPwd = appPwd;
        }

        public string AppPwd
        {
            get { return appPwd; }
            set { appPwd = value; }
        }
    }
}
