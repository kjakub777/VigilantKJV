using Acr.UserDialogs;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VigilantKJV.Models;
using VigilantKJV.Services;

using Xamarin.Forms;

namespace VigilantKJV.ViewModels
{
    public class LastRecitedViewModel : BaseViewModel
    {
        public DataAccess.DataStore DBAccess { get; set; }
        public List<(Guid, DateTime)> previousLastReciteds;
        public Verse SelectedVerse { get; set; }
        public LastRecitedViewModel()
        {
            DBAccess = new DataAccess.DataStore();
            Title = "Last Time Recited";
            Items = new ObservableCollection<Verse>();
            LoadVersesCommand = new Command(async () => await ExecuteLoadVersesCommand());
            previousLastReciteds = new List<(Guid, DateTime)>();
        }
        public ObservableCollection<Verse> Items { get; set; }
        public Command LoadVersesCommand { get; set; }

        internal async Task UpdateRecited(Verse item)
        {
            previousLastReciteds.Add((item.Id, item.LastRecited));
            await DBAccess.UpdateRecited(item.Id,null);
            IsBusy = true;
        }
        internal async Task UpdateRecitedUndo(Verse item)
        {

            if (previousLastReciteds.Any(x => x.Item1 == item.Id))
            {
                var prev = previousLastReciteds.FirstOrDefault(x => x.Item1 == item.Id);
                previousLastReciteds.Remove(prev);
                await DBAccess.UpdateRecited(prev.Item1, prev.Item2);
                IsBusy = true;
            }
            else
            {
                UserDialogs.Instance.Toast("No previous value stored.");
            }
        }
        

        public async Task ExecuteLoadVersesCommand()
        {
            IsBusy = true;
            try
            {
                Items.Clear();
                var vs = await DBAccess.GetLastRecitedAsync();
                vs.ForEach((v) => Items.Add(v));

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
