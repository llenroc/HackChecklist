﻿using Windows.UI.Xaml.Controls;
using Microsoft.HackChecklist.UWP.ViewModels;

namespace Microsoft.HackChecklist.UWP.View
{
    public sealed partial class MainView : Page
    {
        public MainView()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}