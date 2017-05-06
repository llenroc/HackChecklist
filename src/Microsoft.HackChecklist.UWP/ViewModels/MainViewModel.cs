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
using Microsoft.HackChecklist.Models.Consts;
using Microsoft.HackChecklist.Services.Contracts;
using Microsoft.HackChecklist.UWP.Contracts;
using Microsoft.HackChecklist.UWP.Services;
using Microsoft.HackChecklist.UWP.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.Resources;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using static Windows.ApplicationModel.FullTrustProcessLauncher;
using ResponseStatus = Microsoft.HackChecklist.Models.Enums.ResponseStatus;

namespace Microsoft.HackChecklist.UWP.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public const string ConfigurationFileName = "configuration.json";
        public const string ConfigFileUrl = @"https://raw.githubusercontent.com/nmetulev/HackChecklist/migration/src/Microsoft.HackChecklist.UWP/configuration.json";            

        private readonly IJsonSerializerService _jsonSerializerService;
        private readonly IAppDataService _appDataService;
        private readonly IAnalyticsService _analyticsService;
        private readonly INetworkService _networkService;
        private readonly INotificationService _notificationService;

        private readonly ResourceLoader _resourceLoader = new ResourceLoader();

        private string _message;
        private bool _isChecking;
        private bool _isShownToChecklist = false;
        private string _messageChecking;
        private string _messageChecked;

        public MainViewModel(IJsonSerializerService jsonSerializerService, IAppDataService appDataService, IAnalyticsService analyticsService,
            INetworkService networkService, INotificationService notificationService)
        {
            _jsonSerializerService = jsonSerializerService;
            _appDataService = appDataService;
            _analyticsService = analyticsService;
            _networkService = networkService;
            _notificationService = notificationService;
            Init();
        }

        public ICommand CheckRequirementsCommand => new RelayCommand(CheckRequirementsAction, CheckRequirementsCan);

        public ObservableCollection<RequirementViewModel> Requirements { get; } = new ObservableCollection<RequirementViewModel>();

        public string MessageChecking
        {
            get => _messageChecking;
            set
            {
                _messageChecking = value;
                OnPropertyChanged();
            }
        }

        public string MessageChecked
        {
            get => _messageChecked;
            set
            {
                _messageChecked = value;
                OnPropertyChanged();
            }
        }

        public bool IsShownChecklist
        {
            get => _isShownToChecklist;
            set
            {
                _analyticsService.TrackScreen(value ?
                    AnalyticsConfiguration.TestsScreenName : AnalyticsConfiguration.WelcomeScreenName);

                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    value ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
                _isShownToChecklist = value;
                OnPropertyChanged();
            }
        }

        public bool IsChecking
        {
            get => _isChecking;
            set
            {
                _isChecking = value;
                (CheckRequirementsCommand as RelayCommand).RaiseCanExecuteChanged();
                OnPropertyChanged();
            }
        }
        
        public async void Init()
        {
            await _notificationService.NotificationsAsync(NotificationTags.NotTested);

            string strConfiguration = await GetConfiguration();          
            Configuration configuration;
            try
            {
                configuration = _jsonSerializerService.Deserialize<Configuration>(strConfiguration);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

            foreach (var requirement in configuration.Requirements)
            {
                AddRequirement(requirement, 0);
            }

            if (!LocalDataService.IsSet(LocalDataService.FirstLaunchProperty))
            {
                LocalDataService.Set(LocalDataService.FirstLaunchProperty);
                _analyticsService.TrackEvent(AnalyticsConfiguration.AppCategory, AnalyticsConfiguration.AppInstalledEvent);
            }

            _analyticsService.TrackScreen(AnalyticsConfiguration.WelcomeScreenName);
        }

        private async Task<string> GetConfiguration()
        {
            var configuration = string.Empty;
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            try
            {
                configuration = await _networkService.Get(ConfigFileUrl);
                if (localFolder != null)
                {
                    StorageFile storedConfiguration = await localFolder.CreateFileAsync(ConfigurationFileName, CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(storedConfiguration, configuration);
                }
                if (!string.IsNullOrEmpty(configuration)) return configuration;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);                             
            }
            try
            {
                StorageFile storedConfiguration = await localFolder.GetFileAsync(ConfigurationFileName);
                configuration = await FileIO.ReadTextAsync(storedConfiguration);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                configuration = await _appDataService.GetDataFile(ConfigurationFileName);
            }
            return configuration;
        }
		
		private void AddRequirement(Requirement requirement, int indentation)
        {
            if (requirement == null) return;

            Requirements.Add(new RequirementViewModel(requirement, indentation));
            if (requirement.Modules?.Any() ?? false)
            {
                foreach (var module in requirement.Modules)
                {
                    AddRequirement(module, indentation+1);
                }
            }
        }

        private bool CheckRequirementsCan()
        {
            return !IsChecking;
        }

        private async void CheckRequirementsAction()
        {
            IsShownChecklist = true;
            IsChecking = true;
            MessageChecking = _resourceLoader.GetString("TitleChecking");
            MessageChecked = string.Empty;

            _analyticsService.TrackEvent(AnalyticsConfiguration.CheckCategory, AnalyticsConfiguration.ChecklistStartedAction, null, 0);

            await LaunchBackgroundProcess();

            if (App.Connection == null) return;

            bool AllPassed = true;
            bool RequiredPassed = true;

            foreach (var requirement in Requirements)
            {
                var status = await CheckRequirement(requirement);
                if (AllPassed && !status.passed) AllPassed = false;
                if (RequiredPassed && status.required && !status.passed) RequiredPassed = false;
            }

            ShowMessageResponse();
            UpdateNotificationTags();
            SendCompletedTestsAnalytics(AllPassed, RequiredPassed);
            _analyticsService.TrackEvent(AnalyticsConfiguration.CheckCategory, AnalyticsConfiguration.ChecklistCompletedAction, null, 0);

            IsChecking = false;
        }

        private async Task LaunchBackgroundProcess()
        {
            try
            {
                await LaunchFullTrustProcessForCurrentAppAsync();
                await Task.Delay(1000); // quick fix, need to make it better
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }
        }

        private async Task<(bool passed, bool required)> CheckRequirement(RequirementViewModel requirement)
        {
            var valueSet = new ValueSet { { BackgroundProcessCommand.RunChecks, _jsonSerializerService.Serialize(requirement.ModelObject) } };

            requirement.Status = ResponseStatus.Processing;
            requirement.IsLoading = true;
            requirement.NeedUpdateInformation = requirement.AdditionalInformation;

            var response = await App.Connection.SendMessageAsync(valueSet);
            var passed = (response?.Message.Keys.Contains(requirement.Name) ?? false) && (bool)response.Message[requirement.Name];

            ChangeStatus(passed, requirement);

            _analyticsService.TrackEvent(
                AnalyticsConfiguration.CheckCategory,
                AnalyticsConfiguration.ChecklistCheckSingleRequirementEvent,
                requirement.Name,
                passed ? 1 : 0);

            return (passed, requirement.IsOptional);
        }

        private void ChangeStatus(bool passed, RequirementViewModel requirement)
        {
            if (passed)
            {
                requirement.Status = ResponseStatus.Success;
            }
            else
            {
                if (requirement.IsOptional)
                {
                    requirement.Status = ResponseStatus.None;
                }
                else
                {
                    requirement.Status = ResponseStatus.Failed;
                }

                requirement.IsUpdateFailed = true;

                var additionalInformation = requirement.AdditionalInformation;
                if (!string.IsNullOrEmpty(additionalInformation))
                {
                    requirement.NeedUpdateInformation =
                        string.Format(_resourceLoader.GetString("NeedUpdate"), additionalInformation);
                }
            }
            requirement.IsLoading = false;
        }

        private void ShowMessageResponse()
        {
            MessageChecking = _resourceLoader.GetString("TitleCheckCompleted");
            MessageChecked = Requirements.Any(requirement => !requirement.IsOptional && requirement.Status != ResponseStatus.Success)
                ? _resourceLoader.GetString("SubTitleCheckFile")
                : _resourceLoader.GetString("SubTitleCheckSucces");
        }

        private void UpdateNotificationTags()
        {
            var tag = Requirements.All(requirement => requirement.Status == ResponseStatus.Success)
                ? NotificationTags.AllTestsPassed
                : Requirements.Any(requirement => !requirement.IsOptional && requirement.Status != ResponseStatus.Success)
                    ? NotificationTags.RequiredTestsFailed
                    : NotificationTags.RequiredTestsPassed;

            _notificationService.NotificationsAsync(tag);
        }

        private void SendCompletedTestsAnalytics( bool allPassed, bool requiredPassed)
        {
            if (allPassed)
            {
                _analyticsService.TrackEvent(
                    AnalyticsConfiguration.CheckCategory, 
                    AnalyticsConfiguration.ChecklistPassedAllEvent, null, 0);
            }
            else if (requiredPassed)
            {
                _analyticsService.TrackEvent(
                    AnalyticsConfiguration.CheckCategory, 
                    AnalyticsConfiguration.ChecklistPassedEvent, null, 0);
            }
            else
            {
                _analyticsService.TrackEvent(
                    AnalyticsConfiguration.CheckCategory,
                    AnalyticsConfiguration.ChecklistFailedEvent, null, 0);
            }

            if (requiredPassed && !LocalDataService.IsSet(LocalDataService.PassedProperty))
            {
                LocalDataService.Set(LocalDataService.PassedProperty);
                _analyticsService.TrackEvent(
                    AnalyticsConfiguration.CheckCategory, 
                    AnalyticsConfiguration.ChecklistPassedSingletonEvent, null, 0);
            }

            if (requiredPassed && LocalDataService.IsSet(LocalDataService.FailedProperty))
            {
                _analyticsService.TrackEvent(
                    AnalyticsConfiguration.CheckCategory,
                    AnalyticsConfiguration.ChecklistPassedAfterFailingEvent, null, 0);
            }

            if (!requiredPassed && !LocalDataService.IsSet(LocalDataService.FailedProperty))
            {
                LocalDataService.Set(LocalDataService.FailedProperty);
                _analyticsService.TrackEvent(
                    AnalyticsConfiguration.CheckCategory,
                    AnalyticsConfiguration.ChecklistFailedSingletonEvent, null, 0);
            }
            else if (!requiredPassed)
            {
                _analyticsService.TrackEvent(
                    AnalyticsConfiguration.CheckCategory,
                    AnalyticsConfiguration.ChecklistFailedAgainEvent, null, 0);
            }
        }
    }
}
