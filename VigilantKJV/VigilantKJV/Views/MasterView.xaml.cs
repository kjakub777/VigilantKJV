﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using VigilantKJV.Models;

namespace VigilantKJV.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MasterView : ContentPage
    {
      //  Dictionary<int, NavigationPage> MenuPages = new Dictionary<int, NavigationPage>();
        public MasterView()
        {
            InitializeComponent();

            //MasterBehavior = MasterBehavior.Popover;

            //MenuPages.Add((int)MenuItemType.Bible, (NavigationPage)Detail);
        }

        //public async Task NavigateFromMenu(int id)
        //{
        //    if (!MenuPages.ContainsKey(id))
        //    {
        //        switch (id)
        //        {
        //            case (int)MenuItemType.Bible:
        //                MenuPages.Add(id, new NavigationPage(new BiblePage()));
        //                break;
        //            case (int)MenuItemType. Memorized:
        //                MenuPages.Add(id, new NavigationPage(new MemorizedPage()));
        //                break;
        //            case (int)MenuItemType.DbTools:
        //                MenuPages.Add(id, new NavigationPage(new DbToolsPage()));
        //                break;
        //            case (int)MenuItemType.LastRecited:
        //                MenuPages.Add(id, new NavigationPage(new LastRecitedPage()));
        //                break;
        //            
        //            case (int)MenuItemType.About:
        //                MenuPages.Add(id, new NavigationPage(new AboutPage()));
        //                break;
        //        }
        //    }

        //    var newPage = MenuPages[id];

        //    if (newPage != null && Detail != newPage)
        //    {
        //        Detail = newPage;

        //        if (Device.RuntimePlatform == Device.Android)
        //            await Task.Delay(100);

        //        IsPresented = false;
        //    }
        //}
    }
}