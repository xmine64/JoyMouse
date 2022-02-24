using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JoyMouse.Models;

namespace JoyMouse.Interfaces
{
    public interface IControllerService
    {
        IReadOnlyList<IController> Controllers { get; }

        void SetActiveController(IController? controller);

        void StartService();

        void StopService();

        event EventHandler<ControllerContainerEventArgs> ControllerAdded;
        event EventHandler<ControllerContainerEventArgs> ControllerRemoved;
        event EventHandler ExitButtonPressed;
        event EventHandler SettingsButtonPressed;
    }
}
