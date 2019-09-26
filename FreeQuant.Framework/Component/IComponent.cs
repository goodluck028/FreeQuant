using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Framework {
    public interface IComponent {
        void OnLoad();
        void OnReady();
    }
}
