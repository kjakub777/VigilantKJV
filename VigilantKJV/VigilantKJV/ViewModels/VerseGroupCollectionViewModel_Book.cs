using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Text;
using VigilantKJV.DataAccess;

namespace VigilantKJV.ViewModels
{
    public class VerseGroupCollectionViewModel_Book: VerseCollectionViewModel
    {
        protected override async System.Threading.Tasks.Task ExecuteLoadItemsCommandAsync()
        {
            try
            {
                if (IsBusy)
                    return;
                IsBusy = true;
                using (var DBAccess = DataStoreFactory.GetNewDataStore())
                {
                    Items.Clear();
                    var dateList = (await DBAccess.GetMemorizedGroupByBook());


                    if (dateList != null && dateList.Count > 0)
                    {
                        try

                        {
                            foreach (var b in dateList)
                                Items.Add(new DateRecitedViewModel(b.GroupKey, b.Collection, false));
                        }
                        catch (Exception ex)
                        {
                            UserDialogs.Instance.Toast($"{ex}");
                        }
                        UserDialogs.Instance.Toast($"Loaded bookviewmodels.");
                    }
                    else
                    {
                        IsEmpty = true;
                    }
                }

            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert($"The following error occurred:\n{ex}");
                Console.WriteLine($"The following error occurred:\n{ex}");
                System.Diagnostics.Debug.WriteLine($"The following error occurred:\n{ex}");
                throw;
            }
            finally
            {
                IsBusy = false;
            }
        }

    }
}
