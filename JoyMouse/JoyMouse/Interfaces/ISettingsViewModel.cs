using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoyMouse.Interfaces
{
    public interface ISettingsViewModel
    {
        IList<IController> Controllers { get; }

        IController SelectedController { get; set; }
    }
}
