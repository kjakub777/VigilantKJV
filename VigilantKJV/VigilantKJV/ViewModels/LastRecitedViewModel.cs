using Acr.UserDialogs;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VigilantKJV.Models;
using Xamarin.Forms;

namespace VigilantKJV.ViewModels
{
    public class LastRecitedViewModel : BaseViewModel
    {
        public List<(Guid, DateTime)> previousLastReciteds;
        public Verse SelectedVerse { get;set;}
        DataAccess.DataStore db;
        public LastRecitedViewModel()
        {            Title = "Last Time Recited";
            Items = new ObservableCollection<Verse>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            db = new DataAccess.DataStore();
            previousLastReciteds = new List<(Guid, DateTime)>();
        }
        public ObservableCollection<Verse> Items { get; set; }
        public Command LoadItemsCommand { get; set; }

        internal async Task UpdateRecited(Verse item)
        {
            previousLastReciteds.Add((item.Id, item.LastRecited));
            await db.UpdateRecited(item);
        }
        internal async Task UpdateRecitedUndo(Verse item)
        {

            if (previousLastReciteds.Any(x => x.Item1 == item.Id))
            {
                var prev = previousLastReciteds.FirstOrDefault(x => x.Item1 == item.Id);
                previousLastReciteds.Remove(prev);
                await db.UpdateRecited(prev.Item1, prev.Item2);
            }
            else
            {
                UserDialogs.Instance.Toast("No previous value stored.");
            }
        }
        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;
            try
            {
                await ExecuteLoadVersesCommand();
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

        public async Task ExecuteLoadVersesCommand()
        {
            IsBusy = true;
            try
            {
                Items.Clear();
                var vs = await db.GetLastRecitedAsync();
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
