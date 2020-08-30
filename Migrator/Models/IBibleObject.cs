using System;
using System.Collections.Generic;
using System.Text;

namespace VigilantKJV.Models
{
    public interface IBibleObject
    {
        Guid Id { get;set;}
        String FriendlyLabel { get;}
        int Number { get;}
        string Name { get;}
    }
}
