﻿using System;
using System.Windows.Input;

using VigilantKJV.Services;

using Xamarin.Essentials;
using Xamarin.Forms;

namespace VigilantKJV.ViewModels
{
    public class AboutViewModel : BaseViewModel
    { 
        public AboutViewModel()
        { 
            Title = "About";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://xamarin.com"));
        }

        public ICommand OpenWebCommand { get; }
    }
}