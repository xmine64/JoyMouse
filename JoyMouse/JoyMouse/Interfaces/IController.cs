using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JoyMouse.Models;

namespace JoyMouse.Interfaces
{
    public interface IController : IDisposable
    {
        public string Name { get; }

        public IDictionary<int, ControllerButton> KeyMap { get; }

        ControllerState State { get; }

        bool IsAttached();

        void StartListening();

        void StopListening();

        event EventHandler StateChanged;
    }
}
