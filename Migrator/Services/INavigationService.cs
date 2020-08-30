using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using VigilantKJV.Models;

namespace VigilantKJV.Services
{
    public interface INavigationService
    {
        Task NavigateToBibleMain();

        Task NavigateToMemorized();

        Task NavigateToDetailVerseView(Verse v);


        Task NavigateToDbTools();
        Task NavigateToLastRecited();
    }
}
