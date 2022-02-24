using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Win32;
using Windows.Win32.UI.Input.KeyboardAndMouse;

using JoyMouse.Interfaces;
using JoyMouse.Models;

namespace JoyMouse.Services
{
    internal class MouseEventService : IMouseEventService
    {
        public void Move(int x, int y)
        {
            PInvoke.mouse_event(MOUSE_EVENT_FLAGS.MOUSEEVENTF_MOVE, x, y, 0, 0);
        }

        public void Scroll(int x, int y)
        {
            if (y != 0)
                PInvoke.mouse_event(MOUSE_EVENT_FLAGS.MOUSEEVENTF_WHEEL, 0, 0, (uint)y, 0);
            if (x != 0)
                PInvoke.mouse_event(MOUSE_EVENT_FLAGS.MOUSEEVENTF_HWHEEL, 0, 0, (uint)x, 0);
        }

        public void SendLeftDown()
        {
            PInvoke.mouse_event(MOUSE_EVENT_FLAGS.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
        }

        public void SendLeftUp()
        {
            PInvoke.mouse_event(MOUSE_EVENT_FLAGS.MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        public void SendRightDown()
        {
            PInvoke.mouse_event(MOUSE_EVENT_FLAGS.MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
        }

        public void SendRightUp()
        {
            PInvoke.mouse_event(MOUSE_EVENT_FLAGS.MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
        }
    }
}
