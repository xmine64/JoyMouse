using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JoyMouse.Models;

namespace JoyMouse.Interfaces
{
    public interface IMouseEventService
    {
        void SendLeftDown();

        void SendLeftUp();

        void SendRightDown();

        void SendRightUp();

        void Move(int x, int y);

        void Scroll(int x, int y);
    }
}
