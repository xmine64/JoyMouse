using System;
using System.Collections.Generic;

namespace JoyMouse.Models
{
    public class ControllerState
    {
        public int PointerX { get; set; }

        public int PointerY { get; set; }

        public int ScrollX { get; set; }

        public int ScrollY { get; set; }

        public Dictionary<ControllerButton, bool> Buttons { get; } = new()
        {
            { ControllerButton.MoveUp, false },
            { ControllerButton.MoveDown, false },
            { ControllerButton.MoveLeft, false },
            { ControllerButton.MoveRight, false },
            { ControllerButton.LeftButton, false },
            { ControllerButton.RightButton, false },
            { ControllerButton.ScrollUp, false },
            { ControllerButton.ScrollDown, false },
            { ControllerButton.ScrollLeft, false },
            { ControllerButton.ScrollRight, false },
            { ControllerButton.Accelerate2x, false },
            { ControllerButton.Accelerate4x, false },
            { ControllerButton.Decelerate2x, false },
            { ControllerButton.Decelerate4x, false },
            { ControllerButton.Exit, false },
            { ControllerButton.ShowSettings, false },
        };
    }
}
