using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SharpDX.DirectInput;

using JoyMouse.Interfaces;

namespace JoyMouse.Models
{
    internal class JoystickController : IController
    {
        private readonly DirectInput _directInput;
        private readonly Guid _guid;
        private readonly Joystick _joystick;

        private bool _isListening = false;
        private Thread _listenThread;
        private AutoResetEvent _listenWaitHadle;

        public event EventHandler? StateChanged;

        public JoystickController(DirectInput directInput, Guid guid)
        {
            _directInput = directInput;
            _guid = guid;
            _joystick = new Joystick(_directInput, _guid);
            _listenThread = new Thread(ListenForStateChange);
            _listenWaitHadle = new AutoResetEvent(false);
            State = new ControllerState();
        }

        public string Name => _joystick.Information.InstanceName;

        public IDictionary<int, ControllerButton> KeyMap { get; } =
            new Dictionary<int, ControllerButton>()
            {
                { 2, ControllerButton.LeftButton },
                { 10, ControllerButton.LeftButton },
                { 3, ControllerButton.RightButton },
                { 11, ControllerButton.RightButton },
                { 4, ControllerButton.Decelerate2x },
                { 5, ControllerButton.Decelerate4x },
                { 6, ControllerButton.Accelerate2x },
                { 7, ControllerButton.Accelerate4x },
                { 8, ControllerButton.ShowSettings },
                { 9, ControllerButton.Exit },
            };

        public ControllerState State { get; private set; }

        public bool IsAttached()
        {
            return _directInput.IsDeviceAttached(_guid);
        }

        public void Dispose()
        {
            StopListening();
            _joystick.Dispose();
            _listenWaitHadle.Dispose();
        }

        public void StartListening()
        {
            if (_isListening) return;
            _isListening = true;
            _joystick.SetNotification(_listenWaitHadle);
            _joystick.Acquire();
            _listenThread.Start();
        }

        public void StopListening()
        {
            if (_isListening)
            {
                _isListening = false;
                _listenWaitHadle.Set();
                _joystick.Unacquire();
            }
        }

        private void ListenForStateChange()
        {
            JoystickState? state = null;

            while (true)
            {
                if (!_listenWaitHadle.WaitOne())
                    return;

                if (!_isListening)
                    return;

                var newState = _joystick.GetCurrentState();

                for (int i = 0; i < newState.Buttons.Length; i++)
                {
                    if (newState.Buttons[i] != state?.Buttons[i] &&
                        KeyMap.ContainsKey(i))
                    {
                        State.Buttons[KeyMap[i]] = newState.Buttons[i];
                        StateChanged?.Invoke(this, EventArgs.Empty);
                    }
                }

                state = newState;

                var x = (state.X - 32767) / 5000;
                var y = (state.Y - 32767) / 5000;
                if (x != State.PointerX || y != State.PointerY)
                {
                    State.PointerX = x;
                    State.PointerY = y;
                    StateChanged?.Invoke(this, EventArgs.Empty);
                }

                var scrollX = (state.Z - 32767) / -1000;
                var scrollY = (state.RotationZ - 32767) / 1000;
                if (scrollX != State.ScrollX || scrollY != State.ScrollY)
                {
                    State.ScrollX = scrollX;
                    State.ScrollY = scrollY;
                    StateChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}
