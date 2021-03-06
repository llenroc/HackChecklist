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

using Microsoft.HackChecklist.Models.Contracts;
using System.Collections.Generic;

namespace Microsoft.HackChecklist.Models
{
    public class Software : ISoftware
    {
        public string Name { get; set; }

        public string AdditionalInformation { get; set; }

        public IEnumerable<Check> Checks { get; set; }

        public bool IsOptional { get; set; }

        public string InstallationNotes { get; set; }

        public bool ActivateLoading { get; set; }
    }
}
