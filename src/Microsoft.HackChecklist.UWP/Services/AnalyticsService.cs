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

using GoogleAnalytics;
using Microsoft.HackChecklist.UWP.Contracts;
using Microsoft.HockeyApp;
using Microsoft.Services.Store.Engagement;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System.Profile;

namespace Microsoft.HackChecklist.UWP.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private Tracker _tracker;
        private StoreServicesCustomEventLogger _logger;

        public Tracker Tracker
        {
            get
            {
                return _tracker ?? Init();
            } 
        }
        public void TrackEvent(string category, string action, string label = null, long value = 0)
        {
           Tracker.Send(HitBuilder.CreateCustomEvent(category, action, label, value).Build());
            _logger.Log(action);
            HockeyClient.Current.TrackEvent(action);
        }

        public void TrackScreen(string screenName)
        {
            Tracker.ScreenName = screenName;
            Tracker.Send(HitBuilder.CreateScreenView().Build());
        }

        private Tracker Init()
        {
            _logger = StoreServicesCustomEventLogger.GetDefault();

            _tracker = AnalyticsManager.Current.CreateTracker(AnalyticsConfiguration.TrackingId);
            AnalyticsManager.Current.IsDebug = AnalyticsConfiguration.IsDebug;
            AnalyticsManager.Current.ReportUncaughtExceptions = AnalyticsConfiguration.ReportUncaughtExceptions;
            AnalyticsManager.Current.AutoAppLifetimeMonitoring = AnalyticsConfiguration.AutoAppLifetimeMonitoring;
            return _tracker;
        }

        public static string GetDeviceInfo(bool verbatim = false)
        {
            try
            {
                string SystemFamily = "";
                string SystemVersion = "";
                string SystemArchitecture = "";
                string ApplicationVersion = "";
                string DeviceManufacturer = "";
                string DeviceModel = "";

                // get the system family name
                AnalyticsVersionInfo ai = AnalyticsInfo.VersionInfo;
                SystemFamily = ai.DeviceFamily;

                // get the system version number
                string sv = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
                ulong v = ulong.Parse(sv);
                ulong v1 = (v & 0xFFFF000000000000L) >> 48;
                ulong v2 = (v & 0x0000FFFF00000000L) >> 32;
                ulong v3 = (v & 0x00000000FFFF0000L) >> 16;
                ulong v4 = (v & 0x000000000000FFFFL);
                SystemVersion = $"{v1}.{v2}.{v3}.{v4}";

                // get the package architecure
                Package package = Package.Current;
                SystemArchitecture = package.Id.Architecture.ToString();

                // get the app version
                ApplicationVersion = GetAppVersion();

                // get the device manufacturer and model name
                EasClientDeviceInformation eas = new EasClientDeviceInformation();
                DeviceManufacturer = eas.SystemManufacturer;
                DeviceModel = eas.SystemProductName;

                return $"{SystemFamily}|{SystemVersion}|{SystemArchitecture}|{ApplicationVersion}|{DeviceManufacturer}|{DeviceModel}";
            }
            catch
            {
                return "";
            }
        }

        public static string GetAppVersion()
        {
            Package package = Package.Current;
            PackageVersion pv = package.Id.Version;
            return $"{pv.Major}.{pv.Minor}.{pv.Build}.{pv.Revision}";
        }

    }
}
