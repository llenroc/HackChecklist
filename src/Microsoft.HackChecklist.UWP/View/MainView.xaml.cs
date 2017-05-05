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
        public MainView()
        {
            InitializeComponent();
            IoCConfiguration.Configure();
            DataContext = IoCConfiguration.GetType<MainViewModel>();
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
            var vm = (DataContext as MainViewModel);
            if (vm != null && vm.IsShownChecklist)
            {
                vm.IsShownChecklist = false;  
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

    }
}
