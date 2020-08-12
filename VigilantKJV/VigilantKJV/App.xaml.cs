using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using VigilantKJV.Services;
using VigilantKJV.Views;
using VigilantKJV.Models;

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
            InitializeComponent();
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                System.Exception ex = (System.Exception)args.ExceptionObject;
                Console.WriteLine(ex);
            };
          //  DependencyService.Register<NavigationService>();
            DependencyService.Register<MyKjvContext>();

            MainPage = new MainPage();
        }

        protected async override void OnStart()
        {
            //#if DEBUG
            //            DataAccess.DataStore store = new DataAccess.DataStore();
            //            await store.SeedData(null, false);
            //#endif
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
