using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using VigilantKJV.Models;
using VigilantKJV.Services;

using Xamarin.Forms;

namespace VigilantKJV.ViewModels
{
    public class VerseViewModel : BaseViewModel
    {
        public DataAccess.DataStore DBAccess { get; set; }

        private Verse _verse;
        private Guid verseId;
        private bool isMemorized;
        DateTime? previous;
         
        public VerseViewModel(Verse verse,DataAccess.DataStore DBAccess  )
        {   this.DBAccess = DBAccess;
            Title = verse.FullTitle;
            verseId = verse.Id;
            previous = null;
            this._verse = verse;
        }
        public bool IsMemorized
        {
            get
            {
                return isMemorized;
            }
            set
            {
                SetMemorized();
                SetProperty(ref isMemorized, value);
            }
        }
        internal async Task<bool> SetMemorized()
        {
            IsBusy = true;

            try
            {
                var v = await DBAccess.DB.Verse.FindAsync(verseId);
                v.IsMemorized = !v.IsMemorized;
                DBAccess.DB.Update(v);
               return 1== await DBAccess.DB.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.Alert($"There was an error:\n{ex}");
            }
            IsBusy = false;
                                     return false;
        }
        public string sLastRecited => _verse?.LastRecited.ToShortDateString();

        public string VerseName { get { return _verse.ChapVerseText; } }
        //    public int TypeID { get { return _verse.IdTypeID; } }
        internal async Task UpdateRecited(Verse item)
        {
            IsBusy = true;

            var v = item ?? await DBAccess.DB.Verse.FindAsync(verseId);
            previous = v.LastRecited;
            await DBAccess.UpdateRecited(v.Id,null);
            IsBusy = false;
        }
        internal async Task UpdateRecitedUndo(Verse item)
        {
            IsBusy = true;

            var v = item ?? await DBAccess.DB.Verse.FindAsync(verseId);
            var temp = v.LastRecited;
            await DBAccess.UpdateRecited(v.Id, previous);
            previous = temp;
            IsBusy = false;
        }
        public Verse Verse
        {
            get => _verse;
            set=>SetProperty(ref _verse, value,nameof(this.Verse));
        }
    }
}
