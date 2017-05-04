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

using Microsoft.HackChecklist.Models;
using Microsoft.HackChecklist.Models.Enums;
using Microsoft.HackChecklist.Services;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Microsoft.Win32;
using Microsoft.HackChecklist.BackgroundProcess.Extensions;

namespace Microsoft.HackChecklist.BackgroundProcess
{
    internal class Program
    {
        static AppServiceConnection _connection;
        static AutoResetEvent _appServiceExit;

        private static void Main()
        {
            //we use an AutoResetEvent to keep to process alive until the communication channel established by the App Service is open
            _appServiceExit = new AutoResetEvent(false);
            var appServiceThread = new Thread(ThreadProc);
            appServiceThread.Start();
            _appServiceExit.WaitOne();
        }

        private static async void ThreadProc()
        {
            //we create a connection with the App Service defined by the UWP app
            _connection = new AppServiceConnection();
            _connection.AppServiceName = "CommunicationService";
            _connection.PackageFamilyName = Windows.ApplicationModel.Package.Current.Id.FamilyName;
            _connection.RequestReceived += Connection_RequestReceived;
            _connection.ServiceClosed += Connection_ServiceClosed;

            var status = await _connection.OpenAsync();

            if (status != AppServiceConnectionStatus.Success)
            {
                //if the connection fails, we terminate the Win32 process
                _appServiceExit.Set();
            }
            else
            {
                //if the connection is succesfull, we communicate to the UWP app that the channel has been established
                var initialStatus = new ValueSet {{"Status", "Ready"}};
                await _connection.SendMessageAsync(initialStatus);
            }
        }

        private static void Connection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            //when the connection with the App Service is closed, we terminate the Win32 process
            _appServiceExit.Set();
        }

        private static async void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            Requirement value;
            Debug.WriteLine($"Received valueSet: {args.Request.Message.First().Value}");
            try
            {
                value = new JsonSerializerService().Deserialize<Requirement>(args.Request.Message.First().Value.ToString());
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }
            var valueSet = new ValueSet();
            var software = (Software)value;
            if (software != null)
            {
                valueSet.Add(software.Name, CheckRequirement(software));
            }
            Debug.WriteLine($"Responsing valueSet: {valueSet}");
            await args.Request.SendResponseAsync(valueSet);
        }

        private static bool CheckRequirement(Software software)
        {
            string registryValue;
            var checkResult = false;
            Debug.WriteLine($"Checking: {software.Name}");
            switch (software.CheckType)
            {
                case CheckType.RegistryValue:
                    registryValue = RegistryChecker.GetRegistryValue(
                        ParseRegistryHive(software.RegistryHive),
                        software.RegistryKey,
                        software.RegistryValue);
                    checkResult = string.CompareOrdinal(registryValue, software.RegistryExpectedValue) == 0;
                    Debug.WriteLine($" -----> {registryValue}");
                    break;
                case CheckType.IncludedInRegistry:
                    var registryValues = RegistryChecker.GetRegistryValues(
                        ParseRegistryHive(software.RegistryHive),
                        software.RegistryKey,
                        software.RegistryValue);
                    checkResult = registryValues?.Any(value =>
                                      value.Contains(software.RegistryExpectedValue, StringComparison.InvariantCultureIgnoreCase)) ?? false;
                    Debug.WriteLine($" -----> {registryValues.ToList().Count()}");
                    break;
                case CheckType.VisualStudioInstalled:
                    checkResult = new VisualStudioChecker().IsVisualStudio2017Installed();
                    break;
                case CheckType.VisualStudioWorkloadInstalled:
                    checkResult = new VisualStudioChecker().IsWorkloadInstalled(software.RegistryKey);
                    break;
                case CheckType.MinimumVisualStudioWorkloadInstalled:
                    checkResult = new VisualStudioChecker().IsWorkloadInstalled(software.RegistryKey, software.RegistryExpectedValue);
                    break;
                case CheckType.MinimumRegistryValue:
                    registryValue = RegistryChecker.GetRegistryValue(
                        ParseRegistryHive(software.RegistryHive),
                        software.RegistryKey,
                        software.RegistryValue);
                    checkResult = string.CompareOrdinal(registryValue, software.RegistryExpectedValue) >= 0;
                    Debug.WriteLine($" -----> {registryValue}");
                    break;
                case CheckType.AzureCliInstalled:
                    checkResult = AzureCliChecker.IsInstalled();
                    break;
            }
            Debug.WriteLine($" -----> Passed: {checkResult}");
            return checkResult;
        }

        private static RegistryHive ParseRegistryHive(string hive)
        {
            return Enum.TryParse(hive, out RegistryHive result) ? result : RegistryHive.LocalMachine;
        }
    }
}
