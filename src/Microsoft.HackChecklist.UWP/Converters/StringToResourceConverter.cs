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
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Microsoft.HackChecklist.UWP.Converters
{
    class StringToResourceConverter : IValueConverter
    {
        private readonly ResourceLoader _resourceLoader = new ResourceLoader();

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value is String && (String) value == _resourceLoader.GetString("SubTitleCheckSucces")
                ? Application.Current.Resources["SuccessColorBrush"] as SolidColorBrush
                : Application.Current.Resources["FailedColorBrush"] as SolidColorBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
