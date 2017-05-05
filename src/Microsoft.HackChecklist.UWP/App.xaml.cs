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

using System;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Microsoft.HackChecklist.UWP
{
    sealed partial class App : Application
    {
        public event EventHandler<string> StatusUpdated;
        public static AppServiceConnection Connection;

        private BackgroundTaskDeferral _appServiceDeferral;

        public App()
        {
            InitializeComponent();

            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.PreferredLaunchViewSize = new Size(960, 670);
        }

        protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            base.OnBackgroundActivated(args);
            if (!(args.TaskInstance.TriggerDetails is AppServiceTriggerDetails)) return;

            _appServiceDeferral = args.TaskInstance.GetDeferral();
            args.TaskInstance.Canceled += OnTaskCanceled; // Associate a cancellation handler with the background task.

            var details = args.TaskInstance.TriggerDetails as AppServiceTriggerDetails;
            if (details != null)
            {
                Connection = details.AppServiceConnection;
            }
            Connection.RequestReceived += ConnectionOnRequestReceived;
        }

        private void ConnectionOnRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            //the Win32 app has sent us a message. If it's the message to confirm that the channel has been open, we raise the StatusUpdated event
            if (args.Request.Message.ContainsKey("Status"))
            {
                var currentStatus = args.Request.Message["Status"].ToString();
                StatusUpdated?.Invoke(this, currentStatus);
            }
        }

        private void OnTaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            _appServiceDeferral?.Complete();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                DebugSettings.EnableFrameRateCounter = false;
            }
#endif
            var rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                rootFrame = new Frame {Language = Windows.Globalization.ApplicationLanguages.Languages[0]};
                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                }
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                rootFrame.Navigate(typeof(View.MainView), e.Arguments);
            }
            Window.Current.Activate();
        }

        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }
    }
}