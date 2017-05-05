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

using Microsoft.HackChecklist.UWP.Contracts;
using Microsoft.WindowsAzure.Messaging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Networking.PushNotifications;

namespace Microsoft.HackChecklist.UWP.Services
{
    public class NotificationService : INotificationService
    {
        private const string AzureHubName = "hackchecklistnotificationhub";
        private const string AzureHubConnectionString = "Endpoint=sb://hackchecklistnotificationnamespace.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=Gu79KCbjUSZzL96IMco7ZXK/N60oJrmAdf2yP4XSSbY=";

        public async Task NotificationsAsync(string tag)
        {
            try
            {
                var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
                var hub = new NotificationHub(AzureHubName, AzureHubConnectionString);

                var result = await hub.RegisterNativeAsync(channel.Uri, new List<string> { tag });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
