//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Windows.UI.Xaml.Controls;
using Microsoft.HackChecklist.UWP.ViewModels;

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.System.Profile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ViewManagement;
using Windows.Foundation;
using Windows.UI.Core;

namespace Microsoft.HackChecklist.UWP.View
{
    public sealed partial class MainView : Page
    {
        private MainViewModel _viewModel;
        public MainView()
        {
            InitializeComponent();
            IoCConfiguration.Configure();
            _viewModel = IoCConfiguration.GetType<MainViewModel>() as MainViewModel;
            _viewModel.PropertyChanged += Vm_PropertyChanged;
            DataContext = _viewModel;
            ((App)Application.Current).StatusUpdated += MainPage_StatusUpdated;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e) 
        {
            SystemNavigationManager.GetForCurrentView().BackRequested += MainView_BackRequested; ;

            ProgressBar.IsActive = true;
            //we check if the app is running on the desktop: only if that's the case, we leverage the Desktop Bridge specific features
            if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Desktop")
            {
                try
                {
                    await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
                    await Task.Delay(1000); // quick fix, need to make it better

                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception.Message);
                }
            }
        }

        private void MainView_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (_viewModel != null && _viewModel.IsShownChecklist)
            {
                _viewModel.IsShownChecklist = false;  
            }
        }

        private async void MainPage_StatusUpdated(object sender, string e)
        {
            //the Win32 app has initialized the channel with the App Service, so we hide the ProgressRing
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                ProgressBar.IsActive = false;
            });
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.CheckRequirementsCommand.Execute(null);
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // quick fix, binding was not getting triggered in xaml
            if (e.PropertyName == "IsChecking")
            {
                RestartButton.IsEnabled = !_viewModel.IsChecking;
            }
        }

        private void FeedbackLink_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ProvideFeedback();
        }
    }
}
