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
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Microsoft.HackChecklist.Models.Enums;
using Windows.UI.Xaml;

namespace Microsoft.HackChecklist.UWP.Converters
{
    public class StatusToResourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        { 
            return value is ResponseStatus && (ResponseStatus) value == ResponseStatus.Failed
                ? Application.Current.Resources["FailedColorBrush"] as SolidColorBrush
                : Application.Current.Resources["SecondaryColorBrush"] as SolidColorBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
