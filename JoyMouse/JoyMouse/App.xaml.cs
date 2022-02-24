using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.Extensions.DependencyInjection;
using Windows.UI.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;

using JoyMouse.Interfaces;
using JoyMouse.Services;
using JoyMouse.Views;
using JoyMouse.ViewModels;

namespace JoyMouse
{
    public partial class App : Application
    {
        private static IServiceProvider RegisterServices()
        {
            var collection = new ServiceCollection();

            collection.AddTransient<ISettingsViewModel, SettingsViewModel>();

            collection.AddSingleton<IControllerService, ControllerService>();
            collection.AddSingleton<IMouseEventService, MouseEventService>();

            return collection.BuildServiceProvider();
        }

        private Window? m_window;

        public App()
        {
            InitializeComponent();
            Container = RegisterServices();
            var svcController = Container.GetRequiredService<IControllerService>();
            svcController.SettingsButtonPressed += SvcController_SettingsButtonPressed;
            svcController.ExitButtonPressed += SvcController_ExitButtonPressed;
        }

        public IServiceProvider Container { get; }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new SettingsWindow();
            m_window.Activate();
        }

        private void SvcController_SettingsButtonPressed(object? sender, EventArgs e)
        {
            m_window?.DispatcherQueue.TryEnqueue(() =>
            {
                m_window.Activate();
            });
        }

        private void SvcController_ExitButtonPressed(object? sender, EventArgs e)
        {
            var svcController = Container.GetRequiredService<IControllerService>();
            svcController.StopService();
            Exit();
        }
    }
}
