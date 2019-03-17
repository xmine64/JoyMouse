using System;
using System.Linq;
using System.Windows.Forms;
using SlimDX.DirectInput;
using System.Threading;
using System.Runtime.InteropServices;
using System.Drawing;
using SlimDX.Windows;

namespace JoyMouse
{
    static class Program
    {
        public static NotifyIcon TrayIcon;

        public static DirectInput DirectInput;
        public static Joystick Joystick;
        public static Point PointerLoc;
        public static Rectangle ScreenBound;

        public static bool DoLeftDown = false;
        public static bool DoLeftUp = false;
        public static bool DoneLeftDown = false;

        public static bool DoRightDown = false;
        public static bool DoRightUp = false;
        public static bool DoneRightDown = false;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ScreenBound = Screen.PrimaryScreen.Bounds;
            PointerLoc = new Point(0, 0);
            GetCursorPos(out PointerLoc);
            DirectInput = new DirectInput();
            TrayIcon = new NotifyIcon();
            TrayIcon.Text = "JoyMouse";
            TrayIcon.Visible = true;
            try
            {
                var ar = DirectInput.GetDevices().Where(a => a.Type == DeviceType.Joystick).ToArray();
                var i = 0;
                if (ar.Count() > 1)
                {
                    var dlg = new SelectorDialog(ar.Select(a => ++i + ": " + a.ProductName.Trim() + " (" + a.Type + ")").ToArray());
                    if (dlg.ShowDialog() != DialogResult.OK) return;
                    i = dlg.SelectedItem;
                }
                else if (ar.Count() < 1)
                {
                    MessageBox.Show("No joystick connected.");
                    return;
                }
                Joystick = new Joystick(DirectInput, ar[i].InstanceGuid);
                if (Joystick.Acquire().IsFailure)
                    throw new Exception();
            }
            catch { MessageBox.Show("Error while accessing to Joystick"); return; }

            MessagePump.Run(() =>
            {
                // Prevents from pressuring to CPU
                Thread.Sleep(10);

                // Check is Device connected or not.                    
                if (!DirectInput.IsDeviceAttached(Joystick.Information.InstanceGuid))
                    return;

                // Support of Mouse
                GetCursorPos(out PointerLoc);

                // Get States
                var s = Joystick.GetCurrentState();
                var btns = s.GetButtons();

                // Button Mapping
                if (btns[8])
                    Application.ExitThread();/*
                if (btns[0])
                    throw new NotImplementedException();
                if (btns[1])
                    throw new NotImplementedException();*/
                if (btns[2] || btns[10])
                {
                    if (!DoneLeftDown)
                        if (!DoLeftDown)
                            DoLeftDown = true;
                }
                else
                {
                    if (DoneLeftDown)
                    {
                        if (!DoLeftUp)
                            DoLeftUp = true;
                        DoneLeftDown = false;
                    }
                }
                if (btns[3] || btns[11])
                {
                    if (!DoneRightDown)
                        if (!DoRightDown)
                            DoRightDown = true;
                }
                else
                {
                    if (DoneRightDown)
                    {
                        if (!DoRightUp)
                            DoRightUp = true;
                        DoneRightDown = false;
                    }
                }

                // Scroll
                var sd = ((s.Z - 32767) / GetSpeed(btns)) * -10;
                if (sd != 0)
                    Mouse.Send(MouseEvent.WHEEL, 0, 0, sd);

                // Set Cursor
                //
                PointerLoc.X += (s.X - 32767) / GetSpeed(btns);
                PointerLoc.Y += (s.Y - 32767) / GetSpeed(btns);
                // Prevent from Out of Bounding
                if (PointerLoc.X < ScreenBound.X)
                    PointerLoc.X = ScreenBound.X;
                if (PointerLoc.Y < ScreenBound.Y)
                    PointerLoc.Y = ScreenBound.Y;
                if (PointerLoc.X > ScreenBound.Width)
                    PointerLoc.X = ScreenBound.Width;
                if (PointerLoc.Y > ScreenBound.Height)
                    PointerLoc.Y = ScreenBound.Height;
                // Native Calls
                SetCursorPos(PointerLoc);
                if (DoLeftDown)
                {
                    Mouse.Send(MouseEvent.LEFTDOWN);
                    DoLeftDown = false;
                    DoneLeftDown = true;
                }
                if (DoLeftUp)
                {
                    Mouse.Send(MouseEvent.LEFTUP);
                    DoLeftUp = false;
                }
                if (DoRightDown)
                {
                    Mouse.Send(MouseEvent.RIGHTDOWN);
                    DoRightDown = false;
                    DoneRightDown = true;
                }
                if (DoRightUp)
                {
                    Mouse.Send(MouseEvent.RIGHTUP);
                    DoRightUp = false;
                }
            });
            TrayIcon.Visible = false;
            Joystick.Unacquire();
            MessageBox.Show("The JoyMouse has stoppod.");
        }

        public static int GetSpeed(bool[] btns)
        {
            if (btns[4])
                return 10000;
            if (btns[5])
                return 25000;
            if (btns[6])
                return 1000;
            if (btns[7])
                return 500;
            return 5000;
        }

        [DllImport("user32.dll")]
        public static extern void SetCursorPos(Point pt);

        [DllImport("user32.dll")]
        public static extern void GetCursorPos(out Point pt);
    }

    public class Mouse
    {
        #region Native Codes
        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        /*
        public const int MOUSEEVENTF_MOVE = 0x0001;
        public const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        public const int MOUSEEVENTF_LEFTUP = 0x0004;
        public const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        public const int MOUSEEVENTF_RIGHTUP = 0x0010;
        public const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        public const int MOUSEEVENTF_MIDDLEUP = 0x0040;
        public const int MOUSEEVENTF_ABSOLUTE = 0x8000;
        */
        #endregion

        public static void Send(MouseEvent dw, int dx = 0, int dy = 0, int dwData = 0, int dwExtraInfo = 0)
        {
            mouse_event((int)dw, dx, dy, dwData, dwExtraInfo);
        }
    }

    public enum MouseEvent
    {
        MOVE = 0x0001,
        LEFTDOWN = 0x0002,
        LEFTUP = 0x0004,
        RIGHTDOWN = 0x0008,
        RIGHTUP = 0x0010,
        MIDDLEDOWN = 0x0020,
        MIDDLEUP = 0x0040,
        WHEEL = 0x0800,
        ABSOLUTE = 0x8000,
    }
}
