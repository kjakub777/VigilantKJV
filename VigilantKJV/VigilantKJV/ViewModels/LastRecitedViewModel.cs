using Acr.UserDialogs;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VigilantKJV.DataAccess;
using VigilantKJV.Models;
using VigilantKJV.Services;

using Xamarin.Forms;

namespace VigilantKJV.ViewModels
{
    public class LastRecitedViewModel : BaseViewModel
    {
        private List<(int, DateTime)> previousLastReciteds;

        public Verse SelectedVerse { get; set; }

        public LastRecitedViewModel()
        {
            data = DataStoreFactory.GetNewDataContext();
            Title = "Last Time Recited";
            Items = new ObservableCollection<Verse>();
            LoadVersesCommand = new Command(async () => await ExecuteLoadVersesCommand());
            PreviousLastReciteds = new List<(int, DateTime)>();
        }

        public ObservableCollection<Verse> Items { get; set; }

        public Command LoadVersesCommand { get; set; }

        public List<(int, DateTime)> PreviousLastReciteds
        {
            get
            {
                if (previousLastReciteds == null)
                {
                    previousLastReciteds = new List<(int, DateTime)>();
                }
                return previousLastReciteds;
            }
            set
            {
                previousLastReciteds = value;
            }
        }

        internal async Task UpdateRecited(Verse v)
        {

            try
            {
                PreviousLastReciteds.Add((v.Id, v.LastRecited));
                await data.UpdateRecited(v);
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Toast($"Error \n{ex}.");
            }
            ExecuteLoadVersesCommand();

        }
        internal async Task UpdateRecitedUndo()
        {
            try
            {
                if (PreviousLastReciteds.Any(x => x.Item1 == SelectedVerse.Id))
                {
                    var prev = PreviousLastReciteds.FirstOrDefault(x => x.Item1 == SelectedVerse.Id);
                    await data.UpdateRecited(prev.Item1, prev.Item2);
                    PreviousLastReciteds.Remove(prev);
                    IsBusy = true;
                }
                else
                {
                    UserDialogs.Instance.Toast("No previous value stored.");
                }
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Toast($"Error \n{ex}.");
            }
            RaisePropertyChanged(nameof(SelectedVerse));
        }

        DataAccess.DataStore data;


        public async Task ExecuteLoadVersesCommand()
        {
            IsBusy = true;


            try
            {
                Items.Clear();
                var vs = await data.GetLastRecitedAsync();

                try
                {
                    foreach (var v in vs)
                    {
                        Items.Add(v);
                    }
                }
                catch (Exception ex)
                {

                    Acr.UserDialogs.UserDialogs.Instance.Alert($"{ex}");
                    Console.WriteLine($"{ex}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            finally
            {
                IsBusy = false;
            }

        }
    }
}
