using System;

using JoyMouse.Interfaces;

namespace JoyMouse.Models
{
    public class ControllerContainerEventArgs : EventArgs
    {
        public ControllerContainerEventArgs(IController target)
        {
            Target = target;
        }

        public IController Target { get; }
    }
}
