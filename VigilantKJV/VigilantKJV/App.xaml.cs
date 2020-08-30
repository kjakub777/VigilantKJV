using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using VigilantKJV.Services;
using VigilantKJV.Views;
using VigilantKJV.Models;
using VigilantKJV.Helpers;
using VigilantKJV.ViewModels;
using FreshMvvm;

namespace VigilantKJV
{
    public partial class App : Application
    {
        //TODO: Replace with *.azurewebsites.net url after deploying backend to Azure
        //To debug on Android emulators run the web backend against .NET Core not IIS
        //If using other emulators besides stock Google images you may need to adjust the IP address
        //public static string AzureBackendUrl =
        //    DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5000" : "http://localhost:5000";
        //public static bool UseMockDataStore = true;

        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzA2Mjc3QDMxMzgyZTMyMmUzMGs2dGNZUEZWcUZxQkRpNi9KVHJ6WHVKL1lwdWRTTEEyN3pLTTBuaXBWZVk9;MzA2Mjc4QDMxMzgyZTMyMmUzMGRBKzlQTjJqcFQ4SXhSMm9TZkRvZUE4M3B5UWNneVJGWW9XcDUyeHpxdTg9;MzA2Mjc5QDMxMzgyZTMyMmUzMEQ1dmNPRmN3a1htVFNieEw5THVkRkZ5MllmdWNmSlFMdzFZeWtVYlBqamc9;MzA2MjgwQDMxMzgyZTMyMmUzMGxjU1dlSVF3dlFCRXJVL21QMWtlQ2t5MmJqTjVwazhYdWZjU2JSVW91ZUk9;MzA2MjgxQDMxMzgyZTMyMmUzMEFESXN6QncvcStDMjBDL2l0TXJ5TTRBRi9XcjlmN3FUS1JIbkd5NVU2OGs9;MzA2MjgyQDMxMzgyZTMyMmUzMGNrWjZOOUJ2SnRsKzFkamswMDNPc0ZPamc4MnZOaTQxQ01YbTRkNUVicXc9;MzA2MjgzQDMxMzgyZTMyMmUzMG9CS09DaUlEZkRmTWx4dUJGb0pWRDBsNS9mRGZiOWV0OU5Ca1RUNnp4c0E9;MzA2Mjg0QDMxMzgyZTMyMmUzMGlIbVpNYmFtMTFHSTVwMHNieUIvd2IxQ0sybVJwT2RtZ3pLWktHWDRhaEE9;MzA2Mjg1QDMxMzgyZTMyMmUzMEJNT0IvRmZEYjZvUDlCcFp5ZE1NV0xpRUFFL2lQUm82YWJadU1SejBwTGs9;NT8mJyc2IWhia31hfWN9Z2doYmF8YGJ8ampqanNiYmlmamlmanMDHmg4OTI4JjFkZGQTND4yOj99MDw+;MzA2Mjg2QDMxMzgyZTMyMmUzMFVLb1pjeldHdGFtbXhhWVV4aFFFV3RJY0huZ1VuZFJ5RzNyUlh4TXZJc1E9");
            InitializeComponent();
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                System.Exception ex = (System.Exception)args.ExceptionObject;
                Console.WriteLine(ex);
            }; 
            DependencyService.Register<MyKjvContext>();
            DependencyService.Register<DataAccess.DataStore>();

            //MainPage = new MainPage();

            FreshPageModelResolver.PageModelMapper = new FreshViewModelMapper();
        }

        protected override void OnStart()
        {
            /*  new HomeMenuItem {Id = MenuItemType.Memorized, Title="Memory Collection",Image="lightbulb.ico" },
                new HomeMenuItem {Id = MenuItemType.LastRecited, Title="Last Recited",Image="DateTime.png" },
                new HomeMenuItem {Id = MenuItemType.DbTools, Title="Db Tools",Image="process_accept.ico" },
                new HomeMenuItem {Id = MenuItemType.About, Title="About" */
            var masterDetailNav = new MasterDetailNavigationContainer();
            masterDetailNav.Init("Menu", "ic_toolbar_Bars");
            masterDetailNav.AddPage<BibleViewModel>("Kjv Main", "Kjv", "biblecross.png" /* '\uf200'.ToString()*/, null); // piechart icon
            masterDetailNav.AddPage<BibleGridViewModel>("Kjv Grid", "Kjv", "biblecross.png" /* '\uf200'.ToString()*/, null); // piechart icon
            masterDetailNav.AddPage<MemoryVerseGroupCollectionViewModel>("Memory Verses", "Items", "lightbulb.ico"  /*'\uf128'.ToString()*/, null); // question icon
            masterDetailNav.AddPage<LastRecitedViewModel>("Last Recited", "Items", "DateTime.png" /*'\uf128'.ToString()*/, null); // question icon
            masterDetailNav.AddPage<DbToolsViewModel>("DB Tools", "Items", "process_accept.ico"  /*'\uf129'.ToString()*/, null); // info icon
            masterDetailNav.AddPage<AboutViewModel>("About", "Settings",  '\uf129'.ToString() , null); // info icon
       
            MainPage = masterDetailNav;
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
