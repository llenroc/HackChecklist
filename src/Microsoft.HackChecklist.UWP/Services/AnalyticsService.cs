﻿//*********************************************************
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
using Microsoft.Services.Store.Engagement;

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
    }
}
