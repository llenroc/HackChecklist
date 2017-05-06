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

namespace Microsoft.HackChecklist.UWP.Services
{
    public static class AnalyticsConfiguration
    {
        public const string TrackingId = "UA-97593129-1";

        public const bool IsDebug = false;
        public const bool ReportUncaughtExceptions = true;
        public const bool AutoAppLifetimeMonitoring = true;
        
        public const string WelcomeScreenName = "WelcomeView";
        public const string TestsScreenName = "TestView";

        public const string CheckCategory = "Check";
        public const string AppCategory = "App";

        public const string AppInstalledEvent = "AppInstalledAndLaunched";
        public const string ChecklistStartedAction = "ChecklistStarted";
        public const string ChecklistCompletedAction = "ChecklistCompleted";

        public const string ChecklistCheckSingleRequirementEvent = "ChecklistSingleRequirementTested";

        public const string ChecklistFailedEvent = "ChecklistFailed";
        public const string ChecklistFailedSingletonEvent = "ChecklistFailedSingleton";
        public const string ChecklistPassedEvent = "ChecklistPassed";
        public const string ChecklistPassedSingletonEvent = "ChecklistPassedSingleton";
        public const string ChecklistPassedAllEvent = "ChecklistPassedAll";
        public const string ChecklistFailedAgainEvent = "ChecklistFailedAgain";
        public const string ChecklistPassedAfterFailingEvent = "ChecklistPassedAfterFailing";
    }
}
