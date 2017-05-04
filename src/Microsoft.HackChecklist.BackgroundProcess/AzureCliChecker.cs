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

using System.Diagnostics;

namespace Microsoft.HackChecklist.BackgroundProcess
{
    public static class AzureCliChecker
    {
        public static bool IsInstalled()
        {
            Process process = new Process();

            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/C az --version";
            process.StartInfo.UseShellExecute = false;

            process.StartInfo.RedirectStandardOutput = true;

            process.Start();
            
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return output?.Contains("azure-cli") ?? false;
        }
    }
}
