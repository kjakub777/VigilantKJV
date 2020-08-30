using System;
using System.Collections.Generic;
using System.Text;

namespace VigilantKJV.Models
{
    public interface IBibleObject
    {
        int Id { get;set;}
        String FriendlyLabel { get;}
        int Number { get;}
        string Name { get;}
    }
}
