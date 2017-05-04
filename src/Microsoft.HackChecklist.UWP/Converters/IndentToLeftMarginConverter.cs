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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Microsoft.HackChecklist.UWP.Converters
{
    public class IndentToLeftMarginConverter : IValueConverter
    {
        public static int IndentationSize = 43;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var indent = value != null && value is int ? (int)value : 0;
            return new Thickness { Left = indent * IndentationSize, Bottom = 0, Right = 0, Top = 0 };
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }        
    }
}
