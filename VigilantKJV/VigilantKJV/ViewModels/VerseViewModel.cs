using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VigilantKJV.DataAccess;
using VigilantKJV.Models;
using VigilantKJV.Services;

using Xamarin.Forms;

namespace VigilantKJV.ViewModels
{
    public class VerseViewModel : BaseViewModel
    {
        DataAccess.DataStore db;
        private Verse verse;
        private int verseId;
        DateTime? previous;
        private DateTime lastRecited;
        public VerseViewModel(Verse verse)
        {
            db = DataStoreFactory.GetNewDataContext();
            isLoading = true;
            Title = verse.FullTitle;
            verseId = verse.Id;
            previous = null;
            this.verse = verse;
            IsMemorized = verse.IsMemorized;
            isLoading = false;
            this.PropertyChanged += OnPropertyChanged;
        }
        bool isLoading = false;
        bool isMemorized;
        public bool IsMemorized
        {
            get
            {
                return isMemorized;
            }

            set
            {
                if (isMemorized == value)
                {
                    return;
                }
                isMemorized = value;
                SetProperty(ref this.isMemorized, value);
            }
        }


        internal async Task<bool> SetMemorized(bool value)
        {
            IsBusy = true;
            if (isLoading)
                return true;
            try
            {
                db.Update(verse);
                verse.IsMemorized = IsMemorized = value;
                var ret = await db.SaveChangesAsync();

                SetProperty(ref this.isMemorized, value, nameof(IsMemorized));
                return ret > 0;
            }
            catch (Exception ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.Alert($"There was an error:\n{ex}");
            }
            IsBusy = false;
            return false;
        }
        public string sLastRecited => verse?.LastRecited.ToShortDateString();

        public string VerseName { get { return verse.ChapVerseText; } }
        //    public int TypeID { get { return _verse.IdTypeID; } }
        internal async Task<bool> UpdateRecited()
        {
            if (isLoading)
                return false;
            try
            {
                var temp = verse.LastRecited;
                db.Update(verse);
                verse.LastRecited = DateTime.Now.ToLocalTime();

                var ret = await db.SaveChangesAsync() > 0;
                if (ret)
                {
                    previous = temp;
                    SetProperty(ref this.lastRecited, verse.LastRecited, nameof(LastRecited));
                }
                return ret;
            }
            catch (Exception ex)
            {

                Acr.UserDialogs.UserDialogs.Instance.Alert($"{ex}");
                Console.WriteLine($"{ex}");
            }
            return false;
        }
        public string LastRecitedCaption
        {
            get => verse.LastRecitedCaption;
        }
        internal async Task<bool> UpdateRecitedUndo()
        {
            if (isLoading)
                return false;
            IsBusy = true;
            try
            {

                db.Update(verse);
                verse.LastRecited = previous.Value;

                var ret = await db.SaveChangesAsync() > 0;
                if (ret)
                {
                    SetProperty(ref this.lastRecited, verse.LastRecited, nameof(LastRecited));
                }
                return ret;
            }
            catch (Exception ex)
            {

                Acr.UserDialogs.UserDialogs.Instance.Alert($"{ex}");
                Console.WriteLine($"{ex}");
            }
            return false;
        }
        void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }
        public Verse Verse
        {
            get => verse;
            set => SetProperty(ref verse, value, nameof(this.Verse));
        }
        public DateTime LastRecited
        {
            get { return lastRecited; }
            set
            {
                if (lastRecited == value)
                {
                    return;
                }

                lastRecited = value;
                SetProperty(ref this.lastRecited, value, nameof(LastRecited));
            }
        }
    }
}
