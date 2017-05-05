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

using Microsoft.HackChecklist.Models.Enums;

namespace Microsoft.HackChecklist.Models
{
    public class Check
    {
        public CheckType CheckType { get; set; }

        public string RegistryHive { get; set; }

        public string RegistryKey { get; set; }

        public string RegistryValue { get; set; }

        public string Command { get; set; }

        public string ExpectedValue { get; set; }
    }
}
