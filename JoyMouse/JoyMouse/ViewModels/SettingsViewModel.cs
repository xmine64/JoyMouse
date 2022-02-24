using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;

using JoyMouse.Interfaces;
using JoyMouse.Models;

namespace JoyMouse.ViewModels
{
    internal class SettingsViewModel : ObservableObject, ISettingsViewModel
    {
        private readonly IControllerService _controllerService;

        private IController? _controller;

        public SettingsViewModel(IControllerService controllerService)
        {
            _controllerService = controllerService;
            Controllers = new ObservableCollection<IController>(_controllerService.Controllers);
            _controllerService.ControllerAdded += ControllerService_ControllerAdded;
            _controllerService.ControllerRemoved += ControllerService_ControllerRemoved;
        }

        public IList<IController> Controllers { get; }

        public IController? SelectedController
        {
            get => _controller;
            set
            {
                SetProperty(ref _controller, value);
                _controllerService.SetActiveController(value);
                _controllerService.StartService();
            }
        }

        private void ControllerService_ControllerAdded(object? sender, ControllerContainerEventArgs args)
        {
            Controllers.Add(args.Target);
        }

        private void ControllerService_ControllerRemoved(object? sender, ControllerContainerEventArgs args)
        {
            Controllers.Remove(args.Target);
        }
    }
}
