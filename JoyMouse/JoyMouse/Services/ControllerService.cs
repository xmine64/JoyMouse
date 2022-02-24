using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SharpDX.DirectInput;

using JoyMouse.Interfaces;
using JoyMouse.Models;

namespace JoyMouse.Services
{
    internal class ControllerService : IControllerService
    {
        private readonly DirectInput _directInput = new();
        private readonly IMouseEventService _mouseEventService;

        private IController? _activeController;
        private bool _isServiceRunning;
        private List<IController> _controllers;

        private Thread _controllerServiceThread;
        private readonly AutoResetEvent _controllerStateChanged = new(false);

        public ControllerService(IMouseEventService mouseEventService)
        {
            _controllerServiceThread = new Thread(ControllerServiceRunner);
            _mouseEventService = mouseEventService;
            _controllers = EnumerateControllers().ToList();
        }

        public IReadOnlyList<IController> Controllers => _controllers;

        public IEnumerable<IController> EnumerateControllers()
        {
            foreach (var device in _directInput.GetDevices())
            {
                if (device.Type == DeviceType.Joystick)
                {
                    var controller = new JoystickController(_directInput, device.InstanceGuid);
                    controller.StateChanged += Controller_StateChanged;
                    yield return controller;
                }
            }
        }

        public void SetActiveController(IController? controller)
        {
            if (_activeController != null)
            {
                _activeController.StopListening();
            }
            _activeController = controller;
            if (_isServiceRunning)
                _activeController?.StartListening();
        }

        public void StartService()
        {
            if (_isServiceRunning) return;
            _isServiceRunning = true;
            _activeController?.StartListening();
            _controllerServiceThread.Start();
        }

        public void StopService()
        {
            _activeController?.StopListening();
            _isServiceRunning = false;
        }

        private void Controller_StateChanged(object? sender, EventArgs args)
        {
            if (!_isServiceRunning)
                return;

            if (_activeController?.State.Buttons[ControllerButton.ShowSettings] ?? false)
            {
                SettingsButtonPressed?.Invoke(this, EventArgs.Empty);
            }

            if (_activeController?.State.Buttons[ControllerButton.Exit] ?? false)
            {
                ExitButtonPressed?.Invoke(this, EventArgs.Empty);
            }

            _controllerStateChanged.Set();
        }

        private void ControllerServiceRunner()
        {
            var lastLeftState = false;
            var lastRightState = false;

            while (_isServiceRunning && _activeController != null)
            {
                var x = _activeController.State.PointerX;
                var y = _activeController.State.PointerY;
                if (x != 0 || y != 0)
                {
                    if (_activeController.State.Buttons[ControllerButton.Accelerate2x])
                    {
                        x *= 2;
                        y *= 2;
                    }
                    if (_activeController.State.Buttons[ControllerButton.Accelerate4x])
                    {
                        x *= 4;
                        y *= 4;
                    }
                    if (_activeController.State.Buttons[ControllerButton.Decelerate2x])
                    {
                        x /= 2;
                        y /= 2;
                    }
                    if (_activeController.State.Buttons[ControllerButton.Decelerate4x])
                    {
                        x /= 4;
                        y /= 4;
                    }
                    _mouseEventService.Move(x, y);
                }

                var scrollX = _activeController.State.ScrollX;
                var scrollY = _activeController.State.ScrollY;
                if (scrollX != 0 || scrollY != 0)
                {
                    if (_activeController.State.Buttons[ControllerButton.Accelerate2x])
                    {
                        scrollX *= 2;
                        scrollY *= 2;
                    }
                    if (_activeController.State.Buttons[ControllerButton.Accelerate4x])
                    {
                        scrollX *= 4;
                        scrollY *= 4;
                    }
                    if (_activeController.State.Buttons[ControllerButton.Decelerate2x])
                    {
                        scrollX /= 2;
                        scrollY /= 2;
                    }
                    if (_activeController.State.Buttons[ControllerButton.Decelerate4x])
                    {
                        scrollX /= 4;
                        scrollY /= 4;
                    }
                    _mouseEventService.Scroll(scrollX, scrollY);
                }

                if (_activeController.State.Buttons[ControllerButton.LeftButton] && !lastLeftState)
                {
                    lastLeftState = true;
                    _mouseEventService.SendLeftDown();
                }
                else if (!_activeController.State.Buttons[ControllerButton.LeftButton] && lastLeftState)
                {
                    lastLeftState = false;
                    _mouseEventService.SendLeftUp();
                }

                if (_activeController.State.Buttons[ControllerButton.RightButton] && !lastRightState)
                {
                    lastRightState = true;
                    _mouseEventService.SendRightDown();
                }
                else if (!_activeController.State.Buttons[ControllerButton.RightButton] && lastRightState)
                {
                    lastRightState = false;
                    _mouseEventService.SendRightUp();
                }

                Thread.Sleep(10);

                if (x == 0 && y == 0 && scrollX == 0 && scrollY == 0)
                {
                    _controllerStateChanged.WaitOne();
                }
            }
            StopService();
        }

        // TODO: implement these
        public event EventHandler<ControllerContainerEventArgs>? ControllerAdded;
        public event EventHandler<ControllerContainerEventArgs>? ControllerRemoved;

        public event EventHandler ExitButtonPressed;
        public event EventHandler SettingsButtonPressed;
    }
}
