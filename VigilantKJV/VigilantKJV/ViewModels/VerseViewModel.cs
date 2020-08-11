using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VigilantKJV.Models;

namespace VigilantKJV.ViewModels
{
    public class VerseViewModel : BaseViewModel
    {

        private Verse _verse;
        private bool isMemorized;
        DateTime? previous ;
        public VerseViewModel(Verse verse)
        {previous = null;
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
                SetProperty(ref isMemorized, value);
                SetMemorized(Verse, value);
            }
        } 
        internal async void SetMemorized(Verse v1, bool v2)
        {
        IsBusy = true;
            if (v2 != v1.IsMemorized)
            {
                DBAccess.DB.Update(v1);
                await DBAccess.DB.SaveChangesAsync();
            }
            IsBusy = false;
        }
        public string sLastRecited => _verse?.LastRecited.ToShortDateString();

        public string VerseName { get { return _verse.ChapVerseText; } }
        //    public int TypeID { get { return _verse.IdTypeID; } }
        internal async Task UpdateRecited(Verse item)
        {
            previous = item.LastRecited; 
            await DBAccess.UpdateRecited(item);
        }
        internal async Task UpdateRecitedUndo(Verse item)
        {
            var temp= item.LastRecited;
            await DBAccess.UpdateRecited(item,previous);
            previous = temp;
        }
        public Verse Verse
        {
            get => _verse;
        }
    }
}
