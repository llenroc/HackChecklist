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
        public const string CheckAllRequirementsAction = "CheckAllRequirementsAction";
        public const string CheckRequirementAction = "CheckRequirementAction";
        public const string AllRequiredPassedAction = "AllRequiredTestsPassed";
        public const string AllTestPassedAction = "AllTestPassed";
        public const string RequiredFailedAction = "RequiredTestsFailed";

    }
}
