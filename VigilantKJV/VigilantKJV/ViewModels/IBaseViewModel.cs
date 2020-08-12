using System;

using System.Threading.Tasks;

using Xamarin.Forms;

namespace VigilantKJV.ViewModels
{
    public interface IBaseViewModel
    { 
        bool IsBusy { get; set; }
        bool IsEmpty { get; set; }
        string Title { get; set; }
    }
}
