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
using System.Diagnostics;
using System.Text;

namespace Microsoft.HackChecklist.BackgroundProcess
{
    public static class CmdChecker
    {
        private static int lineCount = 0;
        private static StringBuilder output = new StringBuilder();

        public static string RunCommand(string command)
        {
            Process process = new Process();

            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.Arguments = "/C " + command;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;

            process.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
            {
                // Prepend line numbers to each line of the output.
                if (!String.IsNullOrEmpty(e.Data))
                {
                    lineCount++;
                    output.Append("\n[" + lineCount + "]: " + e.Data);
                }
            });

            process.Start();

            process.BeginOutputReadLine();
            process.WaitForExit();
            process.Close();

            return output.ToString();
        }
    }
}
