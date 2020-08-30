using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace VigilantKJV.ViewModels
{
    public interface IBaseViewModel
    { 
        bool IsBusy { get; set; }
        bool IsEmpty { get; set; }
        string Title { get; set; }


        bool SetProperty<T>(ref T backingStore,
                                    T value,
                                    [CallerMemberName] string propertyName = "",
                                    Action onChanged = null);
     

        void RaisePropertyChangedEvent(PropertyChangedEventArgs e);
        #region INotifyPropertyChanged
          event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string propertyName = "");
        #endregion
    }
}
